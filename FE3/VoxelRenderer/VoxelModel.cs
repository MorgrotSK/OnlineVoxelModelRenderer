using System.Numerics;
using Ab4d.SharpEngine.Common;
using Ab4d.SharpEngine.Meshes;
using FE3.VoxelRenderer.Utils;
using FE3.VoxelRenderer.Utils.Octree;
using FE3.VoxelRenderer.Utils.UnmanagedStructures;

namespace FE3.VoxelRenderer;

public sealed class VoxelModel(int depth, int initialCapacity = 128) : IDisposable
{
    private FlatOctree _tree = new(depth, initialCapacity);
    
    public void Dispose() => _tree.Dispose();

    private UnmanagedList<Quad> ExtractAllQuads()
    {
        var quads = new UnmanagedList<Quad>(256);

        _tree.TraverseLeaves((x,y,z,size,data) =>
        {
            EmitLeafQuads(x, y, z, size, data, quads);
        });

        return quads;
    }
    
     public TriangleMesh<PositionNormalTextureVertex> BuildMesh()
    {
        using var quads = ExtractAllQuads();
        int quadCount = quads.Count;

        var vertices = new List<PositionNormalTextureVertex>(quadCount * 4);
        var indices  = new List<int>(quadCount * 6);

        for (int i = 0; i < quadCount; i++)
        {
            ref var q = ref quads[i];
            int x = q.Position.X, y = q.Position.Y, z = q.Position.Z;
            int s = q.Size;

            Vector3 normal;
            Span<Vector3> c = stackalloc Vector3[4];

            switch (q.Axis)
            {
                case -1:
                    normal = -Vector3.UnitX;
                    c[0] = new Vector3(x, y+s, z);
                    c[1] = new Vector3(x, y+s, z+s);
                    c[2] = new Vector3(x, y,   z+s);
                    c[3] = new Vector3(x, y,   z);
                    break;

                case 1:
                    normal = Vector3.UnitX;
                    c[0] = new Vector3(x, y,   z+s);
                    c[1] = new Vector3(x, y+s, z+s);
                    c[2] = new Vector3(x, y+s, z);
                    c[3] = new Vector3(x, y,   z);
                    break;

                case -2:
                    normal = -Vector3.UnitY;
                    c[0] = new Vector3(x,   y, z);
                    c[1] = new Vector3(x,   y, z+s);
                    c[2] = new Vector3(x+s, y, z+s);
                    c[3] = new Vector3(x+s, y, z);
                    break;

                case 2:
                    normal = Vector3.UnitY;
                    c[0] = new Vector3(x,   y, z);
                    c[1] = new Vector3(x+s, y, z);
                    c[2] = new Vector3(x+s, y, z+s);
                    c[3] = new Vector3(x,   y, z+s);
                    break;

                case -3:
                    normal = -Vector3.UnitZ;
                    c[0] = new Vector3(x+s, y,   z);
                    c[1] = new Vector3(x+s, y+s, z);
                    c[2] = new Vector3(x,   y+s, z);
                    c[3] = new Vector3(x,   y,   z);
                    break;

                case 3:
                    normal = Vector3.UnitZ;
                    c[0] = new Vector3(x,   y+s, z);
                    c[1] = new Vector3(x+s, y+s, z);
                    c[2] = new Vector3(x+s, y,   z);
                    c[3] = new Vector3(x,   y,   z);
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
                    new Vector2(j == 1 || j == 2 ? 1f : 0f,
                                j >= 2 ? 1f : 0f)));
            }

            indices.Add(baseIndex + 0);
            indices.Add(baseIndex + 2);
            indices.Add(baseIndex + 1);
            indices.Add(baseIndex + 0);
            indices.Add(baseIndex + 3);
            indices.Add(baseIndex + 2);
        }

        return new TriangleMesh<PositionNormalTextureVertex>(vertices.ToArray(), indices.ToArray());
        
    }
     
    public void SetVoxel(int x, int y, int z, byte value)
    {
        _tree.Insert(x, y, z, value);
    }
    
    private void EmitLeafQuads(int x, int y, int z, int size, byte data, UnmanagedList<Quad> quads)
    {
        for (int face = 0; face < 6; face++)
        {
            sbyte axis = (sbyte)((face >> 1) + 1);
            if ((face & 1) != 0) axis = (sbyte)-axis;

            int ai = (axis > 0 ? axis : -axis) - 1;

            int nx = x, ny = y, nz = z;
            if (axis > 0)
            {
                if (ai == 0) nx += size;
                else if (ai == 1) ny += size;
                else nz += size;
            }
            else
            {
                if (ai == 0) nx -= 1;
                else if (ai == 1) ny -= 1;
                else nz -= 1;
            }

            bool expose = _tree.Get(nx, ny, nz) != data;
            if (!expose) continue;

            int fx = x, fy = y, fz = z;
            if (axis > 0)
            {
                if (ai == 0) fx += size;
                else if (ai == 1) fy += size;
                else fz += size;
            }

            quads.Add(new Quad
            {
                Position = new Int3(fx, fy, fz),
                Size = size,
                Axis = axis
            });
        }
    }
    
    public byte[] Serialize()
    {
        return _tree.Serialize();
    }
    
    public static VoxelModel Deserialize(ReadOnlySpan<byte> data)
    {
        var tree = FlatOctree.Deserialize(data);
        var model = new VoxelModel(tree.Depth, tree.NodeCount);
        model._tree.Dispose();
        model._tree = tree;

        return model;
    }

}
