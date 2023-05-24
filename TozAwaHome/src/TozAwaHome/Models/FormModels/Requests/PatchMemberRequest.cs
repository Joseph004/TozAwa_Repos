
namespace TozAwaHome.Models.FormModels
{
    public class PatchMemberRequest : DeleteRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; } = "";
        public string Email { get; set; }
    }
}