
using MediatR;
using Microsoft.Extensions.Primitives;

namespace TozawaNGO.Auth.Controllers
{
    public class GetMembersQuery : IRequest<IEnumerable<Models.Dtos.Backend.MemberDto>>
    {
        public bool IncludeDeleted { get; set; }
        public GetMembersQuery(Dictionary<string, StringValues> queryParameters)
        {
            if (queryParameters == null)
            {
                return;
            }

            if (queryParameters.ContainsKey(nameof(IncludeDeleted)) && bool.TryParse(queryParameters[nameof(IncludeDeleted)], out var includeDeleted))
            {
                IncludeDeleted = includeDeleted;
            }
        }
    }
}