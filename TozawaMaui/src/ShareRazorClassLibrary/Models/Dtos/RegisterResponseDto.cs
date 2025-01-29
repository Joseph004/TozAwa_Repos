
namespace ShareRazorClassLibrary.Models.Dtos
{
    public class RegisterResponseDto
    {
        public bool RegisterSuccess { get; set; }
        public Guid? ErrorMessageGuid { get; set; }
        public string ErrorMessage { get; set; } = "";
    }
}
