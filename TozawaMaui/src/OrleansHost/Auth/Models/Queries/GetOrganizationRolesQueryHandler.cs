using FluentValidation;
using Grains.Auth.Models.Converters;
using Grains.Auth.Models.Dtos;
using MediatR;

namespace OrleansHost.Auth.Models.Queries;

public class GetOrganizationRolesQuery(Guid id) : IRequest<IEnumerable<RoleDto>>
{
    public Guid Id { get; set; } = id;
}

public class GetOrganizationRolesQueryValidator : AbstractValidator<GetOrganizationRolesQuery>
{
    public GetOrganizationRolesQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

public class GetOrganizationRolesQueryHandler(IMediator mediator) : IRequestHandler<GetOrganizationRolesQuery, IEnumerable<RoleDto>>
{
    private readonly IMediator _mediator = mediator;

    public async Task<IEnumerable<RoleDto>> Handle(GetOrganizationRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await _mediator.Send(new GetRolesQuery(request.Id));
        return roles;
    }
}