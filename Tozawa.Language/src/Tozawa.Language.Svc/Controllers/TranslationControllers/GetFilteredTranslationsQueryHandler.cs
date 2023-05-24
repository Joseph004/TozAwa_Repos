using Tozawa.Language.Svc.Context;
using Tozawa.Language.Svc.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace Tozawa.Language.Svc.Controllers.TranslationControllers
{
    public class GetFilteredTranslations : IRequest<IEnumerable<TranslationResponseDto>>
    {
        public Guid SystemTypeId { get; set; }
        public Guid LanguageId { get; set; }

        public GetFilteredTranslations(Guid systemTypeId, Guid languageId)
        {
            if (languageId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(GetFilteredTranslations));
            }

            SystemTypeId = systemTypeId;
            LanguageId = languageId;
        }
    }

    public class GetFilteredTranslationsQueryHandler : IRequestHandler<GetFilteredTranslations, IEnumerable<TranslationResponseDto>>
    {
        private readonly LanguageContext _context;

        public GetFilteredTranslationsQueryHandler(LanguageContext context) => _context = context;

        public async Task<IEnumerable<TranslationResponseDto>> Handle(GetFilteredTranslations request, CancellationToken cancellationToken)
        {
            var language = await _context.Languages.FirstOrDefaultAsync(x => x.Id == request.LanguageId, cancellationToken);
            if (language == null || language.Deleted)
            {
                throw new ArgumentNullException(nameof(request.LanguageId));
            }

            if (request.SystemTypeId == Guid.Empty)
                throw new ArgumentNullException(nameof(request));

            var translations = await _context.Translations
               .Where(x => x.SystemTypeId == request.SystemTypeId && x.LanguageId == request.LanguageId && !x.Deleted)
               .Select(translation => new { translation.TextId, translation.Text })
               .ToListAsync();

            return translations.Select(translation => new TranslationResponseDto { Id = translation.TextId, Text = translation.Text });
        }
    }
}