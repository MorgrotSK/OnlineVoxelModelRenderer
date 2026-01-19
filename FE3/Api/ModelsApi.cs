using System.Net.Http.Headers;

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
}