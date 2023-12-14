using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TozawaNGO.Pages
{
    [AllowAnonymous]
    public class LogoutModel : PageModel
    {
        public LogoutModel()
        {
        }
        public async Task<IActionResult> OnGetAsync(string returnUrl)
        {
            return LocalRedirect("/" + returnUrl);
        }
    }
}
