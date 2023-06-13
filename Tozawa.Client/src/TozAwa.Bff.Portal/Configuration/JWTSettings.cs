namespace Tozawa.Bff.Portal.Configuration
{
    public class JWTSettings
    {
        public string SecurityKey { get; set; }
        public string ValidIssuer { get; set; }
        public string ValidAudience { get; set; }
        public string SecurityKeyForAuth { get; set; }
        public string ValidIssuerForAuth { get; set; }
        public string ValidAudienceForAuth { get; set; }
        public string ExpiryInMinutes { get; set; }
    }
}