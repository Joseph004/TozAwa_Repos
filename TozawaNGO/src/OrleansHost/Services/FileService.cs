using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace OrleansHost.Services;

public class FileService(IJSRuntime jsRuntime)
{
    private readonly IJSRuntime _jsRuntime = jsRuntime;

    public async Task Download(string name, byte[] content)
    {
        await _jsRuntime.SaveAs($"{name}", Convert.ToBase64String(content));
    }
}