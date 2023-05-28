using System.Net;

namespace Tozawa.Bff.Portal.Models.ResponseRequests;
public class UpdateResponse
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

public class UpdateResponse<TType> where TType : class
{
    public UpdateResponse(bool success, string message = "", TType? entity = null)
    {
        Success = success;
        Message = message;
        Entity = entity;
    }
    public bool Success { get; set; }
    public string Message { get; set; } = "";
    public TType? Entity { get; set; }
}

