using Tozawa.Bff.Portal.Models.Dtos;
using Tozawa.Bff.Portal.Services;

namespace Tozawa.Bff.Portal.Controllers
{
    public class MemberConverter : IMemberConverter
    {
        private readonly ILanguageService _languageService;

        public MemberConverter(ILanguageService languageService)
        {
            _languageService = languageService;
        }
        public MemberDto Convert(Models.Dtos.Backend.MemberDto from) => new()
        {
            Id = from.Id,
            FirstName = from.FirstName,
            Deleted = from.Deleted,
            Email = from.Email,
            DescriptionTextId = from.DescriptionTextId,
            Description = from.DescriptionTextId != Guid.Empty ? _languageService.GetSync(from.DescriptionTextId) : "",
            LastName = from.LastName
        };

        public List<MemberDto> Convert(IEnumerable<Models.Dtos.Backend.MemberDto> from)
        {
            var result = new List<MemberDto>();
            var enumerated = from as Models.Dtos.Backend.MemberDto[] ?? from.ToArray();
            for (var i = 0; i <= enumerated.Length - 1; i++)
            {
                result.Add(Convert(enumerated[i]));
            }
            return result;
        }
    }
}