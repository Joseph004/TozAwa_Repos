using Grains.Auth.Services;
using Grains.Helpers;
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
public class FeatureController(IMediator mediator, ICurrentUserService currentUserService, IUserTokenService userTokenService, IGrainFactory factory) : InitController(mediator, currentUserService, userTokenService, factory)
{
    [HttpPost, Route(""), OnlyAdmin]
    public async Task<IActionResult> CreateFeature([FromBody] CreateFeatureCommand request) => Ok(await _mediator.Send(request));

    [HttpGet, Route(""), OnlyAdmin]
    public async Task<IActionResult> GetFeatures() => Ok(await _mediator.Send(new GetFeaturesQuery(Request.QueryString.HasValue ? QueryHelpers.ParseQuery(Request.QueryString.Value) : null)));

    [HttpPatch, Route("{id}"), OnlyAdmin]
    public async Task<IActionResult> PatchFeature(int id, [FromBody] JsonPatchDocument patchModel) => Ok(await _mediator.Send(new PatchFeatureCommand
    {
        Id = id,
        PatchModel = patchModel
    }));
}