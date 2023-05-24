using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Tozawa.Attachment.Svc.Validation
{
    public class ValidationMiddleware
    {
        private RequestDelegate next;

        public ValidationMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await next.Invoke(context);
        }

    }
    public static class ValidationMiddlewareExtensions
    {
        public static IApplicationBuilder UseValidation(this IApplicationBuilder builder)
        {

            return builder.UseMiddleware<ValidationMiddleware>();
        }
    }

}
