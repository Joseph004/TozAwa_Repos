using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tozawa.Language.Svc.Context;
using Tozawa.Language.Svc.Models;
using FluentValidation;
using MediatR;

namespace Tozawa.Language.Svc.Controllers.ImportControllers
{
    public class ImportTranslationCommand : IRequest<ImportTranslationResultDto>
    {
        public Guid SystemTypeId { get; set; }
        public ImportTranslationTextDto Original { get; set; }
        public List<ImportTranslationTextDto> Translations { get; set; }
    }

    public class ImportTranslationCommandValidator : AbstractValidator<ImportTranslationCommand>
    {
        public ImportTranslationCommandValidator()
        {
            RuleFor(x => x.SystemTypeId).NotEmpty();
            RuleFor(x => x.Original.LanguageId).NotEmpty();
            RuleFor(x => x.Original.Text).NotEmpty();
        }
    }

    public class ImportTranslationTextDto
    {
        public Guid LanguageId { get; set; }
        public string Text { get; set; }
    }

    public class ImportTranslationCommandHandler : IRequestHandler<ImportTranslationCommand, ImportTranslationResultDto>
    {
        private readonly LanguageContext _context;

        public ImportTranslationCommandHandler(LanguageContext context)
        {
            _context = context;
        }

        public async Task<ImportTranslationResultDto> Handle(ImportTranslationCommand request, CancellationToken cancellationToken)
        {
            var newText = new Translation
            {
                Id = Guid.NewGuid(),
                TextId = Guid.NewGuid(),
                XliffState = XliffState.Translated,
                CreatedAt = DateTime.UtcNow,
                LanguageId = request.Original.LanguageId,
                Text = request.Original.Text,
                SystemTypeId = request.SystemTypeId,
            };
            _context.Translations.Add(newText);

            var textId = newText.TextId;

            if (request.Translations.Any())
            {
                foreach (var translation in request.Translations)
                {
                    var dbTranslation = new Translation
                    {
                        XliffState = XliffState.Translated,
                        CreatedAt = DateTime.UtcNow,
                        LanguageId = translation.LanguageId,
                        Text = translation.Text,
                        TextId = textId,
                        SystemTypeId = request.SystemTypeId,
                    };
                    _context.Translations.Add(dbTranslation);
                }
            }

            _context.SaveChanges();
            return await Task.FromResult(new ImportTranslationResultDto
            {
                TextId = textId
            });
        }
    }
}
