using TozawaNGO.Auth.Controllers;
using TozawaNGO.Auth.Models.Authentication;
using TozawaNGO.Auth.Models.Dtos;

namespace TozawaNGO.Auth.Models.Converters
{
    public static class MemberConverter
    {
        public static List<Models.Dtos.Backend.MemberDto> Convert(IEnumerable<ApplicationUser> members)
        {
            var result = new List<Models.Dtos.Backend.MemberDto>();
            var enumerated = members as ApplicationUser[] ?? members.ToArray();
            for (var i = 0; i <= enumerated.Length - 1; i++)
            {
                var Member = Convert(enumerated[i]);
                result.Add(Member);
            }
            return result;

        }
        public static Models.Dtos.Backend.MemberDto Convert(ApplicationUser member, bool isDeletedForever = false) => new()
        {
            Id = member.UserId,
            FirstName = member.FirstName,
            Deleted = member.Deleted,
            Email = member.Email,
            Description = member.Description,
            LastName = member.LastName,
            Roles = member.Roles.Select(x => (RoleDto)x).ToList(),
            DeletedForever = isDeletedForever
        };
    }
}