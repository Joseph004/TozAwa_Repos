using System.Net;
using TozawaNGO.Services;

namespace TozawaNGO.Models.ResponseRequests;

public class DeleteResponse : IResponse
{
    public DeleteResponse(bool success, string message, HttpStatusCode? statusCode)
    {
        Success = success;
        StatusCode = statusCode;
        Message = message;
    }
    public bool Success { get; set; }
    public string Message { get; set; }
    public HttpStatusCode? StatusCode { get; set; }
}
