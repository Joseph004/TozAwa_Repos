using Grains.Auth.Models.Authentication;
using Grains.Auth.Models.Dtos;

namespace Grains.Auth.Models.Converters;

public static class OrganizationConverter
{
    public static OrganizationDto Convert(Organization organization, List<AddressDto> addresses)
    {
        var converted = ConvertSingle(organization, addresses);

        return converted;
    }

    private static OrganizationDto ConvertSingle(Organization organization, List<AddressDto> addresses)
    {
        return new OrganizationDto
        {
            Id = organization.Id,
            PhoneNumber = organization.PhoneNumber,
            Description = organization.Description,
            DescriptionTextId = organization.DescriptionTextId,
            Comment = organization.Comment,
            CommentTextId = organization.CommentTextId,
            Features = organization.Features != null
                        ? organization.Features.Select(x => x.Feature).ToList()
                        : [],
            CreateDate = organization.CreateDate,
            CreatedBy = organization.CreatedBy,
            Email = organization.Email,
            ModifiedBy = organization.ModifiedBy,
            ModifiedDate = organization.ModifiedDate,
            Name = organization.Name,
            Roles = organization.Roles != null ? organization.Roles.Select(RoleConverter.Convert).ToList() : [],
            Addresses = addresses
        };
    }
}