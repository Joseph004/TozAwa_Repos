
using MediatR;
using Microsoft.Extensions.Primitives;

namespace TozawaNGO.Auth.Controllers
{
    public class GetMembersQuery : BaseQuery, IRequest<TableDataDto<Models.Dtos.Backend.MemberDto>>
    {
        public GetMembersQuery(Dictionary<string, StringValues> queryParameters = null) : base(queryParameters)
        {

        }
    }
}