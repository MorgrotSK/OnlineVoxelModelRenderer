using System.Numerics;
using Ab4d.SharpEngine.Common;
using Ab4d.SharpEngine.Meshes;
using FE3.VoxelRenderer.Blocks;
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
            Console.WriteLine(q.BlockId);
            ref readonly var block = ref BlockDictionary.Get(q.BlockId - 1);
            Float2x4 uv = block.UvPattern;

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

        if (IsFaceFullyOccludedBySameData(x, y, z, size, data, axis))
            continue;

        int fx = x, fy = y, fz = z;
        int a = axis > 0 ? axis : -axis;

        // face plane position (matches your BuildMesh expectations)
        if (axis > 0)
        {
            if (a == 1) fx += size;
            else if (a == 2) fy += size;
            else fz += size;
        }

        quads.Add(new Quad
        {
            Position = new Int3(fx, fy, fz),
            Size = size,
            Axis = axis,
            BlockId = data
        });
    }
}

private bool IsFaceFullyOccludedBySameData(int x, int y, int z, int size, byte data, sbyte axis)
{
    int a = axis > 0 ? axis : -axis;
    
    int nx = x, ny = y, nz = z;

    if (a == 1) nx = axis > 0 ? x + size : x - 1;
    if (a == 2) ny = axis > 0 ? y + size : y - 1;
    if (a == 3) nz = axis > 0 ? z + size : z - 1;

    if (a == 1)
    {
        for (int yy = y; yy < y + size; yy++)
            for (int zz = z; zz < z + size; zz++)
                if (_tree.Get(nx, yy, zz) != data)
                    return false;
    }
    else if (a == 2)
    {
        for (int xx = x; xx < x + size; xx++)
            for (int zz = z; zz < z + size; zz++)
                if (_tree.Get(xx, ny, zz) != data)
                    return false;
    }
    else
    {
        for (int xx = x; xx < x + size; xx++)
            for (int yy = y; yy < y + size; yy++)
                if (_tree.Get(xx, yy, nz) != data)
                    return false;
    }

    return true;
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
