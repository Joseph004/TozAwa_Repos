using Microsoft.AspNetCore.Builder;

namespace OrleansHost.Helpers.MiddlewareExceptions;

public static class HttpStatusCodeExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseHttpStatusCodeExceptionMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<HttpStatusCodeExceptionMiddleware>();
    }
}

