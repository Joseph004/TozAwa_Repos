using System.Net;

namespace Tozawa.Auth.Svc.ResponseRequests;

public class DeleteResponse
{
    public DeleteResponse(bool success, string message)
    {
        Success = success;
        Message = message;
    }
    public bool Success { get; set; }
    public string Message { get; set; }
}
