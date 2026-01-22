using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FE3.VoxelRenderer.Utils.Octree;

[StructLayout(LayoutKind.Sequential)]
public struct OctreeNode
{
    public const int Leaf = -1;
    public int FirstChild;
    public byte Data;

    [MethodImpl(MethodImplOptions.AggressiveInlining)] 
    public bool IsLeaf()   => FirstChild == Leaf;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] 
    public bool IsBranch() => FirstChild >= 0;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] 
    public bool IsEmpty()  => IsLeaf() && Data == 0;
}
