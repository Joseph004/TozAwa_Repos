using System.Net;
using TozAwaHome.Services;

namespace TozAwaHome.Models.ResponseRequests;
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
