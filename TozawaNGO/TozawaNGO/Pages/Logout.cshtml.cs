using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TozawaNGO.Pages
{
    [Authorize]
    public class LogoutModel : PageModel
    {
        public LogoutModel()
        {
        }
        public async Task<IActionResult> OnGetAsync(string returnUrl)
        {
            // Clear the existing external cookie
            await HttpContext
                .SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            return LocalRedirect("/" + returnUrl);
        }
    }
}
