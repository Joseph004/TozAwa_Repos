using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TozAwa.Client.Portal.Pages
{
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnGetAsync(string returnUrl = "")
        {
            // Clear the existing external cookie
            if (string.IsNullOrEmpty(returnUrl))
            {
                returnUrl = Url.Content("/home");
            }
            else
            {
                returnUrl = Url.Content($"/{returnUrl}");
            }
            await HttpContext
                .SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            return LocalRedirect(returnUrl);
        }
    }
}
