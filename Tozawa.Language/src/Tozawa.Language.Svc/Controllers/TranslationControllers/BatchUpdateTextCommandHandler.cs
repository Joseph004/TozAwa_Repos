using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tozawa.Language.Svc.Context;
using FluentValidation;
using MediatR;

namespace Tozawa.Language.Svc.Controllers.TranslationControllers
{
    public class BatchUpdateTextCommand : IRequest<List<string>>
    {
        public List<UpdateTextCommand> UpdateTextCommands { get; set; }
    }

    public class BatchUpdateTextCommandValidator : AbstractValidator<BatchUpdateTextCommand>
    {
        public BatchUpdateTextCommandValidator()
        {
            RuleFor(x => x.UpdateTextCommands).NotNull();
        }
    }

    public class BatchUpdateTextCommandHandler : IRequestHandler<BatchUpdateTextCommand, List<string>>
    {
        private readonly LanguageContext _context;
        public BatchUpdateTextCommandHandler(LanguageContext context) => _context = context;

        public async Task<List<string>> Handle(BatchUpdateTextCommand request, CancellationToken cancellationToken)
        {
            var result = new List<string>();
            foreach (var updateTextCommand in request.UpdateTextCommands)
            {
                var translations = _context.Translations.Where(x => x.TextId == updateTextCommand.Id);
                if (!translations.Any() || translations.Any(x => x.SystemTypeId != updateTextCommand.SystemTypeId))
                {
                    result.Add(null);
                    continue;
                }

                var translation = translations.FirstOrDefault(x =>
                    x.LanguageId == updateTextCommand.LanguageId && x.SystemTypeId == updateTextCommand.SystemTypeId);

                if (translation != null)
                {
                    translation.Text = updateTextCommand.Text;
                }
                else
                {
                    var newText = CreateTranslation(updateTextCommand, translations.First());
                    _context.Translations.Add(newText);
                }
                result.Add(updateTextCommand.Text);
            }

            await _context.SaveChangesAsync(cancellationToken);
            return result;
        }

        private static Translation CreateTranslation(UpdateTextCommand request, Translation translation)
        {
            return new Translation
            {
                TextId = translation.TextId,
                XliffState = XliffState.Translated,
                LanguageId = request.LanguageId,
                Text = request.Text,
                SystemTypeId = request.SystemTypeId
            };
        }
    }
}
