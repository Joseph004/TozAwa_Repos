
using MediatR;
using Microsoft.Extensions.Primitives;

namespace Grains.Auth.Controllers
{
    public class GetMembersQuery(Dictionary<string, StringValues> queryParameters = null, bool isAdmin = true) : BaseQuery(queryParameters), IRequest<TableDataDto<Models.Dtos.Backend.MemberDto>>
    {
        public bool IsAdminRequest { get; set; } = isAdmin;
    }
}