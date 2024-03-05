using FluentValidation;
using MediatR;
using OrleansHost.Auth.Models.Dtos.Backend;

namespace OrleansHost.Auth.Models.Commands
{
    public class CreatePartnerCommand  : IRequest<PartnerDto>
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Adress { get; set; }
        public string PhoneNumber { get; set; }
    }
    public class CreatePartnerCommandFluentValidator : AbstractValidator<CreatePartnerCommand>
    {
        public CreatePartnerCommandFluentValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Partnername cannot be empty")
                    .MinimumLength(3).WithMessage("Partnername length must be at least 3.")
                    .MaximumLength(30).WithMessage("Partnername length must not exceed 30.");

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

