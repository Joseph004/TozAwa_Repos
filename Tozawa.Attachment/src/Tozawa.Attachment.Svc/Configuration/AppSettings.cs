namespace Tozawa.Attachment.Svc.Configuration
{
    public class AppSettings
    {
        public ConnectionStrings ConnectionStrings { get; set; }
        public AAD AAD { get; set; }
        public string CorsOrigins { get; set; }
    }
}