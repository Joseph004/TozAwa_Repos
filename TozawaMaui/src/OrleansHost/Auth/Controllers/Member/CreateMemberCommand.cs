


using FluentValidation;
using Grains.Auth.Models.Authentication;
using Grains.Models;
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
        public string Comment { get; set; } = "";
        public List<int> Features { get; set; } = [];
        public List<AddressCommand> Addresses { get; set; } = [];
        public List<RoleEnum> Roles { get; set; } = [];
        public List<FunctionType> Functions { get; set; } = [];
        public List<TranslationRequest> DescriptionTranslations { get; set; } = [];
        public List<TranslationRequest> CommentTranslations { get; set; } = [];
    }
    public class AddressCommand
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public bool Active { get; set; }
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