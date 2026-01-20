using FE3.Api;
using FE3.Api.Types;

public sealed class ModelsService(ModelsApi api)
{
    public async Task UploadAsync(byte[] modelBytes, byte[] thumbnailBytes, string name, CancellationToken ct = default) {
        var res = await api.UploadAsync(modelBytes, thumbnailBytes, name, ct);

        if (!res.IsSuccessStatusCode)
        {
            var body = await res.Content.ReadAsStringAsync(ct);
            throw new InvalidOperationException($"Upload failed ({(int)res.StatusCode}): {body}");
        }
    }
    
    public Task<IReadOnlyList<ModelItem>> GetPublicModelsAsync(CancellationToken ct = default) => api.GetPublicModelsAsync(ct);
    public Task<Stream> GetThumbnailStreamAsync(string modelId, CancellationToken ct = default) => api.GetThumbnailStreamAsync(modelId, ct);
    public Task<ModelItem> GetModelMetaAsync(string modelId, CancellationToken ct = default) => api.GetModelMetaAsync(modelId, ct);
    public Task<Stream> GetModelStreamAsync(string modelId, CancellationToken ct = default) => api.GetModelStreamAsync(modelId, ct);
}
