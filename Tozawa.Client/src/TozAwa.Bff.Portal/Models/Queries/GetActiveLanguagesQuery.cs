

using MediatR;
using Microsoft.Extensions.Primitives;
using Tozawa.Bff.Portal.Models.Dtos;

namespace Tozawa.Bff.Portal.Models
{
    public class GetActiveLanguagesQuery : IRequest<List<ActiveLanguageDto>>
    {
        public string Country { get; set; }

        public GetActiveLanguagesQuery(Dictionary<string, StringValues> queryParameters = null)
        {
            if (queryParameters.ContainsKey(nameof(Country)) && !string.IsNullOrEmpty(queryParameters[nameof(Country)]))
            {
                Country = queryParameters[nameof(Country)];
            }
        }
    }
}
