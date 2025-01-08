using System.Net;
using Newtonsoft.Json.Linq;

namespace OrleansHost.Helpers.MiddlewareExceptions;

public class HttpStatusCodeException : Exception
{
    public int StatusCode { get; set; }
    public string ContentType { get; set; } = @"text/plain";

    public HttpStatusCodeException(int statusCode)
    {
        StatusCode = statusCode;
    }

    public HttpStatusCodeException(int statusCode, string message) : base(message)
    {
        StatusCode = statusCode;
    }

    public HttpStatusCodeException(int statusCode, Exception inner) : this(statusCode, inner.ToString()) { }

    public HttpStatusCodeException(int statusCode, JObject errorObject) : this(statusCode, errorObject.ToString())
    {
        ContentType = @"application/json";
    }
}

public class BadRequestStatusCodeException : HttpStatusCodeException
{
    public BadRequestStatusCodeException() : base((int)HttpStatusCode.BadRequest)
    {
    }

    public BadRequestStatusCodeException(string message) : base((int)HttpStatusCode.BadRequest, message)
    {
    }

    public BadRequestStatusCodeException(Exception inner) : base((int)HttpStatusCode.BadRequest, inner)
    {
    }

    public BadRequestStatusCodeException(JObject errorObject) : base((int)HttpStatusCode.BadRequest, errorObject)
    {
    }
}
public class NotFoundStatusCodeException : HttpStatusCodeException
{
    public NotFoundStatusCodeException() : base((int)HttpStatusCode.NotFound)
    {
    }

    public NotFoundStatusCodeException(string message) : base((int)HttpStatusCode.NotFound, message)
    {
    }

    public NotFoundStatusCodeException(Exception inner) : base((int)HttpStatusCode.NotFound, inner)
    {
    }

    public NotFoundStatusCodeException(JObject errorObject) : base((int)HttpStatusCode.NotFound, errorObject)
    {
    }
}

public class UnauthorizedStatusCodeException : HttpStatusCodeException
{
    public UnauthorizedStatusCodeException() : base((int)HttpStatusCode.Unauthorized)
    {
    }

    public UnauthorizedStatusCodeException(string message) : base((int)HttpStatusCode.Unauthorized, message)
    {
    }

    public UnauthorizedStatusCodeException(Exception inner) : base((int)HttpStatusCode.Unauthorized, inner)
    {
    }

    public UnauthorizedStatusCodeException(JObject errorObject) : base((int)HttpStatusCode.Unauthorized, errorObject)
    {
    }
}
public class ForbiddenStatusCodeException : HttpStatusCodeException
{
    public ForbiddenStatusCodeException() : base((int)HttpStatusCode.Forbidden)
    {
    }

    public ForbiddenStatusCodeException(string message) : base((int)HttpStatusCode.Forbidden, message)
    {
    }

    public ForbiddenStatusCodeException(Exception inner) : base((int)HttpStatusCode.Forbidden, inner)
    {
    }

    public ForbiddenStatusCodeException(JObject errorObject) : base((int)HttpStatusCode.Forbidden, errorObject)
    {
    }
}