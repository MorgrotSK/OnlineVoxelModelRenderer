using System.Numerics;
using Ab4d.SharpEngine.Common;
using Ab4d.SharpEngine.Meshes;
using FE3.VoxelRenderer.Blocks;
using FE3.VoxelRenderer.Utils.Types;
using SharedClass.UnmanagedStructures;

namespace FE3.VoxelRenderer;

public sealed class VoxelModel : IDisposable
{
    private UnmanagedVoxelModel _core;

    public VoxelModel(int depth, int initialCapacity = 128)
    {
        _core = new UnmanagedVoxelModel(depth, initialCapacity);
    }

    public void Dispose()
    {
        _core.Dispose();
    }

    public void SetVoxel(int x, int y, int z, byte value)
    {
        _core.SetVoxel(x, y, z, value);
    }

    public TriangleMesh<PositionNormalTextureVertex> BuildMesh()
    {
        _core.ExtractQuadsByAxis(out var xQuads, out var yQuads, out var zQuads);

        try
        {
            int quadCount = xQuads.Count + yQuads.Count + zQuads.Count;

            var vertices = new List<PositionNormalTextureVertex>(quadCount * 4);
            var indices = new List<int>(quadCount * 6);

            EmitMeshFromQuadList(xQuads, vertices, indices);
            EmitMeshFromQuadList(yQuads, vertices, indices);
            EmitMeshFromQuadList(zQuads, vertices, indices);

            return new TriangleMesh<PositionNormalTextureVertex>(
                vertices.ToArray(),
                indices.ToArray());
        }
        finally
        {
            xQuads.Dispose();
            yQuads.Dispose();
            zQuads.Dispose();
        }
    }

 private static void EmitMeshFromQuadList(UnmanagedList<Quad> quads, List<PositionNormalTextureVertex> vertices, List<int> indices)
    {
        for (int i = 0; i < quads.Count; i++)
        {
            ref var q = ref quads[i];

            int x = q.Position.X, y = q.Position.Y, z = q.Position.Z;
            int s = q.SizeU;

            Vector3 normal;
            Span<Vector3> c = stackalloc Vector3[4];

            ref readonly var block = ref BlockDictionary.Get(q.BlockId - 1);
            Float2X4 uv = block.UvPattern;

            switch (q.Axis)
            {
                case -1:
                    normal = -Vector3.UnitX;
                    c[0] = new Vector3(x, y + s, z);
                    c[1] = new Vector3(x, y + s, z + s);
                    c[2] = new Vector3(x, y, z + s);
                    c[3] = new Vector3(x, y, z);
                    break;

                case 1:
                    normal = Vector3.UnitX;
                    c[0] = new Vector3(x, y, z + s);
                    c[1] = new Vector3(x, y + s, z + s);
                    c[2] = new Vector3(x, y + s, z);
                    c[3] = new Vector3(x, y, z);
                    break;

                case -2:
                    normal = -Vector3.UnitY;
                    c[0] = new Vector3(x, y, z);
                    c[1] = new Vector3(x, y, z + s);
                    c[2] = new Vector3(x + s, y, z + s);
                    c[3] = new Vector3(x + s, y, z);
                    break;

                case 2:
                    normal = Vector3.UnitY;
                    c[0] = new Vector3(x, y, z);
                    c[1] = new Vector3(x + s, y, z);
                    c[2] = new Vector3(x + s, y, z + s);
                    c[3] = new Vector3(x, y, z + s);
                    break;

                case -3:
                    normal = -Vector3.UnitZ;
                    c[0] = new Vector3(x + s, y, z);
                    c[1] = new Vector3(x + s, y + s, z);
                    c[2] = new Vector3(x, y + s, z);
                    c[3] = new Vector3(x, y, z);
                    break;

                case 3:
                    normal = Vector3.UnitZ;
                    c[0] = new Vector3(x, y + s, z);
                    c[1] = new Vector3(x + s, y + s, z);
                    c[2] = new Vector3(x + s, y, z);
                    c[3] = new Vector3(x, y, z);
                    break;

                default:
                    continue;
            }

            int baseIndex = vertices.Count;

            for (int j = 0; j < 4; j++)
            {
                vertices.Add(new PositionNormalTextureVertex(
                    c[j],
                    normal,
                    new Vector2(uv[j].X, uv[j].Y)
                ));
            }

            indices.Add(baseIndex + 0);
            indices.Add(baseIndex + 2);
            indices.Add(baseIndex + 1);
            indices.Add(baseIndex + 0);
            indices.Add(baseIndex + 3);
            indices.Add(baseIndex + 2);

            if (block.IsTransparent)
            {
                indices.Add(baseIndex + 0);
                indices.Add(baseIndex + 1);
                indices.Add(baseIndex + 2);
                indices.Add(baseIndex + 0);
                indices.Add(baseIndex + 2);
                indices.Add(baseIndex + 3);
            }
        }
    }

    public byte[] Serialize() => _core.Serialize();

    public static VoxelModel Deserialize(ReadOnlySpan<byte> data)
    {
        var core = UnmanagedVoxelModel.Deserialize(data);
        var model = new VoxelModel(1);
        model._core.Dispose();
        model._core = core;
        return model;
    }
}
