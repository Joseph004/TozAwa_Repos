using MediatR;
using Microsoft.Extensions.Primitives;
using Tozawa.Bff.Portal.Models.Dtos;

namespace Tozawa.Bff.Portal.Controllers
{
    public class GetMembersQuery : BaseQuery, IRequest<TableDataDto<MemberDto>>
    {
        public GetMembersQuery(Dictionary<string, StringValues> queryParameters = null) : base(queryParameters)
        {

        }
    }
}