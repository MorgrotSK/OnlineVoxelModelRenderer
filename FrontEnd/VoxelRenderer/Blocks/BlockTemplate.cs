using FrontEnd.VoxelRenderer.Utils.Types;

namespace FrontEnd.VoxelRenderer.Blocks;

public struct BlockTemplate
{
    public string Name { get; set; }
    public Float2X4 UvPattern { get; set; }
    public bool IsTransparent { get; set; }
}