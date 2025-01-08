

using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Grains.Auth.Services;
using Grains.Helpers;
using Grains.Models;

namespace Grains.Auth.Controllers
{
    [AuthorizeUserRequirement]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class MemberController(IMediator mediator, ICurrentUserService currentUserService, IUserTokenService userTokenService, IGrainFactory factory) : InitController(mediator, currentUserService, userTokenService, factory)
    {
        [HttpGet, Route(""), CheckRole(FunctionType.ReadAuthorization)]
        public async Task<IActionResult> Get() => Ok(await _mediator.Send(new GetMembersQuery(Request.QueryString.HasValue ? QueryHelpers.ParseQuery(Request.QueryString.Value) : null)));

        [HttpGet, Route("members"), CheckRole(FunctionType.ReadLandLoard)]
        public async Task<IActionResult> GetMembers() => Ok(await _mediator.Send(new GetMembersQuery(Request.QueryString.HasValue ? QueryHelpers.ParseQuery(Request.QueryString.Value) : null, false)));

        [HttpGet, Route("{id}"), CheckRole(FunctionType.ReadAuthorization)]
        public async Task<IActionResult> Get(Guid id) => Ok(await _mediator.Send(new GetMemberQuery { Id = id }));

        [HttpPatch, Route("{id}"), CheckRole(FunctionType.ReadAuthorization, FunctionType.WriteAuthorization)]
        public async Task<IActionResult> PatchMember(Guid id, [FromBody] JsonPatchDocument patchModel) => Ok(await _mediator.Send(new PatchMemberCommand
        {
            Id = id,
            PatchModel = patchModel
        }));

        [HttpPost, Route(""), CheckRole(FunctionType.ReadAuthorization, FunctionType.WriteAuthorization)]
        public async Task<IActionResult> Post([FromBody] CreateMemberCommand request) => Ok(await _mediator.Send(request));

        [HttpPost, Route("reset"), CheckRole(FunctionType.WriteAuthorization)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetUserPasswordCommand request) => Ok(await _mediator.Send(request));

        [HttpPost, Route("changepassword"), CheckRole(FunctionType.WriteAuthorization)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand request) => Ok(await _mediator.Send(request));
    }
}