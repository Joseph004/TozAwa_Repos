namespace Tozawa.Client.Portal.Configurations
{
    public class AppSettings
    {
        public AADClient AADClient { get; set; }
        public TozAwaBffApiSettings TozAwaBffApiSettings { get; set; }
        public string LoginEncryptKey { get; set; }
    }
}