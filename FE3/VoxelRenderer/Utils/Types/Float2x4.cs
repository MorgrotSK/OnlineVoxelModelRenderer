namespace FE3.VoxelRenderer.Utils.Types;

public struct Float2X4
{
    public Float2 TL;
    public Float2 TR;
    public Float2 BL;
    public Float2 BR;

    public Float2 this[int index]
    {
        get => index switch
        {
            0 => TL,
            1 => TR,
            2 => BL,
            3 => BR,
            _ => throw new IndexOutOfRangeException()
        };
    }
}