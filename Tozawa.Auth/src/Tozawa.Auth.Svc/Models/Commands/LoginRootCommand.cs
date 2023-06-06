using FluentValidation;

namespace Tozawa.Auth.Svc.Models.Commands
{
    public class LoginRootCommand
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
    public class LoginRootCommandFluentValidator : AbstractValidator<LoginRootCommand>
    {
        public LoginRootCommandFluentValidator()
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
    }
    public class LoginRootRequest
    {
        public string UserName { get; set; }
        public string Content { get; set; }
    }
}

