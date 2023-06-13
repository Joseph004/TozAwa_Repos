using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Identity.Web.Resource;
using Tozawa.Bff.Portal.ClientMessages;
using Tozawa.Bff.Portal.Configuration;
using Tozawa.Bff.Portal.Models;
using Tozawa.Bff.Portal.Services;

namespace Tozawa.Bff.Portal.Controllers
{
    //[Authorize(AuthenticationSchemes = "tzappauthentication")] 
    [EnableCors("TozAwaCorsPolicyBff")]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TranslationController : InitController
    {
        public TranslationController(IMediator mediator, ICurrentUserService currentUserService, IUserTokenService userTokenService, ILanguageService languageService, AppSettings appSettings)
        : base(mediator, currentUserService, userTokenService)
        {
            UpdateMessages.Configure(languageService, appSettings, currentUserService);
        }

        [HttpGet, Route("systemtexts/{languageId}")]
        public async Task<IActionResult> GetSystemText(Guid languageId) => Ok(await _mediator.Send(new GetSystemTextsQuery { LanguageId = languageId }));

        [HttpGet, Route("activelanguages")]
        public async Task<IActionResult> GetActiveLanguages() => Ok(await _mediator.Send(new GetActiveLanguagesQuery()));
    }
}