using Microsoft.JSInterop;

namespace TozawaMauiHybrid.Services;

public class FileService(IJSRuntime jsRuntime)
{
    private readonly IJSRuntime _jsRuntime = jsRuntime;
    public async Task Download(string name, byte[] content)
    {
        await _jsRuntime.SaveAs($"{name}", Convert.ToBase64String(content));
    }
    public async Task ShowFile(Stream stream, string elementrId, string contentType, string title, bool isImage = false)
    {
        var strRef = new DotNetStreamReference(stream);
        await _jsRuntime.InvokeVoidAsync("SetSource", elementrId, strRef, contentType, title, isImage);
    }
}