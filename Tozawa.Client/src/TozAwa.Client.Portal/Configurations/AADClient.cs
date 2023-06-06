
using System;
using System.Globalization;

namespace Tozawa.Client.Portal.Configurations
{
    public class AADClient
    {
        public string Instance { get; set; }
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public bool ValidateAuthority { get; set; }
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