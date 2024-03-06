using System.ComponentModel.DataAnnotations;  
  
namespace Grains.Auth.Models.FormModels
{  
    public class LoginModel  
    {   
        public string Username { get; set; }  
        public string Password { get; set; }  
    }  
}  