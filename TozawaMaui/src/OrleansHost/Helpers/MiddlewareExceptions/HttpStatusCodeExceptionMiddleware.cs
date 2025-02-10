using Microsoft.AspNetCore.Http;

namespace OrleansHost.Helpers.MiddlewareExceptions;

public class HttpStatusCodeExceptionMiddleware
{
    private readonly RequestDelegate next;

    public HttpStatusCodeExceptionMiddleware(RequestDelegate next)
    {
        this.next = next ?? throw new ArgumentNullException(nameof(next));
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (HttpStatusCodeException ex)
        {
            if (context.Response.HasStarted)
            {
                throw;
            }

            context.Response.Clear();
            context.Response.StatusCode = ex.StatusCode;
            context.Response.ContentType = ex.ContentType;
            context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
            await context.Response.WriteAsync(ex.Message);
        }
        catch
        {
            if (context.Response.HasStarted)
            {
                throw;
            }
            context.Response.Clear();
            context.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
            context.Response.ContentType = @"text/plain";
            context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
            await context.Response.WriteAsync("Server Error");
        }
    }
}