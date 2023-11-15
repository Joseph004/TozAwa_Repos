using System;
using System.Collections.Generic;
using TozawaNGO.Models.Dtos;

namespace TozawaNGO.Models
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
