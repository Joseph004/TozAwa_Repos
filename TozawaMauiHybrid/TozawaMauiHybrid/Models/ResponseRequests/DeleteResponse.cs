using System.Net;
using TozawaMauiHybrid.Services;

namespace TozawaMauiHybrid.Models.ResponseRequests;

public class DeleteResponse(bool success, string message, HttpStatusCode? statusCode) : IResponse
{
    public bool Success { get; set; } = success;
    public string Message { get; set; } = message;
    public HttpStatusCode? StatusCode { get; set; } = statusCode;
}
