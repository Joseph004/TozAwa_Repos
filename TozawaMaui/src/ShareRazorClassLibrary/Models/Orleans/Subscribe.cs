using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ShareRazorClassLibrary.Models
{
    [JsonObject]
    public class SubscribeModel
    {
        [EmailAddress, Required]
        public string Email { get; set; }
    }
}
