namespace Tozawa.Language.Client.Configuration
{
    public class AppSettings
    {
        public AuthSettings AuthSettings { get; set; }
        public RootKey RootKey { get; set; }
        public string LoginEncryptKey { get; set; }
        public LanguageSettings LanguageSettings { get; set; }
        public AADClient AADClient { get; set; }
    }
}