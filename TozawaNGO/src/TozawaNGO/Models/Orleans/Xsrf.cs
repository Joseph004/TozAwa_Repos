using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace TozawaNGO.Models
{
    [JsonObject]
    public class XsrfModel
    {
        public string XsrfToken { get; set; }
    }
}
