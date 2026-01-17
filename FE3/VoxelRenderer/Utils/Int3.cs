namespace FE3.VoxelRenderer.Utils;

public readonly struct Int3(int x, int y, int z)
{
    public readonly int X = x, Y = y, Z = z;

    public int this[int i]
        => i == 0 ? X : (i == 1 ? Y : Z);
}
