namespace Tozawa.Bff.Portal.Configuration
{
    public class AppSettings
    {
        public string SystemTypeId { get; set; }
        public Guid SystemTextGuid => Guid.Parse(SystemTypeId);
        public AAD AAD { get; set; }
        public AADClient AADClient { get; set; }
        public GoogleDrive GoogleDrive { get; set; }
        public LanguageSettings LanguageSettings { get; set; }
        public AuthSettings AuthSettings { get; set; }
        public AttachmentSettings AttachmentSettings { get; set; }
        public TozAwaActivitySvcApiSettings TozAwaActivitySvcApiSettings { get; set; }
    }
}
