// VoxelWorld.cs
using System.Collections.Concurrent;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using Ab4d.SharpEngine.Common;
using Ab4d.SharpEngine.Materials;
using Ab4d.SharpEngine.Meshes;
using Ab4d.SharpEngine.SceneNodes;
using FE3.VoxelRenderer.Utils;

namespace FE3.VoxelRenderer.VoxelWorld;

public sealed class VoxelWorld : IDisposable
{
    public static readonly int ChunkSize = 32;

    private readonly Dictionary<Int2, UnmanagedVoxelModel> _chunks = new();
    private readonly ChunkLoader _loader;

    private readonly Channel<RenderTask> _renderQueue =
        Channel.CreateBounded<RenderTask>(new BoundedChannelOptions(4096)
        {
            SingleReader = false,
            SingleWriter = false,
            FullMode = BoundedChannelFullMode.DropOldest
        });

    private readonly ConcurrentQueue<RenderResult> _completedMeshes = new();
    private readonly ConcurrentDictionary<Int2, byte> _renderInFlight = new();
    private readonly ConcurrentDictionary<Int2, CancellationTokenSource> _chunkRenderCts = new();

    private readonly CancellationTokenSource _renderCts = new();
    private readonly List<Task> _renderWorkers = new();

    private int _started;

    public VoxelWorld(ChunkLoader loader)
    {
        _loader = loader;
    }

    public void StartRenderWorkers(int workerCount = 0)
    {
        if (Interlocked.Exchange(ref _started, 1) == 1)
            return;

        if (workerCount <= 0)
            workerCount = Math.Max(1, Environment.ProcessorCount - 1);

        for (int i = 0; i < workerCount; i++)
        {
            _renderWorkers.Add(Task.Run(RenderWorkerLoop));
        }
    }

    public bool CreateRenderTask(Int2 chunk, CancellationToken externalCt = default)
    {
        if (!_chunks.ContainsKey(chunk))
            return false;

        if (!_renderInFlight.TryAdd(chunk, 0))
            return false;

        // cancel any previous per-chunk render request
        if (_chunkRenderCts.TryRemove(chunk, out var oldCts))
        {
            try { oldCts.Cancel(); } catch { }
            oldCts.Dispose();
        }

        var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_renderCts.Token, externalCt);
        _chunkRenderCts[chunk] = linkedCts;

        // bounded channel may drop; if enqueue fails, release in-flight + dispose CTS
        if (!_renderQueue.Writer.TryWrite(new RenderTask(chunk, linkedCts)))
        {
            _chunkRenderCts.TryRemove(chunk, out _);
            linkedCts.Dispose();
            _renderInFlight.TryRemove(chunk, out _);
            return false;
        }

        return true;
    }

    private async Task RenderWorkerLoop()
    {
        var reader = _renderQueue.Reader;

        try
        {
            while (await reader.WaitToReadAsync(_renderCts.Token))
            {
                while (reader.TryRead(out var task))
                {
                    try
                    {
                        if (task.Token.IsCancellationRequested)
                            continue;

                        if (!_chunks.TryGetValue(task.Chunk, out var model))
                            continue;

                        // CPU-only work
                        var greedy = new GreedyVoxelModel(ref model);

                        // IMPORTANT: BuildMeshData must be CPU-only and return MeshData-compatible payload.
                        var mesh = greedy.BuildMeshData();

                        if (!task.Token.IsCancellationRequested)
                        {
                            _completedMeshes.Enqueue(new RenderResult(task.Chunk, mesh));
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        // ignored
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Render error {task.Chunk}: {ex}");
                    }
                    finally
                    {
                        _renderInFlight.TryRemove(task.Chunk, out _);

                        if (_chunkRenderCts.TryRemove(task.Chunk, out var cts) && ReferenceEquals(cts, task.Cts))
                        {
                            // dispose only if it's the same CTS that belongs to this task
                        }

                        task.Cts.Dispose();
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
            // shutdown
        }
    }

    // Budgeted drain to avoid frame freezes: only process up to maxCount results per call.
    public int DrainCompletedMeshes(int maxCount, StandardMaterial material, Action<Int2, MeshModelNode> visitor)
    {
        int count = 0;

        while (count < maxCount && _completedMeshes.TryDequeue(out var result))
        {
            var rm = result.Mesh;

            var node = new MeshModelNode(
                new TriangleMesh<PositionNormalTextureVertex>(rm.Vertices, rm.Indices),
                material);

            visitor(result.Chunk, node);
            count++;
        }

        return count;
    }

    public async Task<UnmanagedVoxelModel> LoadChunkAsync(Int2 chunk, CancellationToken ct = default)
    {
        if (_chunks.TryGetValue(chunk, out var model))
            return model;

        model = await _loader(chunk, ct);
        _chunks.Add(chunk, model);

        return model;
    }

    public bool UnloadChunk(Int2 chunk)
    {
        // If chunk is queued or being rendered, refuse unload (prevents use-after-dispose).
        if (_renderInFlight.ContainsKey(chunk))
            return false;

        if (_chunkRenderCts.TryRemove(chunk, out var cts))
        {
            try { cts.Cancel(); } catch { }
            cts.Dispose();
        }

        if (_chunks.TryGetValue(chunk, out var model))
        {
            model.Dispose();
            _chunks.Remove(chunk);
            return true;
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsChunkLoaded(Int2 chunk) => _chunks.ContainsKey(chunk);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int2 WorldToChunk(Vector3 worldPos)
    {
        int cx = (int)MathF.Floor(worldPos.X / ChunkSize);
        int cz = (int)MathF.Floor(worldPos.Z / ChunkSize);
        return new Int2(cx, cz);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int3 WorldToLocalVoxel(Vector3 worldPos, Int2 chunk)
    {
        int lx = (int)MathF.Floor(worldPos.X) - chunk.X * ChunkSize;
        int ly = (int)MathF.Floor(worldPos.Y);
        int lz = (int)MathF.Floor(worldPos.Z) - chunk.Y * ChunkSize;
        return new Int3(lx, ly, lz);
    }

    public async Task StopRenderingAsync()
    {
        try { _renderCts.Cancel(); } catch { }
        _renderQueue.Writer.TryComplete();

        try { await Task.WhenAll(_renderWorkers); }
        catch { }

        _renderWorkers.Clear();

        while (_completedMeshes.TryDequeue(out _)) { }

        foreach (var kv in _chunkRenderCts)
        {
            try { kv.Value.Cancel(); } catch { }
            kv.Value.Dispose();
        }
        _chunkRenderCts.Clear();
    }

    public void Dispose()
    {
        StopRenderingAsync().GetAwaiter().GetResult();
        _renderCts.Dispose();

        foreach (var model in _chunks.Values)
            model.Dispose();

        _chunks.Clear();
    }
}

public delegate Task<UnmanagedVoxelModel> ChunkLoader(Int2 chunk, CancellationToken ct);
