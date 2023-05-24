using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace TozAwa.Client.Portal.Pages
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        public string ReturnUrl { get; set; }
        public async Task<IActionResult>
            OnGetAsync(string paramUserName, string paramUserId, string paramUseremail = "", string paramRootUser = "", string returnUrl = "")
        {

            try
            {
                // Clear the existing external cookie
                await HttpContext
                    .SignOutAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme);
            }
            catch { }
            // *** !!! This is where you would validate the user !!! ***
            // In this example we just log the user in
            // (Always log the user in for this demo)
            var claims = new List<Claim>
            {
                new Claim("root-user", string.IsNullOrEmpty(paramRootUser) ? "" : paramRootUser),
                new Claim(ClaimTypes.Email, string.IsNullOrEmpty(paramUseremail) ? "" : paramUseremail),
                new Claim(ClaimTypes.Name, paramUserName),
                new Claim(ClaimTypes.NameIdentifier, paramUserId),
            };
            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);
            /* var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                RedirectUri = this.Request.Host.Value
            }; */
            try
            {
                /*  await HttpContext.SignInAsync(
                 CookieAuthenticationDefaults.AuthenticationScheme,
                 new ClaimsPrincipal(claimsIdentity),
                 authProperties); */
                await HttpContext.SignInAsync(
               CookieAuthenticationDefaults.AuthenticationScheme,
               new ClaimsPrincipal(claimsIdentity));
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }

            if (string.IsNullOrEmpty(returnUrl))
            {
                returnUrl = Url.Content("/home");
            }
            else
            {
                returnUrl = Url.Content($"/{returnUrl}");
            }

            return LocalRedirect(returnUrl);
        }
    }
}
