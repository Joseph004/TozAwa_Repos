namespace Tozawa.Bff.Portal.Configuration
{
    public class AAD
    {
        public string ResourceId { get; set; }
        public string Instance { get; set; }
        public string TenantId { get; set; }
        public string Domain { get; set; }
        public string ClientId { get; set; }
        public string CallbackPath { get; set; }
        public string Scopes { get; set; }
    }
}
