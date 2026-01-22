using FE3.VoxelRenderer.Utils.UnmanagedStructures;

namespace FE3.VoxelRenderer.Utils.Octree;

using System;
using System.Runtime.CompilerServices;

public struct FlatOctree : IDisposable
{
    private readonly int _depth;
    private UnmanagedArray<OctreeNode> _nodes;
    private int _root;
    
    public int Depth => _depth;
    public int NodeCount => _nodes.Count;

    public FlatOctree(int depth, int cap = 128)
    {
        _depth = depth;
        _nodes = new UnmanagedArray<OctreeNode>(cap);
        _root = _nodes.Allocate(1);
        _nodes[_root] = new OctreeNode { FirstChild = OctreeNode.Leaf, Data = 0 };
    }

    public void Dispose() => _nodes.Dispose();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool InBounds(int x, int y, int z) => (uint)x < 64 && (uint)y < 64 && (uint)z < 64;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte Get(int x, int y, int z)
    {
        if (!InBounds(x, y, z)) return 0;
        ulong m = Morton3D.Encode(x, y, z);
        int n = _root;

        for (int l = 0; l < _depth; l++)
        {
            ref var node = ref _nodes[n];
            if (node.IsEmpty()) return 0;
            if (node.IsLeaf()) return node.Data;
            int c = (int)((m >> (3 * (_depth - 1 - l))) & 7);
            n = node.FirstChild + c;
        }
        return _nodes[n].IsLeaf() ? _nodes[n].Data : (byte)0;
    }
    
    public void Insert(int x, int y, int z, byte data)
    {
        if (!InBounds(x, y, z)) return;
        ulong m = Morton3D.Encode(x, y, z);
        Span<int> path = stackalloc int[_depth + 1];
        int n = _root, p = 0;
        path[p++] = n;

        for (int l = 0; l < _depth; l++)
        {
            ref var node = ref _nodes[n];
            if (l == _depth - 1)
            {
                ref var leaf = ref _nodes[n];
                leaf.FirstChild = OctreeNode.Leaf;
                leaf.Data = data;
                break;
            }

            if (node.IsLeaf())
            {
                byte old = node.Data;
                int fc = _nodes.Allocate(8);
                node.FirstChild = fc;
                for (int i = 0; i < 8; i++)
                    _nodes[fc + i] = new OctreeNode { FirstChild = OctreeNode.Leaf, Data = old };
            }

            if (!node.IsBranch())
            {
                int fc = _nodes.Allocate(8);
                node.FirstChild = fc;
                for (int i = 0; i < 8; i++) _nodes[fc + i] = default;
            }

            int c = (int)((m >> (3 * (_depth - 1 - l))) & 7);
            n = node.FirstChild + c;
            path[p++] = n;
        }

        for (int i = p - 1; i > 0; i--)
        {
            ref var par = ref _nodes[path[i - 1]];
            if (!par.IsBranch()) break;
            int fc = par.FirstChild;
            byte v = _nodes[fc].Data;
            for (int c = 0; c < 8; c++)
                if (!_nodes[fc + c].IsLeaf() || _nodes[fc + c].Data != v) return;
            par.FirstChild = OctreeNode.Leaf;
            par.Data = v;
        }
    }
    
    public void TraverseLeaves(Action<int,int,int,int,byte> visitor)
    {
        TraverseNode(_root, 0, 0, 0, 1 << _depth, visitor);
    }

    private void TraverseNode(
        int nodeIdx, int x, int y, int z, int size,
        Action<int,int,int,int,byte> visitor)
    {
        ref var node = ref _nodes[nodeIdx];
        if (node.IsEmpty()) return;

        if (node.IsBranch())
        {
            int half = size >> 1;
            int fc = node.FirstChild;
            for (int i = 0; i < 8; i++)
            {
                int dx = i & 1, dy = (i >> 1) & 1, dz = (i >> 2) & 1;
                TraverseNode(
                    fc + i,
                    x + dx * half,
                    y + dy * half,
                    z + dz * half,
                    half,
                    visitor);
            }
            return;
        }

        if (node.Data != 0)
            visitor(x, y, z, size, node.Data);
    }
    
    public byte[] Serialize()
    {
        unsafe
        {
            const uint magic = 0x52544F46; // "FOTR"

            int nodeCount = _nodes.Count;
            int nodeSize = sizeof(OctreeNode);

            byte[] buffer = new byte[
                4 + 1 + 1 + 4 + 4 + nodeCount * nodeSize
            ];

            fixed (byte* dst = buffer)
            {
                *(uint*)(dst + 0)  = magic;
                *(byte*)(dst + 4)  = (byte)_depth;
                *(int*)(dst + 5)   = nodeCount;
                *(int*)(dst + 9)   = _root;

                Buffer.MemoryCopy(_nodes.RawPtr, dst + 13, nodeCount * nodeSize, nodeCount * nodeSize);
            }

            return buffer;
        }
        
    }
    
    public static FlatOctree Deserialize(ReadOnlySpan<byte> data)
    {
        const uint magic = 0x52544F46; // "FOTR"

        unsafe
        {
            fixed (byte* src = data)
            {
                // 1. Validate magic
                if (*(uint*)(src + 0) != magic)
                    throw new InvalidDataException("Invalid FlatOctree data");

                // 2. Read header
                int depth     = *(byte*)(src + 4);
                int nodeCount = *(int*)(src + 5);
                int root      = *(int*)(src + 9);

                // 3. Create tree
                var tree = new FlatOctree(depth, nodeCount);
                tree._nodes.Allocate(nodeCount - 1);
                tree._root = root;

                // 4. Copy nodes
                Buffer.MemoryCopy(
                    src + 13,
                    tree._nodes.RawPtr,
                    nodeCount * sizeof(OctreeNode),
                    nodeCount * sizeof(OctreeNode));

                return tree;
            }
        }
    }

    
}


