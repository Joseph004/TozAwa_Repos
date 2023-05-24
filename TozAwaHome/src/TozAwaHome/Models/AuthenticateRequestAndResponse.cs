using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TozAwaHome.Models
{
    public class AuthenticateRequestAndResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
