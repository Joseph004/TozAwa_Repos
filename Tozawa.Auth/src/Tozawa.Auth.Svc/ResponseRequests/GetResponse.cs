using System.Net;

namespace Tozawa.Auth.Svc.ResponseRequests;

public class GetResponse<T>
{
#nullable enable
    public GetResponse(bool success, string message, T? entity)
    {
        Success = success;
        Entity = entity;
        Message = message;
    }
    public bool Success { get; set; }
    public T? Entity { get; set; }
    public string Message { get; set; }
}
