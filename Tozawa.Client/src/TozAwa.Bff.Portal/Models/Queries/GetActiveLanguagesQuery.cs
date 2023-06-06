

using MediatR;
using Microsoft.Extensions.Primitives;
using Tozawa.Bff.Portal.Models.Dtos;

namespace Tozawa.Bff.Portal.Models
{
    public class GetActiveLanguagesQuery : IRequest<List<ActiveLanguageDto>>
    {
    }
}
