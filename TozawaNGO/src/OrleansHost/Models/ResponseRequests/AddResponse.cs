using System.Net;
using OrleansHost.Services;

namespace OrleansHost.Models.ResponseRequests;
#nullable enable
public class AddResponse<TType>(bool success, string message, HttpStatusCode? statusCode, TType? entity) : IResponse where TType : class
{
    public bool Success { get; set; } = success;
    public string Message { get; set; } = message;
    public HttpStatusCode? StatusCode { get; set; } = statusCode;
    public TType? Entity { get; set; } = entity;
}

public class AddResponse(bool success, string message, HttpStatusCode? statusCode) : IResponse
{
    public bool Success { get; set; } = success;
    public HttpStatusCode? StatusCode { get; set; } = statusCode;
    public string Message { get; set; } = message;

}
