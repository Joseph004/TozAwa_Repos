using System.Net;

namespace Tozawa.Language.Client.Services;

public interface IResponse
{
    public bool Success { get; set; }
    public HttpStatusCode? StatusCode { get; set; }
    public string Message { get; set; }
}