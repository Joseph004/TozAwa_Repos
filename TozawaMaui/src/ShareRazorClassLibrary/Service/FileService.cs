using Microsoft.JSInterop;

namespace ShareRazorClassLibrary.Services;

public class FileService(IJSRuntime jsRuntime)
{
    private readonly IJSRuntime _jsRuntime = jsRuntime;

    public async Task Download(string name, byte[] content)
    {
        await _jsRuntime.SaveAs($"{name}", Convert.ToBase64String(content));
    }
}