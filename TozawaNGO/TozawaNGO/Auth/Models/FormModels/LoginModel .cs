using System.ComponentModel.DataAnnotations;  
  
namespace TozawaNGO.Auth.Models.FormModels
{  
    public class LoginModel  
    {   
        public string Username { get; set; }  
        public string Password { get; set; }  
    }  
}  