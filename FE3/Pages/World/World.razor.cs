using System.Numerics;
using System.Collections.Concurrent;
using Ab4d.SharpEngine.Cameras;
using Ab4d.SharpEngine.Common;
using Ab4d.SharpEngine.Core;
using Ab4d.SharpEngine.Lights;
using Ab4d.SharpEngine.Materials;
using Ab4d.SharpEngine.SceneNodes;
using Ab4d.SharpEngine.Transformations;
using Ab4d.SharpEngine.Utilities;
using FE3.VoxelRenderer;
using FE3.VoxelRenderer.Utils;
using FE3.VoxelRenderer.VoxelWorld;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Colors = Ab4d.SharpEngine.Common.Colors;

namespace FE3.Pages;

public partial class World : IAsyncDisposable
{
    [Parameter] public string WorldId { get; set; } = null!;

    SharpEngineSceneView _view = null!;
    FirstPersonCamera _camera = null!;

    VoxelWorld _world = null!;
    GpuImage _diffuseGpuImage = null!;

    readonly Dictionary<Int2, MeshModelNode> _rendered = new();

    CancellationTokenSource _cts = null!;
    Task _loopTask = Task.CompletedTask;

    ElementReference _inputHost;
    DotNetObjectReference<World> _dotnetRef = null!;
    IJSObjectReference _jsModule = null!;

    // Streaming
    Int2 _playerChunk;
    int _loadRadius = 8;

    // Unload behavior:
    // - Keep a safety margin beyond load radius to prevent thrashing
    // - Unload lazily with a per-tick budget
    int _unloadMargin = 2;                 // unload radius = _loadRadius + _unloadMargin
    const int UnloadBudgetPerTick = 8;     // limit per tick
    int _unloadCursor;                    // round-robin cursor for lazy unload

    // Timing
    long _lastTickTs;

    // Materials
    StandardMaterial _chunkMaterial = null!;

    // Loading gate (prevents duplicate load requests)
    readonly ConcurrentDictionary<Int2, byte> _loadInFlight = new();

    // ---------------------------
    // Lazy async streaming queue
    // ---------------------------
    readonly object _queueLock = new();
    PriorityQueue<Int2, int> _loadQueue = new(); // min priority => nearest first
    readonly HashSet<Int2> _queued = new();      // queued but not yet in-flight

    // Cap concurrent loads (network + copy + deserialize)
    readonly SemaphoreSlim _loadSemaphore = new(initialCount: 2, maxCount: 2);

    Task[] _streamWorkers = Array.Empty<Task>();

    // Avoid spending too much time just enqueuing
    const int EnqueueBudgetPerTick = 32;

    protected override void OnAfterRender(bool firstRender)
    {
        if (!firstRender) return;

        InitScene();
        _view.SceneViewInitialized += OnSceneViewInitialized;
    }

    async void OnSceneViewInitialized(object? sender, EventArgs e)
    {
        _diffuseGpuImage = await TextureLoader.CreateTextureAsync("Atlas.png", _view.Scene);

        _world = new VoxelWorld(async (chunk, ct) =>
        {
            await using var s = await WorldApi.GetChunkAsync(WorldId, chunk.X, chunk.Y, ct);

            using var ms = new MemoryStream();
            await s.CopyToAsync(ms, ct);

            return UnmanagedVoxelModel.Deserialize(ms.ToArray());
        });

        _world.StartRenderWorkers();

        _dotnetRef = DotNetObjectReference.Create(this);
        _jsModule = await JS.InvokeAsync<IJSObjectReference>("import", "./worldInput.js");
        await _jsModule.InvokeVoidAsync("init", _inputHost, _dotnetRef);

        _cts = new CancellationTokenSource();
        _lastTickTs = Environment.TickCount64;

        // Small number of stream workers (lazy async loading)
        _streamWorkers = new[]
        {
            StreamWorkerAsync(_cts.Token),
            StreamWorkerAsync(_cts.Token)
        };

        _loopTask = TickLoopAsync(_cts.Token);
    }

    void InitScene()
    {
        var scene = _view.Scene;
        var view = _view.SceneView;

        view.BackgroundColor = Colors.LightSkyBlue;
        scene.SetAmbientLight(0.25f);
        scene.Lights.Add(new DirectionalLight(new Vector3(-1, -0.6f, -0.2f)));

        _camera = new FirstPersonCamera
        {
            CameraPosition = new Vector3(_playerPos.X, _playerPos.Y + _eyeHeight, _playerPos.Z),
            Heading = _heading,
            Attitude = _attitude,
            ShowCameraLight = ShowCameraLightType.Never
        };

        view.Camera = _camera;
    }

    async Task TickLoopAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            var now = Environment.TickCount64;
            var dt = (float)(now - _lastTickTs) / 1000f;
            _lastTickTs = now;

            dt = Math.Clamp(dt, 0.0f, 0.05f);

            ApplyMouseLook(dt);
            ApplyKeyboardMove(dt);

            UpdateStreaming(ct); // no await

            await Task.Delay(_tickMs, ct);
        }
    }

    // ---------------------------
    // Lazy async streaming helpers
    // ---------------------------
    bool IsDesiredChunk(in Int2 chunk)
    {
        var pc = _playerChunk;
        int r = _loadRadius;
        return Math.Abs(chunk.X - pc.X) <= r && Math.Abs(chunk.Y - pc.Y) <= r;
    }

    bool IsOutsideUnloadRadius(in Int2 chunk)
    {
        var pc = _playerChunk;
        int ur = _loadRadius + _unloadMargin;

        int dx = Math.Abs(chunk.X - pc.X);
        int dz = Math.Abs(chunk.Y - pc.Y);

        return dx > ur || dz > ur;
    }

    static int Dist2(in Int2 a, in Int2 b)
    {
        int dx = a.X - b.X;
        int dy = a.Y - b.Y;
        return dx * dx + dy * dy;
    }

    void EnqueueChunkIfNeeded(in Int2 chunk)
    {
        if (_world.IsChunkLoaded(chunk))
            return;

        if (_loadInFlight.ContainsKey(chunk))
            return;

        lock (_queueLock)
        {
            if (_queued.Contains(chunk))
                return;

            int prio = Dist2(chunk, _playerChunk); // nearest first
            _loadQueue.Enqueue(chunk, prio);
            _queued.Add(chunk);
        }
    }

    bool TryDequeue(out Int2 chunk)
    {
        lock (_queueLock)
        {
            if (_loadQueue.Count == 0)
            {
                chunk = default;
                return false;
            }

            chunk = _loadQueue.Dequeue();
            _queued.Remove(chunk);
            return true;
        }
    }

    async Task StreamWorkerAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            if (!TryDequeue(out var chunk))
            {
                await Task.Delay(10, ct);
                continue;
            }

            if (!IsDesiredChunk(chunk))
                continue;

            if (!_loadInFlight.TryAdd(chunk, 0))
                continue;

            try
            {
                await _loadSemaphore.WaitAsync(ct);
                try
                {
                    if (!IsDesiredChunk(chunk))
                        continue;

                    await _world.LoadChunkAsync(chunk, ct);

                    if (!IsDesiredChunk(chunk))
                        continue;

                    _world.CreateRenderTask(chunk, ct);
                }
                finally
                {
                    _loadSemaphore.Release();
                }
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[StreamWorker] failed chunk ({chunk.X},{chunk.Y}): {ex}");
            }
            finally
            {
                _loadInFlight.TryRemove(chunk, out _);
            }
        }
    }

    void LazyUnloadFarChunks()
    {
        if (_rendered.Count == 0)
            return;

        var keys = _rendered.Keys.ToArray();
        if (keys.Length == 0)
            return;

        if (_unloadCursor >= keys.Length)
            _unloadCursor = 0;

        int budget = UnloadBudgetPerTick;
        int scanned = 0;

        while (budget > 0 && scanned < keys.Length)
        {
            var c = keys[_unloadCursor];

            _unloadCursor++;
            if (_unloadCursor >= keys.Length)
                _unloadCursor = 0;

            scanned++;

            // Only unload if outside unload radius
            if (!IsOutsideUnloadRadius(c))
                continue;

            // If currently loading, don't fight it
            if (_loadInFlight.ContainsKey(c))
                continue;

            if (_world.UnloadChunk(c))
            {
                if (_rendered.TryGetValue(c, out var node))
                    node.Dispose();

                _rendered.Remove(c);
                budget--;
            }
        }
    }

    void UpdateStreaming(CancellationToken ct)
    {
        // Use camera as the streaming center; fallback to _playerPos if camera isn't ready yet.
        var camPos = _camera?.CameraPosition ?? new Vector3(_playerPos.X, _playerPos.Y, _playerPos.Z);

        _playerChunk = VoxelWorld.WorldToChunk(new Vector3(camPos.X, camPos.Y, camPos.Z));
        int r = _loadRadius;

        // 1) schedule loads / render tasks (minimal, budgeted)
        int enq = 0;

        for (int dz = -r; dz <= r; dz++)
        for (int dx = -r; dx <= r; dx++)
        {
            var chunk = new Int2(_playerChunk.X + dx, _playerChunk.Y + dz);

            if (_world.IsChunkLoaded(chunk))
            {
                if (!_rendered.ContainsKey(chunk))
                    _world.CreateRenderTask(chunk, ct);

                continue;
            }

            if (enq < EnqueueBudgetPerTick)
            {
                EnqueueChunkIfNeeded(chunk);
                enq++;
            }
        }

        // 2) drain meshes (GPU upload / scene-node creation)
        const int meshBudgetPerTick = 20;

        _world.DrainCompletedMeshes(
            meshBudgetPerTick,
            GetChunkMaterial(),
            (chunk, node) =>
            {
                node.Transform = new StandardTransform
                {
                    TranslateX = chunk.X * VoxelWorld.ChunkSize,
                    TranslateY = 0,
                    TranslateZ = chunk.Y * VoxelWorld.ChunkSize
                };

                _view.Scene.RootNode.Add(node);
                _rendered[chunk] = node;
            });

        // 3) lazy unload far chunks (budgeted)
        LazyUnloadFarChunks();
    }

    StandardMaterial GetChunkMaterial()
    {
        if (_chunkMaterial != null)
            return _chunkMaterial;

        _chunkMaterial = new StandardMaterial
        {
            DiffuseTexture = _diffuseGpuImage,
        };

        return _chunkMaterial;
    }

    void ClearSceneRoot()
    {
        _view.Scene.RootNode.DisposeWithAllChildren(
            disposeMeshes: true,
            disposeMaterials: true);

        _rendered.Clear();
    }

    public async ValueTask DisposeAsync()
    {
        _cts.Cancel();

        try
        {
            await _loopTask;
        }
        catch (OperationCanceledException)
        {
            // expected
        }

        try
        {
            if (_streamWorkers.Length > 0)
                await Task.WhenAll(_streamWorkers);
        }
        catch (OperationCanceledException)
        {
            // expected
        }

        await _jsModule.InvokeVoidAsync("dispose");
        await _jsModule.DisposeAsync();

        _dotnetRef.Dispose();

        ClearSceneRoot();
        _world.Dispose();
        _view.Dispose();

        _cts.Dispose();
    }
}
