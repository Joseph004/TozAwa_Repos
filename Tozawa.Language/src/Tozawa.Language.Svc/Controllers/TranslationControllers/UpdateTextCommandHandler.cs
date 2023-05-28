using Tozawa.Language.Svc.Context;
using FluentValidation;
using MediatR;

namespace Tozawa.Language.Svc.Controllers.TranslationControllers
{
    public class UpdateTextCommand : IRequest<string>
    {
        public string Text { get; set; }
        public Guid Id { get; set; }
        public Guid LanguageId { get; set; }
        public Guid SystemTypeId { get; set; }
        public XliffState? XliffState { get; set; }
    }

    public class UpdateTextCommandValidator : AbstractValidator<UpdateTextCommand>
    {
        public UpdateTextCommandValidator()
        {
            RuleFor(x => x.Text).NotEmpty();
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.LanguageId).NotEmpty();
            RuleFor(x => x.SystemTypeId).NotEmpty();
        }
    }

    public class UpdateTextCommandHandler : IRequestHandler<UpdateTextCommand, string>
    {
        private readonly LanguageContext _context;
        public UpdateTextCommandHandler(LanguageContext context) => _context = context;

        public async Task<string> Handle(UpdateTextCommand request, CancellationToken cancellationToken)
        {
            var translations = _context.Translations.Where(x => x.TextId == request.Id).ToList();
            if (!translations.Any() || translations.Any(x => x.SystemTypeId != request.SystemTypeId))
            {
                throw new ArgumentNullException("Invalid text id");
            }

            var translation = translations.FirstOrDefault(x => x.LanguageId == request.LanguageId && x.SystemTypeId == request.SystemTypeId);
            if (translation != null)
            {
                return await UpdateExistingTranslation(request, translation);
            }
            else
            {
                return await CreateNewTranslation(request, translations);
            }
        }

        private async Task<string> CreateNewTranslation(UpdateTextCommand request, List<Translation> translations)
        {
            var translation = CreateTranslation(request, translations.First());
            translation.XliffState = XliffState.Translated;
            _context.Translations.Add(translation);
            return await Save(translation);
        }

        private async Task<string> UpdateExistingTranslation(UpdateTextCommand request, Translation translation)
        {
            if (request.XliffState.HasValue)
            {
                translation.XliffState = request.XliffState.Value;
                translation.Text = translation.XliffState == XliffState.NeedsTranslation ? string.Empty : request.Text;
            }
            else
            {
                translation.Text = request.Text;
            }
            return await Save(translation);
        }

        private async Task<string> Save(Translation translation)
        {
            _context.SaveChanges();
            return await Task.FromResult(translation.Text);
        }

        private static Translation CreateTranslation(UpdateTextCommand request, Translation translation) =>
            new Translation
            {
                TextId = translation.TextId,
                XliffState = XliffState.Translated,
                LanguageId = request.LanguageId,
                Text = request.Text,
                SystemTypeId = request.SystemTypeId
            };
    }
}
