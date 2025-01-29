using System;
using ShareRazorClassLibrary.Models.Dtos;

namespace ShareRazorClassLibrary.Models
{
    public class BlazorServerAuthData
    {
        public UserDto CurrentUser { get; set; }
        public DateTimeOffset Expiration;
        public string IdToken;
        public string AccessToken;
        public string RefreshToken;
        public DateTimeOffset RefreshAt;
    }
}
