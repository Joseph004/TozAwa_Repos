using System.Security.Claims;

namespace TozawaMauiHybrid.Models.Dtos;

public class UserLoginStateDto
{
    public bool IsAuthenticated { get; private set; }
    public string JWTToken { get; private set; }
    public string JWTRefreshToken { get; private set; }
    public Guid WorkOrganizationId { get; private set; } = Guid.Empty;
    public List<Claim> UserClaims { get; private set; }

    public void Set(bool isAuthenticated, string jWTToken, string jWTRefreshToken, Guid workOrganizationId, List<Claim> claims = null)
    {
        IsAuthenticated = isAuthenticated;
        JWTToken = jWTToken;
        JWTRefreshToken = jWTRefreshToken;
        UserClaims = claims;
        WorkOrganizationId = workOrganizationId;
    }
}