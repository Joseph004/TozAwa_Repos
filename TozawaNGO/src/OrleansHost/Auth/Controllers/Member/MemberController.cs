

using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Grains.Auth.Models.Dtos;
using Grains.Auth.Services;
using Grains.Helpers;

namespace Grains.Auth.Controllers
{
    [AuthorizeUserRequirement]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class MemberController(IMediator mediator, ICurrentUserService currentUserService, IUserTokenService userTokenService) : InitController(mediator, currentUserService, userTokenService)
    {
        [HttpGet, Route(""), CheckRole(RoleDto.President, RoleDto.VicePresident)]
        public async Task<IActionResult> Get() => Ok(await _mediator.Send(new GetMembersQuery(Request.QueryString.HasValue ? QueryHelpers.ParseQuery(Request.QueryString.Value) : null)));

        [HttpPatch, Route("{id}"), CheckRole(RoleDto.President, RoleDto.VicePresident)]
        public async Task<IActionResult> PatchMember(Guid id, [FromBody] JsonPatchDocument patchModel) => Ok(await _mediator.Send(new PatchMemberCommand
        {
            Id = id,
            PatchModel = patchModel
        }));

        [HttpPost, Route(""), CheckRole(RoleDto.President, RoleDto.VicePresident)]
        public async Task<IActionResult> Post([FromBody] CreateMemberCommand request) => Ok(await _mediator.Send(request));
    }
}