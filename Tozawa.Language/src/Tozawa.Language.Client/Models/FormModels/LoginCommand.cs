using FluentValidation;
using Tozawa.Language.Client.Services;

namespace Tozawa.Language.Client.Models.FormModels
{
    public class LoginCommand
    {

        public string UserName { get; set; }
        public string Password { get; set; }
    }
    public class LoginCommandFluentValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandFluentValidator(AuthenticationService authenticationService, CancellationToken token)
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username cannot be empty")
                    .MinimumLength(6).WithMessage("Username length must be at least 6.")
                    .MaximumLength(30).WithMessage("Your password length must not exceed 30.");

            RuleFor(x => x.Password).NotEmpty().WithMessage("Your password cannot be empty")
                    .MinimumLength(8).WithMessage("Your password length must be at least 8.")
                    .MaximumLength(16).WithMessage("Your password length must not exceed 16.")
                    .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
                    .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
                    .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.")
                    .Matches(@"[\!\?\*\.]+").WithMessage("Your password must contain at least one (!? *.).");
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
        public string UserName { get; set; } = "";
        public byte[] Content { get; set; }
    }
}