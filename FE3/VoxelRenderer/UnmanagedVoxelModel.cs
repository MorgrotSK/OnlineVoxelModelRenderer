using FE3.VoxelRenderer.Utils;

namespace FE3.VoxelRenderer;
using Utils.Octree;
using Utils.UnmanagedStructures;

public sealed class UnmanagedVoxelModel(int depth, int initialCapacity) : IDisposable
{
    private FlatOctree _tree = new(depth, initialCapacity);

    public void Dispose()
    {
        _tree.Dispose();
    }

    public void SetVoxel(int x, int y, int z, byte value)
    {
        _tree.Insert(x, y, z, value);
    }

    public void ExtractQuadsByAxis(out UnmanagedList<Quad> xQuads, out UnmanagedList<Quad> yQuads, out UnmanagedList<Quad> zQuads)
    {
        var lx = new UnmanagedList<Quad>(256);
        var ly = new UnmanagedList<Quad>(256);
        var lz = new UnmanagedList<Quad>(256);

        var self = this;

        _tree.TraverseLeaves((x, y, z, size, data) =>
        {
            self.EmitLeafQuadsByAxis(x, y, z, size, data, lx, ly, lz);
        });
        
        xQuads = lx;
        yQuads = ly;
        zQuads = lz;
    }

    private void EmitLeafQuadsByAxis(int x, int y, int z, int size, byte data,
        UnmanagedList<Quad> xQuads,
        UnmanagedList<Quad> yQuads,
        UnmanagedList<Quad> zQuads)
    {
        for (int face = 0; face < 6; face++)
        {
            sbyte axis = (sbyte)((face >> 1) + 1);
            if ((face & 1) != 0) axis = (sbyte)-axis;

            if (IsFaceFullyOccludedBySameData(x, y, z, size, data, axis))
                continue;

            int fx = x, fy = y, fz = z;
            int a = axis > 0 ? axis : -axis;

            if (axis > 0)
            {
                if (a == 1) fx += size;
                else if (a == 2) fy += size;
                else fz += size;
            }

            var q = new Quad
            {
                Position = new Int3(fx, fy, fz),
                SizeU = size,
                SizeV = size,
                Axis = axis,
                BlockId = data
            };

            if (a == 1) xQuads.Add(q);
            else if (a == 2) yQuads.Add(q);
            else zQuads.Add(q);
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

    public byte[] Serialize() => _tree.Serialize();

    public static UnmanagedVoxelModel Deserialize(ReadOnlySpan<byte> data)
    {
        var tree = FlatOctree.Deserialize(data);
        return new UnmanagedVoxelModel(1, 1)
        {
            _tree = tree
        };
    }
}
