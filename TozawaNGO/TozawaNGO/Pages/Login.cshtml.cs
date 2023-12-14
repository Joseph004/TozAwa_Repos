using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TozawaNGO.Auth.Services;

namespace TozawaNGO.Pages
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        public LoginModel(IDataProtectionProviderService dataProtectionProviderService)
        {
        }
        public async Task<IActionResult> OnGetAsync(string returnUrl)
        {
            return LocalRedirect("/" + returnUrl);
        }
    }
}
