using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ShareRazorClassLibrary.Models
{
    [JsonObject]
    public class XsrfModel
    {
        public string XsrfToken { get; set; }
    }
}
