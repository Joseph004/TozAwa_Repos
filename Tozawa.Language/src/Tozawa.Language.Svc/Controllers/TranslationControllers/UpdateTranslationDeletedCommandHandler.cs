using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tozawa.Language.Svc.Context;
using FluentValidation;
using MediatR;


namespace Tozawa.Language.Svc.Controllers.TranslationControllers
{
    public class UpdateTranslationDeletedCommand : IRequest
    {
        public Guid Id { get; set; }
        public bool Deleted { get; set; }
        public UpdateTranslationDeletedCommand(Guid id, bool deleted)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(UpdateTranslationDeletedCommand));
            }
            Id = id;
            Deleted = deleted;
        }
    }

    public class UpdateTranslationDeletedCommandValidator : AbstractValidator<UpdateTranslationDeletedCommand>
    {
        public UpdateTranslationDeletedCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    public class UpdateTranslationDeletedCommandHandler : IRequestHandler<UpdateTranslationDeletedCommand>
    {
        private readonly LanguageContext _context;
        public UpdateTranslationDeletedCommandHandler(LanguageContext context) => _context = context;
        public async Task<Unit> Handle(UpdateTranslationDeletedCommand request, CancellationToken cancellationToken)
        {
            var translations = _context.Translations.Where(x => x.TextId == request.Id).ToList();
            if (!translations.Any())
            {
                throw new ArgumentNullException("Invalid text id");
            }

            foreach (var translation in translations)
            {
                translation.Deleted = request.Deleted;
            }

            _context.SaveChanges();

            return await Task.FromResult(Unit.Value);
        }
    }
}
