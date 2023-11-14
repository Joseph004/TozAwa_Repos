using System.Net;
using TozawaNGO.Services;

namespace TozawaNGO.Models.ResponseRequests;

public class GetResponse<T> : IResponse
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
