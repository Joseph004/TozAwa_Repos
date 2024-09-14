using System.Net;
using ShareRazorClassLibrary.Services;

namespace ShareRazorClassLibrary.Models.ResponseRequests;

public class DeleteResponse(bool success, string message, HttpStatusCode? statusCode) : IResponse
{
    public bool Success { get; set; } = success;
    public string Message { get; set; } = message;
    public HttpStatusCode? StatusCode { get; set; } = statusCode;
}
