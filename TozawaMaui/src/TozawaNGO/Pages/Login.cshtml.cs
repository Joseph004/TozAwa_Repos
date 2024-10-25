using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TozawaNGO.Pages
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        public async Task<IActionResult> OnGetAsync(string returnUrl)
        {
            await Task.FromResult(1);
            return returnUrl == "homePage" ? LocalRedirect("/") : LocalRedirect("/" + returnUrl);
        }
    }
}
