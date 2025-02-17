
using System.Globalization;

namespace Tozawa.Language.Client.Configuration
{
    public class AADClient
    {
        public string Instance { get; set; }
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Authority
        {
            get
            {
                return String.Format(CultureInfo.InvariantCulture,
                                     Instance, TenantId);
            }
        }
    }
}