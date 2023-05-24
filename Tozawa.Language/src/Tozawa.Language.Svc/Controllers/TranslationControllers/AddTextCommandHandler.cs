using System;
using System.Threading;
using System.Threading.Tasks;
using Tozawa.Language.Svc.Context;
using FluentValidation;
using MediatR;

namespace Tozawa.Language.Svc.Controllers.TranslationControllers
{
    public class AddTextCommand : IRequest<Guid>
    {
        public string Text { get; set; }
        public Guid LanguageId { get; set; }
        public Guid SystemTypeId { get; set; }
    }

    public class AddTextCommandValidator : AbstractValidator<AddTextCommand>
    {
        public AddTextCommandValidator()
        {
            RuleFor(x => x.Text).NotEmpty();
            RuleFor(x => x.LanguageId).NotEmpty();
            RuleFor(x => x.SystemTypeId).NotEmpty();
        }
    }

    public class AddTextCommandHandler : IRequestHandler<AddTextCommand, Guid>
    {
        private readonly LanguageContext _context;
        public AddTextCommandHandler(LanguageContext context) => _context = context;

        public async Task<Guid> Handle(AddTextCommand request, CancellationToken cancellationToken)
        {
            var newText = CreateTranslation(request);
            _context.Translations.Add(newText);
            _context.SaveChanges();
            return await Task.FromResult(newText.TextId);
        }

        private static Translation CreateTranslation(AddTextCommand request)
        {
            return new Translation
            {
                TextId = Guid.NewGuid(),
                XliffState = XliffState.Translated,
                LanguageId = request.LanguageId,
                Text = request.Text.Trim(),
                IsOriginalText = true,
                SystemTypeId = request.SystemTypeId
            };
        }
    }
}
