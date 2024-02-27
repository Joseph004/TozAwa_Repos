namespace TozawaNGO.Configurations
{
    public class SiloSettings
    {
        public string FileProcessingInterval { get; set; }
        public string LargeStateStorageConnectionString { get; set; }
        public string LargeStateStorageName { get; set; }
        public string StateStorageConnectionString { get; set; }
        public string StateStorageName { get; set; }
    }
}