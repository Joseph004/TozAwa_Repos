﻿using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Tozawa.Language.Svc.Models.Enums;
using Tozawa.Language.Svc.Services;

namespace Tozawa.Language.Svc.Controllers.ImportControllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [EnableCors("TozAwaCorsPolicyBff")]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ImportController : InitController
    {
        public ImportController(IMediator mediator, ICurrentUserService currentUserService, IUserTokenService userTokenService)
            : base(mediator, currentUserService, userTokenService)
        {
        }


        // Not used by Tozawa.Language Client
        [HttpPost, Route(""), CheckRole(FunctionType.WriteLanguage)]
        public async Task<IActionResult> Import([FromBody] ImportTranslationCommand request) => Ok(await _mediator.Send(request));
    }
}