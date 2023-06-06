using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Identity.Web.Resource;
using Tozawa.Bff.Portal.ClientMessages;
using Tozawa.Bff.Portal.Configuration;
using Tozawa.Bff.Portal.Models;
using Tozawa.Bff.Portal.Models.Enums;
using Tozawa.Bff.Portal.Services;

namespace Tozawa.Bff.Portal.Controllers
{
    //[Authorize(AuthenticationSchemes = "tzappauthentication")]
    [Authorize(AuthenticationSchemes = "tzuserauthentication")]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class MemberController : InitController
    {
        public MemberController(IMediator mediator, ICurrentUserService currentUserService, ILanguageService LanguageService, AppSettings appSettings)
        : base(mediator, currentUserService)
        {
            UpdateMessages.Configure(LanguageService, appSettings, currentUserService);
        }

        [HttpGet, Route(""), CheckRole(FunctionType.ReadSetting, FunctionType.ReadMember)]
        public async Task<IActionResult> GetMembers()
        {
            return Ok(await _mediator.Send(new GetMembersQuery(Request.QueryString.HasValue ? QueryHelpers.ParseQuery(Request.QueryString.Value) : null)));
        }

        [HttpPut, Route("{id}"), CheckRole(FunctionType.WriteSetting, FunctionType.WriteMember)]
        public async Task<IActionResult> PatchMember(Guid id, [FromBody] PatchMemberCommand request)
        {
            request.Id = id;
            return Ok(await _mediator.Send(request));
        }

        [HttpPost, Route(""), CheckRole(FunctionType.WriteSetting, FunctionType.WriteMember)]
        public async Task<IActionResult> AddMember([FromBody] AddMemberCommand request)
        {
            return Ok(await _mediator.Send(request));
        }
    }
}