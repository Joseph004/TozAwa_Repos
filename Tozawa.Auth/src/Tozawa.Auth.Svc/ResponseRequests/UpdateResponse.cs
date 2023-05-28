using System.Net;

namespace Tozawa.Auth.Svc.ResponseRequests;
public class UpdateResponse
{
#nullable enable
    public UpdateResponse(bool success, string message)
    {
        Success = success;
        Message = message;
    }

    public bool Success { get; set; }
    public object? Entity { get; set; } = null;
    public string Message { get; set; }
}
