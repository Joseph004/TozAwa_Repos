

namespace ShareRazorClassLibrary.Models.Dtos
{
    public class UserDto
    {
        public string Password { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime LastLogin { get; set; }
        public List<RoleDto> Roles { get; set; }
        public Guid PartnerId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Adresse { get; set; }
        public Guid Id { get; set; }
        public bool IsAuthenticated { get; set; }
    }
}


