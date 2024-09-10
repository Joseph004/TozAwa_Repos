using Microsoft.AspNetCore.Http;

namespace OrleansHost.Helpers;

public class ErrorWrappingMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate next = next;

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next.Invoke(context);
        }
        catch (Exception ex)
        {
            ex.Message.ToString();
            context.Response.StatusCode = 500;
            //await context.Response.WriteAsync(...); // change you response body if needed
        }
    }
}