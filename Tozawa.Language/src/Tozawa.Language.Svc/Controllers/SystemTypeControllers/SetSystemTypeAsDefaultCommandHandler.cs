using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tozawa.Language.Svc.Context;
using FluentValidation;
using MediatR;


namespace Tozawa.Language.Svc.Controllers.SystemTypeControllers
{
    public class SetSystemTypeAsDefaultCommand : IRequest<Unit>
    {
        public Guid Id { get; }
        public SetSystemTypeAsDefaultCommand(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(SetSystemTypeAsDefaultCommand));
            }
            Id = id;
        }
    }

    public class SetSystemTypeAsDefaultCommandValidator : AbstractValidator<SetSystemTypeAsDefaultCommand>
    {
        public SetSystemTypeAsDefaultCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    public class SetSystemTypeAsDefaultCommandHandler : IRequestHandler<SetSystemTypeAsDefaultCommand>
    {
        private readonly LanguageContext _context;
        public SetSystemTypeAsDefaultCommandHandler(LanguageContext context) => _context = context;

        public async Task<Unit> Handle(SetSystemTypeAsDefaultCommand request, CancellationToken cancellationToken)
        {
            var systemType = _context.SystemTypes.SingleOrDefault(x => x.Id == request.Id);

            if (systemType == null)
            {
                throw new ArgumentNullException($"{request.Id}");
            }

            var defaultSystemType = _context.SystemTypes.SingleOrDefault(x => x.IsDefault);
            if (defaultSystemType != null)
            {
                defaultSystemType.IsDefault = false;
            }
            systemType.IsDefault = true;
            _context.SaveChanges();
            return await Task.FromResult(Unit.Value);
        }
    }
}
