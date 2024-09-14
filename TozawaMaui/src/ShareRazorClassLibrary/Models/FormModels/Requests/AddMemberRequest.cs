using FluentValidation;
using ShareRazorClassLibrary.Helpers;
using ShareRazorClassLibrary.Services;

namespace ShareRazorClassLibrary.Models.FormModels
{
    public class AddMemberRequest
    {
        public string Email { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Description { get; set; } = "";
        public List<TranslationRequest> DescriptionTranslations { get; set; } = [];
    }
    public class AddMemberRequestFluentValidator : AbstractValidator<AddMemberRequest>
    {
        public AddMemberRequestFluentValidator(ITranslationService translationService)
        {
            RuleFor(x => x.Email)
            .Cascade(CascadeMode.Continue)
            .NotNull()
            .EmailAddress()
            .WithMessage(translationService.Translate(SystemTextId.Avalidemailisrequired, "A valid email is required").Text)
            .Matches(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$")
            .WithMessage(translationService.Translate(SystemTextId.Avalidemailisrequired, "A valid email is required").Text)
            .MustAsync(async (value, cancellationToken) => await IsUniqueAsync(value));
        }
        private async Task<bool> IsUniqueAsync(string email)
        {
            // Simulates a long running http call
            await Task.Delay(1);
            return email.ToLower() != "test@test.com";
        }
    }
}