using System.Net;

namespace Tozawa.Bff.Portal.Models.ResponseRequests;

public class GetResponse<T>
{
#nullable enable
    public GetResponse(bool success, string message, HttpStatusCode? statusCode, T? entity)
    {
        Success = success;
        StatusCode = statusCode;
        Entity = entity;
        Message = message;
    }
    public bool Success { get; set; }
    public T? Entity { get; set; }
    public string Message { get; set; }
    public HttpStatusCode? StatusCode { get; set; }
}
