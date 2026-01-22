namespace FE3.VoxelRenderer.Utils;

public struct Quad
{
    public Int3 Position;
    public int SizeU;
    public int SizeV;
    public sbyte Axis; // ±1=X, ±2=Y, ±3=Z
    public ushort BlockId;  // index into BlockDictionary
    
}