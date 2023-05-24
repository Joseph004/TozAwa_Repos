using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Tozawa.Attachment.Svc.Middleware.AuthenticationOrderMiddleware
{
    public class AuthenticationOrder
    {
        private readonly RequestDelegate next;
        private readonly IEnumerable<string> _schemes;

        public AuthenticationOrder(RequestDelegate next, IEnumerable<string> schemes)
        {
            this.next = next ?? throw new ArgumentNullException(nameof(next));
            _schemes = schemes;
        }

        public async Task Invoke(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
            {
                context.Items["IsAnonymousEndpoint"] = true;
                await next(context);
                return;
            }
            foreach (var scheme in _schemes)
            {
                var result = await context.AuthenticateAsync(scheme);
                if (!result.Succeeded) continue;
                context.User = result.Principal;
                break;
            }
            await next(context);
        }
    }

    public static class AuthenticationOrderMiddelwareExtensions
    {
        public static IApplicationBuilder UseAuthenticationOrder(this IApplicationBuilder builder, IEnumerable<string> schemes)
        {
            return builder.UseMiddleware<AuthenticationOrder>(schemes);
        }
    }
}
