using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tozawa.Language.Svc.Context;
using FluentValidation;
using MediatR;


namespace Tozawa.Language.Svc.Controllers.SystemTypeControllers
{
    public class DeleteSystemTypeCommand : IRequest<Guid>
    {
        public Guid? Id { get; set; }
    }

    public class DeleteSystemTypeCommandValidator : AbstractValidator<DeleteSystemTypeCommand>
    {
        public DeleteSystemTypeCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    public class DeleteSystemTypeCommandHandler : IRequestHandler<DeleteSystemTypeCommand, Guid>
    {
        private readonly LanguageContext _context;

        public DeleteSystemTypeCommandHandler(LanguageContext context) => _context = context;

        public async Task<Guid> Handle(DeleteSystemTypeCommand request, CancellationToken cancellationToken)
        {
            var systemTypeToDelete = _context.SystemTypes.FirstOrDefault(x => x.Id == request.Id);
            if (systemTypeToDelete == null)
            {
                throw new ArgumentNullException(nameof(request.Id));
            }
            _context.SystemTypes.Remove(systemTypeToDelete);
            _context.SaveChanges();
            return await Task.FromResult(systemTypeToDelete.Id);
        }
    }
}