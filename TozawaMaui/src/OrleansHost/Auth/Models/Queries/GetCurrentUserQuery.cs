using FluentValidation;
using MediatR;
using Grains.Auth.Models.Dtos;

namespace OrleansHost.Auth.Models.Queries
{
    public class GetCurrentUserQuery(Guid oid, Guid? organizationId = null) : IRequest<CurrentUserDto>
    {
        public Guid Oid { get; set; } = oid;
        public Guid? OrganizationId { get; set; } = organizationId;
    }


    public class GetCurrentUserQueryValidator : AbstractValidator<GetCurrentUserQuery>
    {
        public GetCurrentUserQueryValidator()
        {
            RuleFor(x => x.Oid).NotEmpty();
        }
    }
}