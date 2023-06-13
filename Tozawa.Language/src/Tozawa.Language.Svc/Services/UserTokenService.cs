
using System.Security.Claims;
using System.Text.Json;
using Tozawa.Language.Svc.Configuration;
using Tozawa.Language.Svc.Models.Dtos;

namespace Tozawa.Language.Svc.Services
{
    public interface IUserTokenService
    {
        CurrentUserDto GenerateUseFromToken(string token);
    }

    public class UserTokenService : IUserTokenService
    {
        private readonly AppSettings _appSettings;
        public UserTokenService(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public CurrentUserDto GenerateUseFromToken(string token)
        {
            var claims = ParseClaimsFromJwt(token);
            var claimValue = claims.First(x => x.Type == nameof(CurrentUserDto)).Value;

            return System.Text.Json.JsonSerializer.Deserialize<CurrentUserDto>(claimValue);
        }
        private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];

            var jsonBytes = ParseBase64WithoutPadding(payload);

            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));
            return claims;
        }
        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}