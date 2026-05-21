using FrontEnd.VoxelRenderer.Utils.Types;

namespace FrontEnd.VoxelRenderer.WorldRenderer;

public readonly struct RenderTask
{
    public Int2 Chunk { get; }
    public CancellationTokenSource Cts { get; }
    public CancellationToken Token => Cts.Token;

    public RenderTask(Int2 chunk, CancellationTokenSource cts)
    {
        Chunk = chunk;
        Cts = cts;
    }
}