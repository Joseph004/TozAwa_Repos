using Tozawa.Language.Svc.Context;
using Tozawa.Language.Svc.Models;
using FluentValidation;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tozawa.Language.Svc.Controllers.LanguageControllers
{
    public class CreateLanguageCommand : IRequest<LanguageDto>
    {
        public string ShortName { get; set; }
        public string Name { get; set; }
        public string LongName { get; set; }
    }

    public class CreateLanguageCommandValidator : AbstractValidator<CreateLanguageCommand>
    {
        public CreateLanguageCommandValidator()
        {
            RuleFor(x => x.ShortName).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.LongName).NotEmpty();
        }
    }

    public class CreateLanguageCommandHandler : IRequestHandler<CreateLanguageCommand, LanguageDto>
    {
        private readonly LanguageContext _context;
        public CreateLanguageCommandHandler(LanguageContext context) => _context = context;

        public async Task<LanguageDto> Handle(CreateLanguageCommand request, CancellationToken cancellationToken)
        {
            var language = CreateLanguage(request);
            _context.Languages.Add(language);
            _context.SaveChanges();
            return await Task.FromResult(Convert(language));
        }

        private static Languagemd CreateLanguage(CreateLanguageCommand request)
        {
            return new Languagemd()
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                LongName = request.LongName,
                ShortName = request.ShortName
            };
        }

        private LanguageDto Convert(Languagemd language) => new LanguageDto
        {
            Id = language.Id,
            Deleted = language.Deleted,
            ShortName = language.ShortName,
            Name = language.Name,
            LongName = language.LongName
        };
    }
}
