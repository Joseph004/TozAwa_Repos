using System.Net;

namespace TozawaMauiHybrid.Services;

public interface IResponse
{
    public bool Success { get; set; }
    public HttpStatusCode? StatusCode { get; set; }
    public string Message { get; set; }
}