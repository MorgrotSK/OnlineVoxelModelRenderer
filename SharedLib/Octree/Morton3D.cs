using System.Runtime.CompilerServices;

namespace SharedClass.Octree;

public static class Morton3D
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong Encode(int x, int y, int z)
    {
        ulong m = 0;
        for (var i = 0; i < 6; i++)
        {
            m |= ((ulong)(x >> i) & 1) << (3 * i);
            m |= ((ulong)(y >> i) & 1) << (3 * i + 1);
            m |= ((ulong)(z >> i) & 1) << (3 * i + 2);
        }
        return m;
    }
}
