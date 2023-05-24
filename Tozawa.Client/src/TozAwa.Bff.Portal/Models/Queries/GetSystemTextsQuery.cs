

using System;
using MediatR;

namespace Tozawa.Bff.Portal.Models
{
    public class GetSystemTextsQuery : IRequest<Dictionary<Guid, string>>
    {
        public Guid LanguageId { get; set; }
    }
}
