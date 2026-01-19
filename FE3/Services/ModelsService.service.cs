using FE3.Api;

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
}
