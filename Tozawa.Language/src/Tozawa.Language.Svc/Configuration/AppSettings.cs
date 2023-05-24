namespace Tozawa.Language.Svc.Configuration
{
    public class AppSettings
    {
        public string AzureImportFilesDirectory { get; set; }
        public string AzureExportFilesDirectory { get; set; }
        public string AzureStorageConnectionstring { get; set; }
        public string ContainersKeysDir { get; set; }
        public ConnectionStrings ConnectionStrings { get; set; }
        public AAD AAD { get; set; }
    }
}
