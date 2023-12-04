using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using TozawaNGO.Attachment.Models.Commands;
using TozawaNGO.Auth.Controllers;
using TozawaNGO.Auth.Models.Dtos;
using TozawaNGO.Auth.Services;
using TozawaNGO.Context;
using TozawaNGO.Services;

namespace TozawaNGO.Attachment.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [EnableCors("TozAwaCorsPolicyBff")]
    [Produces("application/json")]
    [Route("api/[controller]")]

    public class OwnerController : InitController
    {
        public OwnerController(IMediator mediator, TozawaNGO.Auth.Services.ICurrentUserService currentUserService, IUserTokenService userTokenService) : base(mediator, currentUserService, userTokenService)
        {
        }

        [HttpGet, Route("{fromOwnerId}/copyTo/{toOwnerId}"), CheckRole(RoleDto.President, RoleDto.VicePresident)]
        public async Task<IActionResult> AddCopy(Guid fromOwnerId, Guid toOwnerId) =>
            Ok(await _mediator.Send(new CopyOwnerAttachmentsCommand { FromOwnerId = fromOwnerId, ToOwnerId = toOwnerId }));
    }
}