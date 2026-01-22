using System.Numerics;
using System.Runtime.CompilerServices;
using Ab4d.SharpEngine.Core;
using Ab4d.SharpEngine.SceneNodes;
using FE3.VoxelRenderer.Utils;
namespace FE3.VoxelRenderer.VoxelWorld;

public sealed class VoxelWorld
{
    public static int ChunkSize = 32;
    
    private readonly Dictionary<Int2, UnmanagedVoxelModel> _chunks = new();
    private readonly ChunkLoader _loader;
    public VoxelWorld(ChunkLoader loader)
    {
        _loader = loader;
    }

    public void PrintKeys()
    {
        foreach (var key in _chunks.Keys)
        {
            Console.WriteLine($"Key: {key}");
        }
    }
    public async Task<UnmanagedVoxelModel> LoadChunkAsync(Int2 chunk, CancellationToken ct = default)
    {
        if (_chunks.TryGetValue(chunk, out var model))
            return model;

        model = await _loader(chunk, ct);
        _chunks.Add(chunk, model);

        return model;
    }
    
    public bool UnloadChunk(Int2 chunk) {
        if (_chunks.TryGetValue(chunk, out var model))
        {
            model.Dispose();
            _chunks.Remove(chunk);
            return true;
        }
        return false;
    }
    
    
    public MeshModelNode RenderChunk(Int2 chunk, GpuImage? diffuseTexture)
    {
        if (!_chunks.TryGetValue(chunk, out var model))
            throw new InvalidOperationException($"Chunk {chunk} is not loaded.");

        var greedy = new GreedyVoxelModel(ref model);

        return greedy.BuildMesh(diffuseTexture);
    }
    
    public void RenderAllChunks(GpuImage? diffuseTexture, Action<Int2, MeshModelNode> visitor)
    {
        foreach (var kv in _chunks)
        {
            var chunk = kv.Key;
            var model = kv.Value;

            var greedy = new GreedyVoxelModel(ref model);
            var mesh = greedy.BuildMesh(diffuseTexture);

            visitor(chunk, mesh);
        }
    }
    
    
    
    
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int2 WorldToChunk(Vector3 worldPos)
    {
        int cx = (int)MathF.Floor(worldPos.X / ChunkSize);
        int cz = (int)MathF.Floor(worldPos.Z / ChunkSize);
        return new Int2(cx, cz);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsChunkLoaded(Int2 chunk) => _chunks.ContainsKey(chunk);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int3 WorldToLocalVoxel(Vector3 worldPos, Int2 chunk)
    {
        int lx = (int)MathF.Floor(worldPos.X) - chunk.X * ChunkSize;
        int ly = (int)MathF.Floor(worldPos.Y);
        int lz = (int)MathF.Floor(worldPos.Z) - chunk.Y * ChunkSize;
        return new Int3(lx, ly, lz);
    }
    
    public void Dispose()
    {
        foreach (var model in _chunks.Values)
            model.Dispose();

        _chunks.Clear();
    }
}

public delegate Task<UnmanagedVoxelModel> ChunkLoader(Int2 chunk, CancellationToken ct);
