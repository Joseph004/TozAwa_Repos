


using FluentValidation;
using MediatR;

namespace Grains.Auth.Controllers
{
    public class CreateMemberCommand : IRequest<Models.Dtos.Backend.MemberDto>
    {
        public string Email { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Country { get; set; } = "Sweden";
        public Guid DescriptionId { get; set; }
        public string Description { get; set; } = "";
        public List<TranslationRequest> DescriptionTranslations { get; set; } = [];
    }

    public class CreateMemberCommandRequestFluentValidator : AbstractValidator<CreateMemberCommand>
    {
        public CreateMemberCommandRequestFluentValidator()
        {
            RuleFor(x => x.FirstName)
            .NotNull()
            .NotEmpty();

            RuleFor(x => x.LastName)
            .NotNull()
            .NotEmpty();

            RuleFor(x => x.DescriptionTranslations)
            .NotNull()
            .Must(x => x.All(y => y.LanguageId != Guid.Empty));

            RuleFor(x => x.Email)
            .Cascade(CascadeMode.Continue)
            .NotNull()
            .EmailAddress()
            .WithMessage("A valid email is required")
            .Matches(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$")
            .WithMessage("A valid email is required");
        }
    }
    public class TranslationRequest
    {
        public Guid LanguageId { get; set; }
        public string Text { get; set; }
    }
}