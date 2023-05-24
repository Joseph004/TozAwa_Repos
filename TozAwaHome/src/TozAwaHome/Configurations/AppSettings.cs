using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TozAwaHome.Configurations
{
    public class AppSettings
    {
        public AADClient AADClient { get; set; }
        public TozAwaBffApiSettings TozAwaBffApiSettings { get; set; }
        public string LoginEncryptKey { get; set; }
    }
}
