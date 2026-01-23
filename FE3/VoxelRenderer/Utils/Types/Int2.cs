namespace FE3.VoxelRenderer.Utils.Types;

public readonly struct Int2(int x, int y) : IEquatable<Int2>
{
    public readonly int X = x;
    public readonly int Y = y;

    public bool Equals(Int2 other) => X == other.X && Y == other.Y;
    public override bool Equals(object? obj) => obj is Int2 o && Equals(o);
    public override int GetHashCode() => HashCode.Combine(X, Y);

    public override string ToString() => $"({X},{Y})";
}
