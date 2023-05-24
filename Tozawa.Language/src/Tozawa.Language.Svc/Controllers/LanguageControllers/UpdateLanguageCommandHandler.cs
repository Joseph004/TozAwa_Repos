using Tozawa.Language.Svc.Context;
using Tozawa.Language.Svc.Models;
using FluentValidation;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace Tozawa.Language.Svc.Controllers.LanguageControllers
{
    public class UpdateLanguageCommand : IRequest<LanguageDto>
    {
        public Guid Id { get; set; }
        public string ShortName { get; set; }
        public string Name { get; set; }
        public string LongName { get; set; }
        public bool Deleted { get; set; }
    }

    public class UpdateLanguageCommandValidator : AbstractValidator<UpdateLanguageCommand>
    {
        public UpdateLanguageCommandValidator()
        {
            RuleFor(x => x.ShortName).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.LongName).NotEmpty();
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    public class UpdateLanguageCommandHandler : IRequestHandler<UpdateLanguageCommand, LanguageDto>
    {
        private readonly LanguageContext _context;
        public UpdateLanguageCommandHandler(LanguageContext context) => _context = context;

        public Task<LanguageDto> Handle(UpdateLanguageCommand request, CancellationToken cancellationToken)
        {
            var language = _context.Languages.FirstOrDefault(x => x.Id == request.Id);
            if (language == null)
            {
                throw new ArgumentNullException("Language not found");
            }

            language.ShortName = request.ShortName;
            language.Name = request.Name;
            language.LongName = request.LongName;
            language.Deleted = request.Deleted;
            _context.SaveChanges();

            return Task.FromResult(Convert(language));
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