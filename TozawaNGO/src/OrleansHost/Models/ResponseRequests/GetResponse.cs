using System.Net;
using OrleansHost.Services;

namespace OrleansHost.Models.ResponseRequests;
#nullable enable
public class GetResponse<T>(bool success, string message, HttpStatusCode? statusCode, T? entity) : IResponse
{
    public bool Success { get; set; } = success;
    public T? Entity { get; set; } = entity;
    public string Message { get; set; } = message;
    public HttpStatusCode? StatusCode { get; set; } = statusCode;
}
