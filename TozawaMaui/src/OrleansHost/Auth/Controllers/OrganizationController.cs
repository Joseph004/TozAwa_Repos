using Grains.Auth.Services;
using Grains.Helpers;
using Grains.Models;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using OrleansHost.Auth.Models.Commands;
using OrleansHost.Auth.Models.Queries;

namespace Grains.Auth.Controllers;

[AuthorizeUserRequirement]
[Produces("application/json")]
[Route("api/[controller]")]
public class OrganizationController(IMediator mediator, ICurrentUserService currentUserService, IUserTokenService userTokenService, IGrainFactory factory) : InitController(mediator, currentUserService, userTokenService, factory)
{
    [HttpGet, Route(""), CheckRole(FunctionType.ReadAuthorization)]
    public async Task<IActionResult> GetOrganizations() => Ok(await _mediator.Send(new GetOrganizationsQuery(Request.QueryString.HasValue ? QueryHelpers.ParseQuery(Request.QueryString.Value) : null)));
    [HttpGet, Route("{id}"), CheckRole(FunctionType.ReadAuthorization)]
    public async Task<IActionResult> Get(Guid id) => Ok(await _mediator.Send(new GetOrganizationQuery { Id = id }));

    [HttpPost, Route(""), OnlyAdmin]
    public async Task<IActionResult> Create([FromBody] CreateOrganizationCommand request) => Ok(await _mediator.Send(request));

    [HttpPatch, Route("{id}"), OnlyAdmin]
    public async Task<IActionResult> PatchOrganization(Guid id, [FromBody] JsonPatchDocument patchModel) => Ok(await _mediator.Send(new PatchOrganizationCommand
    {
        Id = id,
        PatchModel = patchModel
    }));
}