using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tozawa.Language.Svc.Context;
using FluentValidation;
using MediatR;

namespace Tozawa.Language.Svc.Controllers.LanguageControllers
{
    public class SetLanguageAsDefaultCommand : IRequest<Unit>
    {
        public Guid Id { get; }

        public SetLanguageAsDefaultCommand(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(SetLanguageAsDefaultCommand));
            }
            Id = id;
        }
    }

    public class SetLanguageAsDefaultCommandValidator : AbstractValidator<SetLanguageAsDefaultCommand>
    {
        public SetLanguageAsDefaultCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    public class SetLanguageAsDefaultCommandHandler : IRequestHandler<SetLanguageAsDefaultCommand>
    {
        private readonly LanguageContext _context;
        public SetLanguageAsDefaultCommandHandler(LanguageContext context) => _context = context;

        public async Task<Unit> Handle(SetLanguageAsDefaultCommand request, CancellationToken cancellationToken)
        {
            var language = _context.Languages.SingleOrDefault(x => x.Id == request.Id);

            if (language == null)
            {
                throw new ArgumentNullException($"{request.Id}");
            }

            var defaultLanguage = _context.Languages.SingleOrDefault(x => x.IsDefault);
            if (defaultLanguage != null)
            {
                defaultLanguage.IsDefault = false;
            }

            language.IsDefault = true;

            _context.SaveChanges();
            return await Task.FromResult(Unit.Value);
        }
    }
}
