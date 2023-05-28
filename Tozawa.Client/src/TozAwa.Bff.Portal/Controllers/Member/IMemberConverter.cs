using Tozawa.Bff.Portal.Models.Dtos;
using Tozawa.Bff.Portal.Services;

namespace Tozawa.Bff.Portal.Controllers
{
    public interface IMemberConverter : IEntityConverter<Models.Dtos.Backend.MemberDto, MemberDto>
    {
    }
}