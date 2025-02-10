using FluentValidation;
using ShareRazorClassLibrary.Helpers;
using ShareRazorClassLibrary.Services;

namespace ShareRazorClassLibrary.Models.FormModels
{
    public class RegisterCommand
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Country { get; set; } = "";
        public string CountryCode { get; set; } = "";
        public string City { get; set; } = "";
        public string Password { get; set; }
        public byte[] Content { get; set; }
    }
    public class RegisterCommandFluentValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandFluentValidator(ITranslationService translationService, AuthenticationService authenticationService)
        {
            RuleFor(x => x.Email)
            .Cascade(CascadeMode.Continue)
            .NotNull()
            .EmailAddress()
            .WithMessage(translationService.Translate(SystemTextId.Avalidemailisrequired, "A valid email is required").Text)
            .Matches(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$")
            .WithMessage(translationService.Translate(SystemTextId.Avalidemailisrequired, "A valid email is required").Text)
            .MustAsync(async (value, cancellationToken) => await IsUniqueAsync(value));

            RuleFor(x => x.Password).NotEmpty().WithMessage(translationService.Translate(SystemTextId.YourPasswordCannotBeEmpty, "Your password cannot be empty").Text)
                    .MinimumLength(8).WithMessage(translationService.Translate(SystemTextId.YourPasswordLengthMustBeAtLeast8Letters, "Your password length must be at least 8.").Text)
                    .MaximumLength(16).WithMessage(translationService.Translate(SystemTextId.YourPasswordLengthMustNotExceed16Letters, "Your password length must not exceed 16.").Text)
                    .Matches(@"[A-Z]+").WithMessage(translationService.Translate(SystemTextId.YourPasswordMustContainAtLeastOneUppercaseLetter, "Your password must contain at least one uppercase letter.").Text)
                    .Matches(@"[a-z]+").WithMessage(translationService.Translate(SystemTextId.YourPasswordMustContainAtLeastOneLowercaseLetter, "Your password must contain at least one lowercase letter.").Text)
                    .Matches(@"[0-9]+").WithMessage(translationService.Translate(SystemTextId.YourPasswordMustContainAtLeastOneNumber, "Your password must contain at least one number.").Text)
                    .Matches(@"[\!\?\*\.]+").WithMessage(translationService.Translate(SystemTextId.YourPasswordMustContainAtLeastOneSymbol, "Your password must contain at least one (!? *.).").Text);
        }
        private async Task<bool> IsUniqueAsync(string email)
        {
            // Simulates a long running http call
            await Task.Delay(1);
            return !email.Equals("test@test.com", StringComparison.CurrentCultureIgnoreCase);
        }
        private async Task<bool> IsEmailError(string email, string emailError)
        {
            return await Task.FromResult(string.IsNullOrEmpty(emailError));
        }
        private async Task<bool> IsUserNameError(string password, string passwordError)
        {
            return await Task.FromResult(string.IsNullOrEmpty(passwordError));
        }
        private async Task<bool> IsPasswordError(string userName, string userNameError)
        {
            return await Task.FromResult(string.IsNullOrEmpty(userNameError));
        }
        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
         {
             var result = await ValidateAsync(ValidationContext<RegisterCommand>.CreateWithOptions((RegisterCommand)model, x => x.IncludeProperties(propertyName)));
             if (result.IsValid)
                 return [];
             return result.Errors.Select(e => e.ErrorMessage);
         };
    }
    public class RegisterRequest
    {
        public string Email { get; set; } = "";
        public byte[] Content { get; set; }
    }
}