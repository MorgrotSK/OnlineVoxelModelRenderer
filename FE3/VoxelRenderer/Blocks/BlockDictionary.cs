using FE3.VoxelRenderer.Utils;
using FE3.VoxelRenderer.Utils.Types;

namespace FE3.VoxelRenderer.Blocks;

public static class BlockDictionary
{
    private static readonly BlockTemplate[] BlockTypes;
    private const float TileSize = 1.0f / 16.0f;

    static BlockDictionary()
    {
        BlockTypes = new BlockTemplate[5];

        BlockTypes[0] = new BlockTemplate
        {
            Name = "Wood",
            IsTransparent = false,
            UvPattern = CalculateUvPattern(0, 0)
        };

        BlockTypes[1] = new BlockTemplate
        {
            Name = "Leaves",
            IsTransparent = true,
            UvPattern = CalculateUvPattern(1, 0)
        };
        
        BlockTypes[2] = new BlockTemplate
        {
            Name = "Stone",
            IsTransparent = false,
            UvPattern = CalculateUvPattern(2, 0)
        };
        
        BlockTypes[3] = new BlockTemplate
        {
            Name = "Grass",
            IsTransparent = false,
            UvPattern = CalculateUvPattern(3, 0)
        };
        BlockTypes[4] = new BlockTemplate
        {
            Name = "Dirt",
            IsTransparent = false,
            UvPattern = CalculateUvPattern(4, 0)
        };
        
    }

    public static ref readonly BlockTemplate Get(int id) => ref BlockTypes[id];
    
    private static Float2X4 CalculateUvPattern(int tileX, int tileY)
    {
        float u0 = tileX * TileSize;
        float u1 = u0 + TileSize;

        float v0 = tileY * TileSize;
        float v1 = v0 + TileSize;

        return new Float2X4
        {
            TL = new Float2(u0, v0),
            TR = new Float2(u1, v0),
            BL = new Float2(u0, v1),
            BR = new Float2(u1, v1)
        };
    }
}

