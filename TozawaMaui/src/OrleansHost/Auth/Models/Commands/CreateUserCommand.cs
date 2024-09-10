using FluentValidation;
using MediatR;
using Grains.Auth.Models.Dtos.Backend;

namespace OrleansHost.Auth.Models.Commands
{
    public class CreateUserCommand : IRequest<MemberDto>
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<int> Roles { get; set; } = [0];
        public string Email { get; set; }
        public string Adress { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
    }
    public class CreateUserCommandFluentValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandFluentValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("FirstName cannot be empty")
                    .MinimumLength(3).WithMessage("FirstName length must be at least 3.")
                    .MaximumLength(30).WithMessage("FirstName length must not exceed 30.");

            RuleFor(x => x.LastName)
       .NotEmpty().WithMessage("LastName cannot be empty")
           .MinimumLength(3).WithMessage("LastName length must be at least 3.")
           .MaximumLength(30).WithMessage("LastName length must not exceed 30.");

            RuleFor(x => x.Email)
       .Cascade(CascadeMode.Stop)
       .NotEmpty()
      .WithMessage("A valid email is required")
       .Matches(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$")
       .WithMessage("A valid email is required")
       .MustAsync(async (value, cancellationToken) => await IsUniqueAsync(value));
        }

        private static async Task<bool> IsUniqueAsync(string email)
        {
            await Task.Delay(1);
            return email.ToLower() != "test@test.com";
        }
    }
}

