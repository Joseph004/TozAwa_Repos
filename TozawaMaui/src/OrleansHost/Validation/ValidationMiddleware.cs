using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace OrleansHost.Validation;

public class ValidationMiddleware(RequestDelegate next)
{
    private RequestDelegate _next = next;

    public async Task Invoke(HttpContext context)
    {
        await _next.Invoke(context);
    }

}
public static class ValidationMiddlewareExtensions
{
    public static IApplicationBuilder UseValidation(this IApplicationBuilder builder)
    {

        return builder.UseMiddleware<ValidationMiddleware>();
    }
}