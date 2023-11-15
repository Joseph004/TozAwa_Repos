namespace TozawaNGO.Configurations
{
    public class JWTSettings
    {
        public string SecurityKey { get; set; }
        public string ValidIssuer { get; set; }
        public string ValidAudience { get; set; }
        public string ExpiryInMinutes { get; set; }
    }
}
