using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TozAwaHome.Configurations
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
