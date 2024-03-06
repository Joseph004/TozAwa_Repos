namespace Grains.Configurations
{
    public class JWTSettings
    {
        public string SecurityKey { get; set; }
        public string ValidIssuer { get; set; }
        public string ValidAudience { get; set; }
        public string ExpiryInMinutes { get; set; }
        public string OnInactivity { get; set; }

        public int LogoutUserOn => Convert.ToInt32(ExpiryInMinutes) + Convert.ToInt32(OnInactivity);
    }
}
