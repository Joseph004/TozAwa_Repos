using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace TozawaNGO.Attachment.Converters;

public static class FormFileConverter 
{
    private static readonly IList<string> ConvertToPngMimeTypes = new List<string> { "image/tiff", "image/x-tiff", "image/bmp", "image/x-windows-bmp" };

    public static async Task<(byte[] byteArray, string name, string mimeType)> GetConvertedByteByteArray(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new Exception(nameof(file));
        }
        using (var ms = new MemoryStream())
        {
            await file.CopyToAsync(ms).ConfigureAwait(false);

            if (ConvertToPngMimeTypes.Contains(file.ContentType))
            {
                return (GetPngBytes(ms), GetPngName(file.FileName), "image/png");
            }
            
            return (ms.ToArray(), file.FileName, file.ContentType);
        }

    }

    public static bool IsImage(string contentType, byte[] fileBytes)
    {
        if (contentType != null)
        {
            if (contentType.Contains("image/"))
                return true;
        }

        try
        {
            using (var ms = new MemoryStream(fileBytes))
            {
                using (Image.FromStream(ms))
                {
                    return true;
                }
            }
        }
        catch (Exception)
        {
            return false;
        }   

    }
    private static string GetPngName(string fileName)
    {            
        var strings = fileName.Split('.').ToList();
        fileName = string.Join(".", strings.Take(strings.Count - 1));
        fileName += ".png";
        return fileName;
    }
 

    public static async Task<byte[]> ImageToPng(IFormFile file)
    {
        using (var ms = new MemoryStream())
        {
            await file.CopyToAsync(ms).ConfigureAwait(false);
            return GetPngBytes(ms);
        }

    }

    private static byte[] GetPngBytes(MemoryStream ms)
    {
        using (var saveStream = new MemoryStream())
        {
            var image = Image.FromStream(ms);
            image.Save(saveStream, ImageFormat.Png);
            return saveStream.ToArray();
        }
    }
}
