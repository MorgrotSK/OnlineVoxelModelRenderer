using Ab4d.SharpEngine.Common;

namespace FrontEnd.VoxelRenderer.Utils.Types;

public readonly struct GreedyMeshData(PositionNormalTextureVertex[] vertices, int[] indices)
{
    public readonly PositionNormalTextureVertex[] Vertices = vertices;
    public readonly int[] Indices = indices;
}
