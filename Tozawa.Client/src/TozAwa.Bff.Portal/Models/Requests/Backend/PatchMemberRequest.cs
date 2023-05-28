using Tozawa.Client.Portal.HttpClients;

namespace Tozawa.Bff.Portal.Models.Request.Backend
{
    public class PatchMemberRequest : PatchBase
    {
        public PatchMemberRequest(Frontend.PatchMemberRequest bffRequest)
        {
            FirstName = bffRequest.FirstName;
            LastName = bffRequest.LastName;
            Email = bffRequest.Email;
            DescriptionTextId = bffRequest.DescriptionTextId;
            Deleted = bffRequest.Deleted;
            DeleteForever = bffRequest.DeleteForever;
        }
        public string FirstName { get; set; } = null;
        public string LastName { get; set; } = null;
        public string Email { get; set; } = null;
        public Guid? DescriptionTextId { get; set; }
        public bool? Deleted { get; set; } = null;
        public bool? DeleteForever { get; set; } = null;
    }
}