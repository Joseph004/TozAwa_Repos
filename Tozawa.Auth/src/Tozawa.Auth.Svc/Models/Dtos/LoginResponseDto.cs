namespace Tozawa.Auth.Svc.Models.Dtos
{
    public class LoginResponseDto
    {
        public CurrentUserDto User { get; set; } = new();
        public bool LoginSuccess { get; set; }
        public int LoginAttemptCount { get; set; } = 0;
        public Guid ErrorMessageGuid { get; set; }
        public string ErrorMessage { get; set; } = "";
    }
}