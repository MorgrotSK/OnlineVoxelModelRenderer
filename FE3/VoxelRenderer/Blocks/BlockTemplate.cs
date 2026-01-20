using FE3.VoxelRenderer.Utils;

namespace FE3.VoxelRenderer.Blocks;

public struct BlockTemplate
{
    public string Name { get; set; }
    public Float2x4 UvPattern { get; set; }
    public bool IsTransparent { get; set; }
}