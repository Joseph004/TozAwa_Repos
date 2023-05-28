using System.Net;
using Tozawa.Client.Portal.Services;

namespace Tozawa.Client.Portal.Models.ResponseRequests;

public class AddResponse<TType> : IResponse where TType : class
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

public class AddResponse : IResponse
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
