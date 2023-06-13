namespace Tozawa.Auth.Svc.Configurations
{
    public class AppSettings
    {
        public ConnectionStrings ConnectionStrings { get; set; }
        public AAD AAD { get; set; }
        public JWTSettings JWTSettings { get; set; }
        public string CorsOrigins { get; set; }
    }
}