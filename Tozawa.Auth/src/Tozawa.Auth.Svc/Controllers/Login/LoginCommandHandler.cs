using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Tozawa.Auth.Svc.Models.Dtos;

namespace Tozawa.Auth.Svc.Controllers.Login
{
    public class LoginCommand : IRequest<LoginResponseDto>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class LoginCommandFluentValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandFluentValidator()
        {
            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
               .WithMessage("A valid email is required")
                .Matches(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$")
                .WithMessage("A valid email is required")
                .MustAsync(async (value, cancellationToken) => await IsUniqueAsync(value));

            RuleFor(x => x.Password).NotEmpty().WithMessage("Your password cannot be empty")
                    .MinimumLength(8).WithMessage("Your password length must be at least 8.")
                    .MaximumLength(16).WithMessage("Your password length must not exceed 16.")
                    .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
                    .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
                    .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.")
                    .Matches(@"[\!\?\*\.]+").WithMessage("Your password must contain at least one (!? *.).");
        }
        private async Task<bool> IsUniqueAsync(string email)
        {
            await Task.Delay(1);
            return email.ToLower() != "test@test.com";
        }
    }
    public class LoginRequest
    {
        public string Email { get; set; }
        public byte[] Content { get; set; }
    }
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponseDto>
    {
        public LoginCommandHandler()
        {

        }

        public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var response = new LoginResponseDto
            {
                LoginSuccess = true,
                ErrorMessage = ""
            };
            if (request.Email != "josephluhandu@yahoo.com")
            {
                response.LoginSuccess = false;
                response.ErrorMessage = "Your email adress does not exist";
            }
            if (request.Password != "Congonumber01?")
            {
                response.LoginSuccess = false;
                response.ErrorMessage = "This password is wrong";
            }
            return await Task.FromResult(response);
        }

        private static readonly AuthenticationProperties COOKIE_EXPIRES = new AuthenticationProperties()
        {
            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
            IsPersistent = true,
        };
    }
}

