using Grains.Auth.Models.Dtos;

namespace Grains.Auth.Models.Converters;

public static class OrganizationConverter
{
    public static OrganizationDto Convert(OrganizationItem organization,
        List<AddressDto> addresses,
        List<RoleDto> roles)
    {
        var converted = ConvertSingle(organization, addresses, roles);

        return converted;
    }

    private static OrganizationDto ConvertSingle(OrganizationItem organization,
        List<AddressDto> addresses,
        List<RoleDto> roles)
    {
        return new OrganizationDto
        {
            Id = organization.Id,
            PhoneNumber = organization.PhoneNumber,
            Description = organization.Description,
            DescriptionTextId = organization.DescriptionTextId,
            Comment = organization.Comment,
            City = organization.City,
            Country = organization.CountryCode,
            CommentTextId = organization.CommentTextId,
            Features = organization.Features,
            CreateDate = organization.CreateDate,
            CreatedBy = organization.CreatedBy,
            Email = organization.Email,
            ModifiedBy = organization.ModifiedBy,
            ModifiedDate = organization.ModifiedDate,
            Name = organization.Name,
            Roles = roles,
            Addresses = addresses
        };
    }
}