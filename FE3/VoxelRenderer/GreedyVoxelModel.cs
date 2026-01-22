using System.Numerics;
using System.Runtime.CompilerServices;
using Ab4d.SharpEngine.Common;
using Ab4d.SharpEngine.Core;
using Ab4d.SharpEngine.Materials;
using Ab4d.SharpEngine.Meshes;
using Ab4d.SharpEngine.SceneNodes;
using FE3.VoxelRenderer.Blocks;
using FE3.VoxelRenderer.Utils;
using FE3.VoxelRenderer.Utils.UnmanagedStructures;

namespace FE3.VoxelRenderer;

public ref struct GreedyVoxelModel
{
    private ref UnmanagedVoxelModel _model;

    private enum QuadAxis : byte { X, Y, Z }

    private struct MaskCell
    {
        public ushort BlockId;
    }

    private struct MaskRect
    {
        public int U;
        public int V;
        public int Width;
        public int Height;
        public ushort BlockId;
    }

    public GreedyVoxelModel(ref UnmanagedVoxelModel model)
    {
        _model = ref model;
    }

    public GreedyMeshData BuildMeshData()
    {
        _model.ExtractQuadsByAxis(out var xQuads, out var yQuads, out var zQuads);
        var rects = new List<Quad>(256);
        ProcessAxis(xQuads, QuadAxis.X, rects);
        ProcessAxis(yQuads, QuadAxis.Y, rects);
        ProcessAxis(zQuads, QuadAxis.Z, rects);
        xQuads.Dispose();
        yQuads.Dispose();
        zQuads.Dispose();
        
        int rectCount = rects.Count;
        var vertices = new List<PositionNormalTextureVertex>(rectCount * 4);
        var indices  = new List<int>(rectCount * 6);
         for (int i = 0; i < rectCount; i++)
        {
            var r = rects[i];

            int x = r.Position.X;
            int y = r.Position.Y;
            int z = r.Position.Z;

            int su = r.SizeU;
            int sv = r.SizeV;

            Vector3 normal;
            Span<Vector3> c = stackalloc Vector3[4];

            ref readonly var block = ref BlockDictionary.Get(r.BlockId - 1);
            Float2x4 uv = block.UvPattern;

            switch (r.Axis)
            {
                case -1:
                    normal = -Vector3.UnitX;
                    c[0] = new(x, y + sv, z);
                    c[1] = new(x, y + sv, z + su);
                    c[2] = new(x, y, z + su);
                    c[3] = new(x, y, z);
                    break;

                case 1:
                    normal = Vector3.UnitX;
                    c[0] = new(x, y, z + su);
                    c[1] = new(x, y + sv, z + su);
                    c[2] = new(x, y + sv, z);
                    c[3] = new(x, y, z);
                    break;

                case -2:
                    normal = -Vector3.UnitY;
                    c[0] = new(x, y, z);
                    c[1] = new(x, y, z + sv);
                    c[2] = new(x + su, y, z + sv);
                    c[3] = new(x + su, y, z);
                    break;

                case 2:
                    normal = Vector3.UnitY;
                    c[0] = new(x, y, z);
                    c[1] = new(x + su, y, z);
                    c[2] = new(x + su, y, z + sv);
                    c[3] = new(x, y, z + sv);
                    break;

                case -3:
                    normal = -Vector3.UnitZ;
                    c[0] = new(x + su, y, z);
                    c[1] = new(x + su, y + sv, z);
                    c[2] = new(x, y + sv, z);
                    c[3] = new(x, y, z);
                    break;

                case 3:
                    normal = Vector3.UnitZ;
                    c[0] = new(x, y + sv, z);
                    c[1] = new(x + su, y + sv, z);
                    c[2] = new(x + su, y, z);
                    c[3] = new(x, y, z);
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
                    new Vector2(uv[j].X, uv[j].Y)));
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
         
         return new GreedyMeshData(vertices.ToArray(), indices.ToArray());
    }
    

    public MeshModelNode BuildMesh(GpuImage? diffuseGpuImage)
    {
        var material = new StandardMaterial
        {
            DiffuseTexture = diffuseGpuImage
        };

        _model.ExtractQuadsByAxis(out var xQuads, out var yQuads, out var zQuads);
        
        var rects = new List<Quad>(256);

        ProcessAxis(xQuads, QuadAxis.X, rects);
        ProcessAxis(yQuads, QuadAxis.Y, rects);
        ProcessAxis(zQuads, QuadAxis.Z, rects);
        xQuads.Dispose();
        yQuads.Dispose();
        zQuads.Dispose();
        
        var meshData = BuildMeshData();
        
        return new MeshModelNode(
            new TriangleMesh<PositionNormalTextureVertex>(meshData.Vertices, meshData.Indices), 
            material);
    }
    private static void ProcessAxis(UnmanagedList<Quad> axisQuads, QuadAxis axis, List<Quad> output)
    {
        var slices = GroupQuadsIntoSlices(axisQuads, axis);

        foreach (var kv in slices)
        {
            int sliceCoord = kv.Key;
            var sliceQuads = kv.Value;

            var pos = new UnmanagedList<Quad>(8);
            var neg = new UnmanagedList<Quad>(8);

            for (int i = 0; i < sliceQuads.Count; i++)
            {
                ref var q = ref sliceQuads[i];
                if (q.Axis > 0) pos.Add(q);
                else neg.Add(q);
            }

            if (pos.Count > 0)
                ProcessSignedSlice(pos, axis, sliceCoord, pos[0].Axis, output);

            if (neg.Count > 0)
                ProcessSignedSlice(neg, axis, sliceCoord, neg[0].Axis, output);

            pos.Dispose();
            neg.Dispose();
            sliceQuads.Dispose();
        }
    }

    private static void ProcessSignedSlice(UnmanagedList<Quad> quads, QuadAxis axis, int sliceCoord, sbyte signedAxis, List<Quad> output)
    {
        var mask = BuildSliceMask(quads, axis, out int width, out int height, out int uMin, out int vMin, out int cellSize);

        var maskRects = new List<MaskRect>(16);
        ExtractGreedyRectangles(mask, width, height, maskRects);

        ProjectMaskRectsToSliceRects(maskRects, axis, sliceCoord, uMin, vMin, cellSize, signedAxis, output);
    }

    // ---------------- Mesh Build ----------------

    public TriangleMesh<PositionNormalTextureVertex> BuildGreedyMesh(List<Quad> rects)
    {
        int rectCount = rects.Count;

        var vertices = new List<PositionNormalTextureVertex>(rectCount * 4);
        var indices  = new List<int>(rectCount * 6);

        for (int i = 0; i < rectCount; i++)
        {
            var r = rects[i];

            int x = r.Position.X;
            int y = r.Position.Y;
            int z = r.Position.Z;

            int su = r.SizeU;
            int sv = r.SizeV;

            Vector3 normal;
            Span<Vector3> c = stackalloc Vector3[4];

            ref readonly var block = ref BlockDictionary.Get(r.BlockId - 1);
            Float2x4 uv = block.UvPattern;

            switch (r.Axis)
            {
                case -1:
                    normal = -Vector3.UnitX;
                    c[0] = new(x, y + sv, z);
                    c[1] = new(x, y + sv, z + su);
                    c[2] = new(x, y, z + su);
                    c[3] = new(x, y, z);
                    break;

                case 1:
                    normal = Vector3.UnitX;
                    c[0] = new(x, y, z + su);
                    c[1] = new(x, y + sv, z + su);
                    c[2] = new(x, y + sv, z);
                    c[3] = new(x, y, z);
                    break;

                case -2:
                    normal = -Vector3.UnitY;
                    c[0] = new(x, y, z);
                    c[1] = new(x, y, z + sv);
                    c[2] = new(x + su, y, z + sv);
                    c[3] = new(x + su, y, z);
                    break;

                case 2:
                    normal = Vector3.UnitY;
                    c[0] = new(x, y, z);
                    c[1] = new(x + su, y, z);
                    c[2] = new(x + su, y, z + sv);
                    c[3] = new(x, y, z + sv);
                    break;

                case -3:
                    normal = -Vector3.UnitZ;
                    c[0] = new(x + su, y, z);
                    c[1] = new(x + su, y + sv, z);
                    c[2] = new(x, y + sv, z);
                    c[3] = new(x, y, z);
                    break;

                case 3:
                    normal = Vector3.UnitZ;
                    c[0] = new(x, y + sv, z);
                    c[1] = new(x + su, y + sv, z);
                    c[2] = new(x + su, y, z);
                    c[3] = new(x, y, z);
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
                    new Vector2(uv[j].X, uv[j].Y)));
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

        return new TriangleMesh<PositionNormalTextureVertex>(
            vertices.ToArray(),
            indices.ToArray());
    }
    
    private static Dictionary<int, UnmanagedList<Quad>> GroupQuadsIntoSlices(UnmanagedList<Quad> quads, QuadAxis axis)
    {
        var slices = new Dictionary<int, UnmanagedList<Quad>>(32);

        for (int i = 0; i < quads.Count; i++)
        {
            ref var quad = ref quads[i];
            int slice = GetSliceCoord(quad, axis);

            if (!slices.TryGetValue(slice, out var list))
            {
                list = new UnmanagedList<Quad>(16);
                slices.Add(slice, list);
            }

            list.Add(quad);
        }

        return slices;
    }

    private static MaskCell[,] BuildSliceMask(
        UnmanagedList<Quad> sliceQuads,
        QuadAxis axis,
        out int width,
        out int height,
        out int uMin,
        out int vMin,
        out int cellSize)
    {
        cellSize = 2;

        int uMax = int.MinValue;
        int vMax = int.MinValue;
        uMin = int.MaxValue;
        vMin = int.MaxValue;

        for (int i = 0; i < sliceQuads.Count; i++)
        {
            ref var q = ref sliceQuads[i];
            ProjectToMaskCoords(q, axis, out int u, out int v);

            int uEnd = u + q.SizeU;
            int vEnd = v + q.SizeV;

            if (u < uMin) uMin = u;
            if (v < vMin) vMin = v;
            if (uEnd > uMax) uMax = uEnd;
            if (vEnd > vMax) vMax = vEnd;
        }

        width  = (uMax - uMin) / cellSize;
        height = (vMax - vMin) / cellSize;

        var mask = new MaskCell[width, height];

        for (int i = 0; i < sliceQuads.Count; i++)
        {
            ref var q = ref sliceQuads[i];
            ProjectToMaskCoords(q, axis, out int u0, out int v0);

            int uStart = (u0 - uMin) / cellSize;
            int vStart = (v0 - vMin) / cellSize;

            int uSpan = q.SizeU / cellSize;
            int vSpan = q.SizeV / cellSize;

            for (int du = 0; du < uSpan; du++)
            for (int dv = 0; dv < vSpan; dv++)
                mask[uStart + du, vStart + dv].BlockId = q.BlockId;
        }

        return mask;
    }

    private static void ExtractGreedyRectangles(MaskCell[,] mask, int width, int height, List<MaskRect> output)
    {
        for (int v = 0; v < height; v++)
        {
            for (int u = 0; u < width; u++)
            {
                ushort blockId = mask[u, v].BlockId;
                if (blockId == 0) continue;

                int maxWidth = 1;
                while (u + maxWidth < width && mask[u + maxWidth, v].BlockId == blockId)
                    maxWidth++;

                int maxHeight = 1;
                bool canGrow = true;

                while (v + maxHeight < height && canGrow)
                {
                    for (int x = 0; x < maxWidth; x++)
                        if (mask[u + x, v + maxHeight].BlockId != blockId)
                        {
                            canGrow = false;
                            break;
                        }
                    if (canGrow) maxHeight++;
                }

                output.Add(new MaskRect
                {
                    U = u,
                    V = v,
                    Width = maxWidth,
                    Height = maxHeight,
                    BlockId = blockId
                });

                for (int dv = 0; dv < maxHeight; dv++)
                    for (int du = 0; du < maxWidth; du++)
                        mask[u + du, v + dv].BlockId = 0;
            }
        }
    }

    private static void ProjectMaskRectsToSliceRects(
        List<MaskRect> maskRects,
        QuadAxis axis,
        int sliceCoord,
        int uMin,
        int vMin,
        int cellSize,
        sbyte signedAxis,
        List<Quad> output)
    {
        for (int i = 0; i < maskRects.Count; i++)
        {
            var r = maskRects[i];

            int u = uMin + r.U * cellSize;
            int v = vMin + r.V * cellSize;

            Int3 pos = axis switch
            {
                QuadAxis.X => new Int3(sliceCoord, v, u),
                QuadAxis.Y => new Int3(u, sliceCoord, v),
                _          => new Int3(u, v, sliceCoord)
            };

            output.Add(new Quad
            {
                Position = pos,
                SizeU = r.Width * cellSize,
                SizeV = r.Height * cellSize,
                Axis = signedAxis,
                BlockId = r.BlockId
            });
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ProjectToMaskCoords(in Quad q, QuadAxis axis, out int u, out int v)
    {
        switch (axis)
        {
            case QuadAxis.X: u = q.Position.Z; v = q.Position.Y; break;
            case QuadAxis.Y: u = q.Position.X; v = q.Position.Z; break;
            default:         u = q.Position.X; v = q.Position.Y; break;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetSliceCoord(in Quad q, QuadAxis axis)
    {
        return axis switch
        {
            QuadAxis.X => q.Position.X,
            QuadAxis.Y => q.Position.Y,
            _          => q.Position.Z
        };
    }
}
