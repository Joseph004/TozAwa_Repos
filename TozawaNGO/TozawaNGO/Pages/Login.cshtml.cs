using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json;
using TozawaNGO.Auth.Models;
using TozawaNGO.Auth.Models.Dtos;
using TozawaNGO.Auth.Services;
using TozawaNGO.Features;

namespace TozawaNGO.Pages
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly IDataProtectionProviderService _dataProtectionProviderService;
        public LoginModel(IDataProtectionProviderService dataProtectionProviderService)
        {
            _dataProtectionProviderService = dataProtectionProviderService;
        }
        public async Task<IActionResult> OnGetAsync(string data)
        {
            try
            {
                // Clear the existing external cookie
                await HttpContext
                    .SignOutAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme);
            }
            catch { }

            var decryptToken = _dataProtectionProviderService.DecryptString("vtLJA1vT^qwrqhgtrdfvcj7_", data.Replace("_", "/"));
            var tokenProvider = JsonConvert.DeserializeObject<TokenProvider>(decryptToken);

            var refreshAt = DateTimeOffset.UtcNow.AddSeconds(Convert.ToDouble(tokenProvider.ExpiresIn)).ToString(CultureInfo.InvariantCulture);
            var tokens = new List<AuthenticationToken>
            {
                 new() {
                 Name = OpenIdConnectParameterNames.AccessToken,
                 Value = tokenProvider.Token
                       },
       new() {
         Name = OpenIdConnectParameterNames.RefreshToken,
         Value = tokenProvider.RefreshToken
       },
       new() {
         Name = OpenIdConnectParameterNames.ExpiresIn,
         Value = tokenProvider.ExpiresIn
       }
       };
            var claimsIdentity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            var claims = new List<Claim>
            {
                new(nameof(CurrentUserDto), await GetUserTokenWhenUserIsAuthenticate(tokenProvider.Token)),
                new("authToken", tokenProvider.Token),
                new("refreshToken", tokenProvider.RefreshToken),
                new(ClaimTypes.NameIdentifier, tokenProvider.Id),
                new("admin-member", tokenProvider.IsAdmin ? "MemberIsAdmin" : ""),
                new(ClaimTypes.Name, tokenProvider.UserName),
                new(ClaimTypes.Email, tokenProvider.Email),
                new("refresh_at", refreshAt),
                new("exp", tokenProvider.ExpiresIn)
            };
            claimsIdentity.AddClaims(claims);
            try
            {
                var authProperties = new AuthenticationProperties
                { };
                authProperties.StoreTokens(tokens);

                await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }

            return LocalRedirect(tokenProvider.ReturnUrl);
        }
        private async Task<string> GetUserTokenWhenUserIsAuthenticate(string token)
        {
            var authUser = await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), "jwtAuthType"))));
            if (!authUser.User.Claims.Any()) return string.Empty;

            return authUser.User.Claims.Where(x => x.Type == nameof(CurrentUserDto)).Select(c => c.Value).SingleOrDefault();
        }
    }
}
