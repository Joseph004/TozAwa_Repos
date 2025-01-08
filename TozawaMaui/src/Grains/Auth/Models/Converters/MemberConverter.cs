using Grains.Auth.Models.Authentication;
using Grains.Auth.Models.Dtos;

namespace Grains.Auth.Models.Converters
{
    public static class MemberConverter
    {
        public static List<Models.Dtos.Backend.MemberDto> Convert(IEnumerable<ApplicationUser> members, Dictionary<Guid, List<AddressDto>> addresses)
        {
            var result = new List<Models.Dtos.Backend.MemberDto>();
            var enumerated = members as ApplicationUser[] ?? members.ToArray();
            for (var i = 0; i <= enumerated.Length - 1; i++)
            {
                var Member = Convert(enumerated[i], addresses.ContainsKey(enumerated[i].UserId) ? addresses[enumerated[i].UserId] : []);
                result.Add(Member);
            }
            return result;
        }
        public static Models.Dtos.Backend.MemberDto Convert(ApplicationUser member, List<AddressDto> addresses, bool isDeletedForever = false) => new()
        {
            Id = member.UserId,
            FirstName = member.FirstName,
            Deleted = member.Deleted,
            Email = member.Email,
            Description = member.Description,
            DescriptionTextId = member.DescriptionTextId,
            Comment = member.Comment,
            CommentTextId = member.CommentTextId,
            LastName = member.LastName,
            Features = member.Organizations?.SelectMany(o => o.Features) != null
            ? member.Organizations?.SelectMany(o => o.Features).Select(x => x.Feature).ToList()
            : [],
            Roles = member.Roles != null
            ? member.Roles.Select(x => new RoleDto
            {
                Id = x.Role.Id,
                OrganizationId = x.Role.OrganizationId,
                Functions = x.Role.Functions != null
                    ? x.Role.Functions.Select(y => new FunctionDto
                    {
                        FunctionType = y.Functiontype
                    }).ToList()
                    : []
            }).ToList()
            : [],
            Addresses = addresses,
            DeletedForever = isDeletedForever
        };
    }
}