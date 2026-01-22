using Ab4d.SharpEngine.SceneNodes;
using FE3.VoxelRenderer.Utils;

namespace FE3.VoxelRenderer.VoxelWorld;

public readonly struct RenderResult
{
    public Int2 Chunk { get; }
    public GreedyMeshData Mesh { get; }

    public RenderResult(Int2 chunk, GreedyMeshData mesh)
    {
        Chunk = chunk;
        Mesh = mesh;
    }
}