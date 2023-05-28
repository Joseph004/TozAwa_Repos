namespace Tozawa.Bff.Portal.Models.Request.Frontend
{
    public class PatchMemberRequest
    {
        public string FirstName { get; set; } = null;
        public string LastName { get; set; } = null;
        public string Email { get; set; } = null;
        public Guid? DescriptionTextId { get; set; } = null;
        public bool? Deleted { get; set; } = null;
        public bool? DeleteForever { get; set; } = null;
    }
}