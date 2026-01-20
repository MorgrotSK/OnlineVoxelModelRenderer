using FE3.VoxelRenderer.Utils;
namespace FE3.VoxelRenderer.Blocks;

public static class BlockDictionary
{
    private static readonly BlockTemplate[] BlockTypes;
    public static int Length => BlockTypes.Length;
    private const float TileSize = 1.0f / 16.0f;

    static BlockDictionary()
    {
        BlockTypes = new BlockTemplate[2];

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
    }

    public static ref readonly BlockTemplate Get(int id) => ref BlockTypes[id];
    
    private static Float2x4 CalculateUvPattern(int tileX, int tileY)
    {
        float u0 = tileX * TileSize;
        float u1 = u0 + TileSize;

        float v0 = tileY * TileSize;
        float v1 = v0 + TileSize;

        return new Float2x4
        {
            TL = new Float2(u0, v0),
            TR = new Float2(u1, v0),
            BL = new Float2(u0, v1),
            BR = new Float2(u1, v1)
        };
    }
}

