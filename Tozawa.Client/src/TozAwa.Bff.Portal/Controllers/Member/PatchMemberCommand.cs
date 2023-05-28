using MediatR;
using Tozawa.Bff.Portal.Models.Dtos;
using Tozawa.Bff.Portal.Models.ResponseRequests;

namespace Tozawa.Bff.Portal.Controllers
{
    public class PatchMemberCommand : IRequest<UpdateResponse<MemberDto>>
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; } = "";
        public string Email { get; set; }
        public bool Deleted { get; set; }
        public bool DeleteForever { get; set; }
    }
}