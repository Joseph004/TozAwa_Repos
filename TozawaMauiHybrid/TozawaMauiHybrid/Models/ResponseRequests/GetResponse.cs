using System.Net;
using TozawaMauiHybrid.Services;

namespace TozawaMauiHybrid.Models.ResponseRequests;
#nullable enable
public class GetResponse<T>(bool success, string message, HttpStatusCode? statusCode, T? entity) : IResponse
{
    public bool Success { get; set; } = success;
    public T? Entity { get; set; } = entity;
    public string Message { get; set; } = message;
    public HttpStatusCode? StatusCode { get; set; } = statusCode;
}
