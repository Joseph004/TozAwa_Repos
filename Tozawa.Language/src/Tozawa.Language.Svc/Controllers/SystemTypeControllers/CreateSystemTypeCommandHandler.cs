using System;
using System.Threading;
using System.Threading.Tasks;
using Tozawa.Language.Svc.Context;
using FluentValidation;
using MediatR;

namespace Tozawa.Language.Svc.Controllers.SystemTypeControllers
{
    public class CreateSystemTypeCommand : IRequest<Guid>
    {
        public Guid? Id { get; set; }
        public string Description { get; set; }
    }

    public class CreateSystemTypeCommandValidator : AbstractValidator<CreateSystemTypeCommand>
    {
        public CreateSystemTypeCommandValidator()
        {
            RuleFor(x => x.Description).NotEmpty();
        }
    }

    public class CreateSystemTypeCommandHandler : IRequestHandler<CreateSystemTypeCommand, Guid>
    {
        private readonly LanguageContext _context;

        public CreateSystemTypeCommandHandler(LanguageContext context) => _context = context;

        public async Task<Guid> Handle(CreateSystemTypeCommand request, CancellationToken cancellationToken)
        {
            var systemType = new SystemType()
            {
                Id = request.Id ?? Guid.NewGuid(),
                Description = request.Description
            };
            _context.SystemTypes.Add(systemType);
            _context.SaveChanges();
            return await Task.FromResult(systemType.Id);
        }
    }
}
