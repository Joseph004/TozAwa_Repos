using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tozawa.Language.Svc.Context;
using MediatR;

namespace Tozawa.Language.Svc.Controllers.LanguageControllers
{
    public class DeleteLanguageCommand : IRequest
    {
        public Guid Id { get; set; }

        public DeleteLanguageCommand(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException(nameof(DeleteLanguageCommand));
            }
            Id = id;
        }
    }

    public class DeleteLanguageCommandHandler : IRequestHandler<DeleteLanguageCommand>
    {
        private readonly LanguageContext _context;
        public DeleteLanguageCommandHandler(LanguageContext context) => _context = context;

        public async Task<Unit> Handle(DeleteLanguageCommand request, CancellationToken cancellationToken)
        {
            var language = _context.Languages.FirstOrDefault(x => x.Id == request.Id);
            if (language == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (language.Deleted)
            {
                var existingTranslations = _context.Translations.FirstOrDefault(x => x.LanguageId == request.Id);
                if (existingTranslations != null)
                {
                    throw new ArgumentException("Cannot delete a language with existing translations.");
                }
                _context.Remove(language);
            }
            else
            {
                language.Deleted = true;
            }
            _context.SaveChanges();
            return await Task.FromResult(Unit.Value);
        }
    }
}
