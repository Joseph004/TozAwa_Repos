using System.Net;

namespace Tozawa.Auth.Svc.ResponseRequests;

public class AddResponse<TType> where TType : class
{
#nullable enable
    public AddResponse(bool success, string message, TType? entity)
    {
        Success = success;
        Entity = entity;
        Message = message;
    }
    public bool Success { get; set; }
    public string Message { get; set; }
    public TType? Entity { get; set; }
}

public class AddResponse 
{
#nullable enable
    public AddResponse(bool success, string message)
    {
        Success = success;
        Message = message;
    }

    public bool Success { get; set; }
    public string Message { get; set; }

}
