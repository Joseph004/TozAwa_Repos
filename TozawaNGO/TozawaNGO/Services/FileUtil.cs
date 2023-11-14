using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace TozawaNGO.Services;

public static class FileUtil
{
    public static ValueTask<object> SaveAs(this IJSRuntime js, string filename, string data)
        => js.InvokeAsync<object>(
            "saveAsFile",
            filename,
            data);

    public static async Task<IFormFile> GetFormFile(IBrowserFile file)
    {
        var stream = LoadBase64(Convert.ToBase64String(await file.GetBytes()));
        return ReturnFormFile(stream, file.Name, file.ContentType);
    }

    public static async Task<byte[]> GetBytes(this IBrowserFile file)
    {
        using var memoryStream = new MemoryStream();
        await file.OpenReadStream(file.Size).CopyToAsync(memoryStream);
        return memoryStream.ToArray();
    }

    public static IFormFile ReturnFormFile(MemoryStream stream, string fileName, string fileContentType)
        => new FormFile(stream, 0, stream.Length, fileName, fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = fileContentType
        };


    public static async Task<MemoryStream> GetStream(this IBrowserFile file)
    {
        var memoryStream = new MemoryStream
        {
            Position = 0
        };
        await file.OpenReadStream(file.Size).CopyToAsync(memoryStream);
        return memoryStream;
    }

    public static string GetBase64(this IFormFile file)
    {
        using var ms = new MemoryStream();
        file.CopyTo(ms);
        var fileBytes = ms.ToArray();
        return Convert.ToBase64String(fileBytes);
    }

    public static MemoryStream LoadBase64(string base64)
    {
        byte[] bytes = Convert.FromBase64String(base64);
        MemoryStream ms = new(bytes);
        return ms;
    }
}