using System;

namespace Tozawa.Client.Portal.AuthenticationServices
{
public class BlazorServerAuthData
{
    public string SubjectId;
    public DateTimeOffset Expiration;
    public string IdToken;
    public string AccessToken;
    public string RefreshToken;
    public DateTimeOffset RefreshAt;
}
}


