using System.ComponentModel.DataAnnotations.Schema;

namespace TozawaNGO.Auth.Models
{
    public class TokenProvider : EventArgs
    {
        public string XsrfToken { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string ExpiresIn { get; set; }
        public string ReturnUrl { get; set; }
        public bool IsAdmin { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Id { get; set; }
    }
    public class InitialApplicationState
    {
        public string XsrfToken { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string ExpiresIn { get; set; }
    }
}