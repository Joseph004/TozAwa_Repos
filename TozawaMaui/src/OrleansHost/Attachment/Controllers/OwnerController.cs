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
using Grains.Models;

namespace OrleansHost.Attachment.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [EnableCors("TozAwaCorsPolicyBff")]
    [Produces("application/json")]
    [Route("api/[controller]")]

    public class OwnerController(IMediator mediator, Grains.Auth.Services.ICurrentUserService currentUserService, IUserTokenService userTokenService, IGrainFactory factory) : InitController(mediator, currentUserService, userTokenService, factory)
    {
        [HttpGet, Route("{fromOwnerId}/copyTo/{toOwnerId}"), CheckRole(FunctionType.WriteAttachment, FunctionType.WriteAdmin, FunctionType.WriteLandLoard)]
        public async Task<IActionResult> AddCopy(Guid fromOwnerId, Guid toOwnerId) =>
            Ok(await _mediator.Send(new CopyOwnerAttachmentsCommand { FromOwnerId = fromOwnerId, ToOwnerId = toOwnerId }));
    }
}