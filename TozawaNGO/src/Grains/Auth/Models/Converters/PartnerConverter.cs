using Grains.Auth.Models.Authentication;
using Grains.Auth.Models.Dtos;

namespace Grains.Auth.Models.Converters
{
    public static class PartnerConverter
    {
        public static List<Models.Dtos.Backend.PartnerDto> Convert(IEnumerable<Partner> partners)
        {
            var result = new List<Models.Dtos.Backend.PartnerDto>();
            var enumerated = partners as Partner[] ?? partners.ToArray();
            for (var i = 0; i <= enumerated.Length - 1; i++)
            {
                var Partner = Convert(enumerated[i]);
                result.Add(Partner);
            }
            return result;

        }
        public static Models.Dtos.Backend.PartnerDto Convert(Partner partner) => new()
        {
            Id = partner.Id,
            Name = partner.Name,
            Deleted = partner.Deleted,
            Email = partner.Email,
            Description = partner.Description,
            Adress = partner.Adress
        };
    }
}