using Grains.Auth.Models.Dtos;

namespace Grains.Auth.Models.Converters
{
    public static class MemberConverter
    {
        public static Models.Dtos.Backend.MemberDto Convert(
            MemberItem member,
        List<CurrentUserOrganizationDto> organizations,
        List<AddressDto> addresses,
        List<RoleDto> roles,
        List<CurrentUserFunctionDto> functions,
        bool isDeletedForever = false) => new()
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
            Features = member.Features,
            Roles = roles,
            Gender = member.Gender,
            Addresses = addresses,
            DeletedForever = isDeletedForever,
            Organizations = organizations,
            Functions = functions
        };
    }
}