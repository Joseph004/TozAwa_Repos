namespace Tozawa.Auth.Svc.Configurations
{
    public class AppSettings
    {
        public ConnectionStrings ConnectionStrings { get; set; }
        public AAD AAD { get; set; }
        public string LoginEncryptKey { get; set; }
    }
}