using MediatR;
using Tozawa.Bff.Portal.Models;
using Tozawa.Bff.Portal.Models.Dtos;
using Tozawa.Bff.Portal.Models.ResponseRequests;

namespace Tozawa.Bff.Portal.Controllers
{
    public class AddMemberCommand : IRequest<AddResponse<MemberDto>>
    {
        public string Email { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Description { get; set; } = "";
        public List<TranslationRequest> DescriptionTranslations { get; set; } = new List<TranslationRequest>();
    }
}