using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using OrleansHost.Attachment.Models.Commands;
using Grains.Auth.Controllers;
using Grains.Auth.Models.Dtos;
using Grains.Auth.Services;
using Grains.Context;
using Grains.Services;

namespace OrleansHost.Attachment.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [EnableCors("TozAwaCorsPolicyBff")]
    [Produces("application/json")]
    [Route("api/[controller]")]

    public class OwnerController(IMediator mediator, Grains.Auth.Services.ICurrentUserService currentUserService, IUserTokenService userTokenService) : InitController(mediator, currentUserService, userTokenService)
    {
        [HttpGet, Route("{fromOwnerId}/copyTo/{toOwnerId}"), CheckRole(RoleDto.President, RoleDto.VicePresident)]
        public async Task<IActionResult> AddCopy(Guid fromOwnerId, Guid toOwnerId) =>
            Ok(await _mediator.Send(new CopyOwnerAttachmentsCommand { FromOwnerId = fromOwnerId, ToOwnerId = toOwnerId }));
    }
}