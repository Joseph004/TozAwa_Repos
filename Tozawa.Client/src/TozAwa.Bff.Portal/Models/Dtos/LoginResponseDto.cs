namespace Tozawa.Bff.Portal.Models.Dtos
{
    public class LoginResponseDto
    {
        public bool LoginSuccess { get; set; }
        public int LoginAttemptCount { get; set; } = 0;
        public Guid ErrorMessageGuid { get; set; }
        public string ErrorMessage { get; set; } = "";
        public string Token { get; set; } = "";
        public string RefreshToken { get; set; }
        public string ServerMessage { get; set; } = "";
        public bool ProcessDone { get; set; } = false;
    }
}