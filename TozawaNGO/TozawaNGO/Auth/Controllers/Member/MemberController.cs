

using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using TozawaNGO.Auth.Models.Dtos;
using TozawaNGO.Auth.Services;
using TozawaNGO.Helpers;

namespace TozawaNGO.Auth.Controllers
{
    [AuthorizeUserRequirement]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class MemberController : InitController
    {
        public MemberController(IMediator mediator, ICurrentUserService currentUserService, IUserTokenService userTokenService) : base(mediator, currentUserService, userTokenService)
        {
        }

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