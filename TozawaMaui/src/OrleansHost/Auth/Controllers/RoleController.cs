using Grains.Auth.Services;
using Grains.Helpers;
using Grains.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrleansHost.Auth.Models.Queries;

namespace Grains.Auth.Controllers;

[AuthorizeUserRequirement]
[Produces("application/json")]
[Route("api/[controller]")]
public class RoleController(IMediator mediator, ICurrentUserService currentUserService, IUserTokenService userTokenService, IGrainFactory factory) : InitController(mediator, currentUserService, userTokenService, factory)
{
    [HttpGet, Route("{organizationid}"), CheckRole(FunctionType.ReadAuthorization)]
    public async Task<IActionResult> GetOrganiztionRoles(Guid organizationid) => Ok(await _mediator.Send(new GetOrganizationRolesQuery(organizationid)));
}