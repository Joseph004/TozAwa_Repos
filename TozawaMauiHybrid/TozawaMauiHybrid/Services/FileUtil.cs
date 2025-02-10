using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace TozawaMauiHybrid.Services;

public static class FileUtil
{
    public static ValueTask<object> SaveAs(this IJSRuntime js, string filename, string data)
        => js.InvokeAsync<object>(
            "saveAsFile",
            filename,
            data);

    public static async Task<byte[]> GetBytes(this IBrowserFile file)
    {
        using var memoryStream = new MemoryStream();
        await file.OpenReadStream(file.Size).CopyToAsync(memoryStream);
        return memoryStream.ToArray();
    }


    public static async Task<MemoryStream> GetStream(this IBrowserFile file)
    {
        var memoryStream = new MemoryStream
        {
            Position = 0
        };
        await file.OpenReadStream(file.Size).CopyToAsync(memoryStream);
        return memoryStream;
    }

    public static MemoryStream LoadBase64(string base64)
    {
        byte[] bytes = Convert.FromBase64String(base64);
        MemoryStream ms = new(bytes);
        return ms;
    }
    public static byte[] ReadAllBytesFromStream(Stream instream)
    {
        if (instream is MemoryStream)
            return ((MemoryStream)instream).ToArray();

        using (var memoryStream = new MemoryStream())
        {
            instream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }
    }
    public static MemoryStream StreamFromByteArray(byte[] content)
    {
        MemoryStream stream = new(content);

        return stream;
    }
}