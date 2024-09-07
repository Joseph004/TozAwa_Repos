
using FluentValidation;
using MediatR;

namespace Grains.Auth.Controllers
{
    public class GetMemberQuery : IRequest<Models.Dtos.Backend.MemberDto>
    {
        public Guid Id { get; set; }
    }
    public class GetMemberQueryFluentValidator : AbstractValidator<GetMemberQuery>
    {
        public GetMemberQueryFluentValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();
        }
    }
}