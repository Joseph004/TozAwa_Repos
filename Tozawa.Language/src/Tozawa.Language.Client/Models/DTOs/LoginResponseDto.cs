using Tozawa.Language.Client.Models.Dtos;

namespace Tozawa.Language.Client.Models.DTOs
{
    public class LoginResponseDto
    {
        public CurrentUserDto User { get; set; } = new();
        public bool LoginSuccess { get; set; }
        public int LoginAttemptCount { get; set; } = 0;
        public string ErrorMessage { get; set; } = "";
        public Guid? ErrorMessageGuid { get; set; }
    }
}
