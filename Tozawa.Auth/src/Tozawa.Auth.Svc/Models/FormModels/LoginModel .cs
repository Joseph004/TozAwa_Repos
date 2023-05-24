using System.ComponentModel.DataAnnotations;  
  
namespace Tozawa.Auth.Svc.Models.FormModels
{  
    public class LoginModel  
    {   
        public string Username { get; set; }  
        public string Password { get; set; }  
    }  
}  