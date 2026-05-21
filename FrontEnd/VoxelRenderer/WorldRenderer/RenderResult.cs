using FrontEnd.VoxelRenderer.Utils.Types;

namespace FrontEnd.VoxelRenderer.WorldRenderer;

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