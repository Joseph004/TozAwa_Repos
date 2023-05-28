using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tozawa.Language.Svc.Context;
using MediatR;


namespace Tozawa.Language.Svc.Controllers.TranslationControllers
{
    public class DeleteTranslationCommand : IRequest
    {
        public Guid Id { get; set; }

        public DeleteTranslationCommand(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(DeleteTranslationCommand));
            }
            Id = id;
        }
    }

    public class DeleteTranslationCommandHandler : IRequestHandler<DeleteTranslationCommand>
    {
        private readonly LanguageContext _context;
        public DeleteTranslationCommandHandler(LanguageContext context) => _context = context;

        public async Task<Unit> Handle(DeleteTranslationCommand request, CancellationToken cancellationToken)
        {
            var translations = _context.Translations.Where(x => x.TextId == request.Id).ToList();
            if (!translations.Any())
            {
                throw new ArgumentNullException(nameof(request.Id));
            }

            _context.Translations.RemoveRange(translations);
            _context.SaveChanges();
            return await Task.FromResult(Unit.Value);
        }
    }
}
