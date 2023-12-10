using System.Net;
using TozawaNGO.Services;

namespace TozawaNGO.Models.ResponseRequests;

public class UpdateResponse<TType> : IResponse where TType : class
{
#nullable enable
    public UpdateResponse(bool success, string message, HttpStatusCode? statusCode, TType? entity)
    {
        Success = success;
        StatusCode = statusCode;
        Entity = entity;
        Message = message;
    }
    public bool Success { get; set; }
    public string Message { get; set; }
    public HttpStatusCode? StatusCode { get; set; }
    public TType? Entity { get; set; }
}
public class UpdateResponse : IResponse
{
#nullable enable
    public UpdateResponse(bool success, string message, HttpStatusCode? statusCode)
    {
        Success = success;
        StatusCode = statusCode;
        Message = message;
    }

    public bool Success { get; set; }
    public HttpStatusCode? StatusCode { get; set; }
    public object? Entity { get; set; } = null;
    public string Message { get; set; }
}
