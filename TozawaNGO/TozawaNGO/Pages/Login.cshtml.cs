using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TozawaNGO.Auth.Services;

namespace TozawaNGO.Pages
{
    [AllowAnonymous]
    public class LoginModel(IDataProtectionProviderService dataProtectionProviderService) : PageModel
    {
        public async Task<IActionResult> OnGetAsync(string returnUrl)
        {
            await Task.FromResult(1);
            return LocalRedirect("/" + returnUrl);
        }
    }
}
