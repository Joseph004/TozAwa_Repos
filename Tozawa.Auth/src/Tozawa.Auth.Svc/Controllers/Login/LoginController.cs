using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Tozawa.Auth.Svc.Controllers.Login
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class LoginController : ControllerBase
    {
        protected readonly IMediator _mediator;
        public LoginController(IMediator mediator)
        {
            _mediator = mediator;
        }

        private static readonly AuthenticationProperties COOKIE_EXPIRES = new AuthenticationProperties()
        {
            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
            IsPersistent = true
        };

        [HttpPost, Route("signin")]
        public async Task<ActionResult> SignInPost([FromBody] LoginCommand request) => Ok(await _mediator.Send(request));
        /* {
            var response = await _mediator.Send(request);
            if (response)
            {
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, request.Email),
                new Claim(ClaimTypes.Name,  "Joseph Luhandu"),
                new Claim(ClaimTypes.Role,  "Administrator"),
            };

                var claimsIdentity = new ClaimsIdentity(claims,
                                                        CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = COOKIE_EXPIRES;

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                              new ClaimsPrincipal(claimsIdentity),
                                              authProperties);
            }

            return this.Ok(response);
        } */

        [HttpPost, Route("signout")]
        public async Task<ActionResult> SignOutPost()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return this.Ok();
        }
    }
}


