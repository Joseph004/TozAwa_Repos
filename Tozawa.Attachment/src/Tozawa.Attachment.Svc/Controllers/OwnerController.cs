﻿using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tozawa.Attachment.Svc.Context;
using Tozawa.Attachment.Svc.Models.Commands;
using Tozawa.Attachment.Svc.Services;

namespace Tozawa.Attachment.Svc.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/[controller]")]

    public class OwnerController : InitController
    {
        public OwnerController(IMediator mediator, ICurrentUserService currentUserService) : base(mediator, currentUserService)
        {
        }

        [HttpGet, Route("{fromOwnerId}/copyTo/{toOwnerId}"), CheckRole(FunctionType.WriteImmovable, FunctionType.WriteTravling, FunctionType.WriteActivity)]
        public async Task<IActionResult> AddCopy(Guid fromOwnerId, Guid toOwnerId) =>
            Ok(await _mediator.Send(new CopyOwnerAttachmentsCommand { FromOwnerId = fromOwnerId, ToOwnerId = toOwnerId }));
    }
}