namespace FE3.Api;

public sealed class WorldApi(HttpClient api)
{
    public async Task<Stream> GetChunkAsync(string worldId, int u, int v, CancellationToken ct = default)
    {
        var res = await api.GetAsync(
            $"/world/{worldId}/{u}/{v}",
            HttpCompletionOption.ResponseHeadersRead,
            ct);

        res.EnsureSuccessStatusCode();

        return await res.Content.ReadAsStreamAsync(ct);
    }
}
