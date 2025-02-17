using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using Tozawa.Bff.Portal.ClientMessages;
using Tozawa.Bff.Portal.Configuration;
using Tozawa.Bff.Portal.Models.Enums;
using Tozawa.Bff.Portal.Services;

namespace Tozawa.Bff.Portal.Controllers
{
    //[Authorize(AuthenticationSchemes = "tzappauthentication")]
    [EnableCors("TozAwaCorsPolicyBff")]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ObjectTextController : InitController
    {
        public ObjectTextController(IMediator mediator, ICurrentUserService currentUserService, IUserTokenService userTokenService, ILanguageService languageService, AppSettings appSettings)
        : base(mediator, currentUserService, userTokenService)
        {
            UpdateMessages.Configure(languageService, appSettings, currentUserService);
        }

        [HttpPut, Route(""), CheckRole(FunctionType.WriteMember, FunctionType.WriteActivity, FunctionType.WritePayment)]
        public async Task<IActionResult> Update([FromBody] UpdateObjectTextCommand request)
        {
            return Ok(await _mediator.Send(request));
        }

        [HttpPost, Route(""), CheckRole(FunctionType.ReadMember, FunctionType.ReadActivity, FunctionType.ReadPayment)]
        public async Task<IActionResult> CheckObjectText([FromBody] CheckObjectTextCommand request)
        {
            return Ok(await _mediator.Send(request));
        }
    }
}