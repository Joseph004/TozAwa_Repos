using System.ComponentModel.DataAnnotations;

namespace Grains.Auth.Models.FormModels
{
    public class RegisterModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

    }
}