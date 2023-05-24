using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TozAwaHome.Models
{
    internal class Setting
    {
        public static UserBasicDetail UserBasicDetail { get; set; }
        public const string BaseUrl = "https://localhost:7277";
		public const string MobileDeviceUrl = "https://10.0.2.2:7277";
	}
}
