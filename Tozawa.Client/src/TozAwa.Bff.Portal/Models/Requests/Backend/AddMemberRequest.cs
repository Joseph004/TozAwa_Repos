namespace Tozawa.Bff.Portal.Models.Request.Backend
{
    public class AddMemberRequest
    {
        public string Email { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public Guid DescriptionId { get; set; }
    }
}