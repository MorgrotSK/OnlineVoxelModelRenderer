using System.Net.Http.Headers;
using System.Text.Json;
using FE3.Api.Types;

namespace FE3.Api;

public sealed class ModelsApi(HttpClient api)
{
    public async Task<HttpResponseMessage> UploadAsync(byte[] modelBytes, byte[] thumbnailBytes, string name, CancellationToken ct = default)
    {
        using var form = new MultipartFormDataContent();

        form.Add(new ByteArrayContent(modelBytes)
        {
            Headers = { ContentType = new MediaTypeHeaderValue("application/octet-stream") }
        }, "voxelModel", "model.fotr");

        form.Add(new ByteArrayContent(thumbnailBytes)
        {
            Headers = { ContentType = new MediaTypeHeaderValue("image/png") }
        }, "thumbnail", "thumbnail.png");

        form.Add(new StringContent(name), "name");

        return await api.PostAsync("/models/upload", form, ct);
    }
    
    public async Task SetAccessAsync(string modelId, bool isPrivate, CancellationToken ct = default)
    {
        using var res = await api.PatchAsync(
            $"/models/{modelId}/access?access={isPrivate.ToString().ToLowerInvariant()}",
            content: null,
            ct);
        
        res.EnsureSuccessStatusCode();
    }
    
    public async Task RemoveModelAsync(string modelId, CancellationToken ct = default)
    {
        using var res = await api.DeleteAsync($"/models/{modelId}", ct);

        res.EnsureSuccessStatusCode();
    }
    
    public async Task<Stream> GetModelStreamAsync(string modelId, CancellationToken ct = default)
    {
        var res = await api.GetAsync($"/models/{modelId}", HttpCompletionOption.ResponseHeadersRead, ct);

        res.EnsureSuccessStatusCode();

        return await res.Content.ReadAsStreamAsync(ct);
    }
    
    public async Task<ModelItem> GetModelMetaAsync(
        string modelId, CancellationToken ct = default)
    {
        using var res = await api.GetAsync($"/models/{modelId}/meta", ct);
        res.EnsureSuccessStatusCode();

        await using var stream = await res.Content.ReadAsStreamAsync(ct);

        return await JsonSerializer.DeserializeAsync<ModelItem>(stream, JsonOptions, ct) ?? throw new InvalidOperationException("Invalid model metadata response.");
    }
    
    public async Task<IReadOnlyList<ModelItem>> GetPublicModelsAsync(CancellationToken ct = default)
    {
        using var res = await api.GetAsync("/models", ct);
        res.EnsureSuccessStatusCode();

        await using var stream = await res.Content.ReadAsStreamAsync(ct);

        return await JsonSerializer.DeserializeAsync<IReadOnlyList<ModelItem>>(stream, JsonOptions, ct) ?? [];
    }
    
    public async Task<Stream> GetThumbnailStreamAsync(string modelId, CancellationToken ct = default) {
        var res = await api.GetAsync(
            $"/models/{modelId}/thumbnail",
            HttpCompletionOption.ResponseHeadersRead,
            ct);

        res.EnsureSuccessStatusCode();

        return await res.Content.ReadAsStreamAsync(ct);
    }
    
    
    private static readonly JsonSerializerOptions JsonOptions =
        new()
        {
            PropertyNameCaseInsensitive = true
        };

}