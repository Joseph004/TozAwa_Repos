using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Tozawa.Bff.Portal.ClientMessages;
using Tozawa.Bff.Portal.Configuration;
using Tozawa.Bff.Portal.Models;
using Tozawa.Bff.Portal.Services;

namespace Tozawa.Bff.Portal.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TranslationController : InitController
    {
        public TranslationController(IMediator mediator, ICurrentUserService currentUserService, ILanguageService LanguageService, AppSettings appSettings)
        : base(mediator, currentUserService)
        {
            UpdateMessages.Configure(LanguageService, appSettings, currentUserService);
        }

        [HttpGet, Route("systemtexts/{languageId}")]
        public async Task<IActionResult> GetSystemText(Guid languageId) => Ok(await _mediator.Send(new GetSystemTextsQuery { LanguageId = languageId }));

        [HttpGet, Route("activelanguages")]
        public async Task<IActionResult> GetActiveLanguages() => Ok(await _mediator.Send(new GetActiveLanguagesQuery(Request.QueryString.HasValue ? QueryHelpers.ParseQuery(Request.QueryString.Value) : null)));
    }
}