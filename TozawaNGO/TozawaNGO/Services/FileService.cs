using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace TozawaNGO.Services;

public class FileService
{
    private readonly IJSRuntime _jsRuntime;
    public FileService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }
    public async Task Download(string name, byte[] content)
    {
        await _jsRuntime.SaveAs($"{name}", Convert.ToBase64String(content));
    }
}