using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace SEM_Drahos.utils;

public static class ModelsHelpers
{
    private const int MaxPictureSize = 5 * 1024 * 1024;
    public static async Task<(bool ok, string? error)> ValidateModelAsync(IFormFile file)
    {
        if (file.Length < 13)
            return (false, "File too small");

        await using var stream = file.OpenReadStream();

        byte[] header = new byte[13];
        var read = await stream.ReadAsync(header, 0, header.Length);

        if (read != 13)
            return (false, "Incomplete header");

        uint magic = BitConverter.ToUInt32(header, 0);
        if (magic != 0x52544F46)
            return (false, "Invalid magic");
        
        return (true, null);
    }
    
    public static async Task<(bool ok, string? error)> ValidateModelThumbnailAsync(IFormFile thumbnail)
    {
        if (thumbnail.Length == 0)
            return (false, "Thumbnail is empty");

        // Optional: hard size limit (example: 5 MB)
        if (thumbnail.Length > MaxPictureSize)
            return (false, "Thumbnail is too large");
        
        try
        {
            await using var stream = thumbnail.OpenReadStream();

            // This fully validates the image structure
            using var image = await Image.LoadAsync(stream);

            // Optional: dimension sanity checks
            if (image.Width <= 0 || image.Height <= 0)
                return (false, "Invalid image dimensions");

            if (image.Width > 4096 || image.Height > 4096)
                return (false, "Thumbnail resolution too large");

            return (true, null);
        }
        catch (UnknownImageFormatException)
        {
            return (false, "Thumbnail is not a supported image format");
        }
        catch (Exception)
        {
            return (false, "Thumbnail image is corrupted");
        }
    }

}