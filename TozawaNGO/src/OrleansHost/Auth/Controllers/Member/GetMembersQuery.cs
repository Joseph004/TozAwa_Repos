
using MediatR;
using Microsoft.Extensions.Primitives;

namespace Grains.Auth.Controllers
{
    public class GetMembersQuery(Dictionary<string, StringValues> queryParameters = null) : BaseQuery(queryParameters), IRequest<TableDataDto<Models.Dtos.Backend.MemberDto>>
    {
    }
}