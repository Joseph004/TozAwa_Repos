﻿using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Tozawa.Language.Svc.Models.Enums;
using Tozawa.Language.Svc.Services;

namespace Tozawa.Language.Svc.Controllers.TranslationSummaryControllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [EnableCors("TozAwaCorsPolicyBff")]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class TranslationSummaryController : InitController
    {
        public TranslationSummaryController(IMediator mediator, ICurrentUserService currentUserService, IUserTokenService userTokenService)
       : base(mediator, currentUserService, userTokenService)
        {
        }

        [HttpGet, Route("paged"), CheckRole(FunctionType.ReadLanguage)]
        public async Task<IActionResult> GetTranslationSummaryPaged()
        => Ok(await _mediator.Send(new GetTranslationSummaryPagedQuery(Request.QueryString.HasValue ? QueryHelpers.ParseQuery(Request.QueryString.Value) : null)));
    }
}
