using System.Net;
using TozawaNGO.Services;

namespace TozawaNGO.Models.ResponseRequests;

public class UpdateResponse<TType>(bool success, string message, HttpStatusCode? statusCode, TType? entity) : IResponse where TType : class
{
    public bool Success { get; set; } = success;
    public string Message { get; set; } = message;
    public HttpStatusCode? StatusCode { get; set; } = statusCode;
    public TType? Entity { get; set; } = entity;
}
public class UpdateResponse(bool success, string message, HttpStatusCode? statusCode) : IResponse
{
    public bool Success { get; set; } = success;
    public HttpStatusCode? StatusCode { get; set; } = statusCode;
    public object? Entity { get; set; } = null;
    public string Message { get; set; } = message;
}
