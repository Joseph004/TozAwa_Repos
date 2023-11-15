using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using TozawaNGO.Helpers;
using TozawaNGO.Services;
using TozawaNGO.Services;

namespace TozawaNGO.Models.FormModels
{
    public class LoginCommand
    {
        public bool LoginAsRoot { get; set; } = false;
        public string UserName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; }
    }
    public class LoginCommandFluentValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandFluentValidator(ITranslationService translationService, AuthenticationService authenticationService)
        {
            RuleFor(x => x.LoginAsRoot).NotNull();

            RuleFor(x => x.Email)
            .Cascade(CascadeMode.Continue)
            .NotNull()
            .EmailAddress()
            .WithMessage(translationService.Translate(SystemTextId.Avalidemailisrequired, "A valid email is required").Text)
            .Matches(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$")
            .WithMessage(translationService.Translate(SystemTextId.Avalidemailisrequired, "A valid email is required").Text)
            .MustAsync(async (value, cancellationToken) => await IsUniqueAsync(value))
            .When(y => !y.LoginAsRoot);

            RuleFor(x => x.UserName)
            .NotNull()
            .WithMessage(translationService.Translate(SystemTextId.Usernamecannotbeempty, "Username cannot be empty").Text)
            .MinimumLength(6).WithMessage(translationService.Translate(SystemTextId.UserNameMustBeAtLeastSixLetters, "Username length must be at least 6 letters.").Text)
            .MaximumLength(30).WithMessage(translationService.Translate(SystemTextId.UserNameMustNotExceedthrertyLetters, "User name length must not exceed 30.").Text)
            //.MustAsync((x, cancellationToken) => UserIsNotLockedout(authenticationService, x)).WithMessage(translationService.Translate(SystemTextId.TemporarlyLockout, "You've been temporarely lockedout, please contact a technician!").Text)
            .When(y => y.LoginAsRoot);

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
            return email.ToLower() != "test@test.com";
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
             var result = await ValidateAsync(ValidationContext<LoginCommand>.CreateWithOptions((LoginCommand)model, x => x.IncludeProperties(propertyName)));
             if (result.IsValid)
                 return Array.Empty<string>();
             return result.Errors.Select(e => e.ErrorMessage);
         };
    }
    public class LoginRequest
    {
        public string Email { get; set; } = "";
        public string Content { get; set; }
    }
}