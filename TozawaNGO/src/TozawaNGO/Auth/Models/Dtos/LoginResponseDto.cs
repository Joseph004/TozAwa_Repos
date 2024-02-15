namespace TozawaNGO.Auth.Models.Dtos
{
    public class LoginResponseDto
    {
        //public CurrentUserDto User { get; set; } = new();
        public bool LoginSuccess { get; set; }
        public int LoginAttemptCount { get; set; } = 0;
        public Guid ErrorMessageGuid { get; set; }
        public string ErrorMessage { get; set; } = "";
        public string Token { get; set; } = "";
        public string ExpiresIn { get; set; }
        public string RefreshToken { get; set; } = "";
        public string ServerMessage { get; set; } = "";
        public bool ProcessDone { get; set; } = false;
    }
}