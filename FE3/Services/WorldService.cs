namespace Ab4d.SharpEngine.WebGL.Services;

using FE3.Api;

public sealed class WorldService(WorldApi api)
{
    public Task<Stream> GetChunkAsync(string worldId, int u, int v, CancellationToken ct = default) => api.GetChunkAsync(worldId, u, v, ct);
}
