using System.Net;

namespace Tozawa.Bff.Portal.Models.ResponseRequests;

public class AddResponse<TType> where TType : class
{
#nullable enable
    public AddResponse(bool success, string message, HttpStatusCode? statusCode, TType? entity)
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

public class AddResponse
{
#nullable enable
    public AddResponse(bool success, string message, HttpStatusCode? statusCode)
    {
        Success = success;
        StatusCode = statusCode;
        Message = message;
    }

    public bool Success { get; set; }
    public HttpStatusCode? StatusCode { get; set; }
    public string Message { get; set; }

}
