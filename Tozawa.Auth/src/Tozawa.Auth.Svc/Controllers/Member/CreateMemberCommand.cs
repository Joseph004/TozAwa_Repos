


using MediatR;

namespace Tozawa.Auth.Svc.Controllers
{
    public class CreateMemberCommand : IRequest<Models.Dtos.Backend.MemberDto>
    {
        public string Email { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Country { get; set; } = "Sweden";
        public Guid DescriptionId { get; set; }
    }
}