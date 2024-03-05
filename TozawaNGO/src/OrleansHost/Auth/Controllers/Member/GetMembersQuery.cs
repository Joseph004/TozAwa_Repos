
using MediatR;
using Microsoft.Extensions.Primitives;

namespace OrleansHost.Auth.Controllers
{
    public class GetMembersQuery(Dictionary<string, StringValues> queryParameters = null) : BaseQuery(queryParameters), IRequest<TableDataDto<Models.Dtos.Backend.MemberDto>>
    {
    }
}