using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tozawa.Bff.Portal.ClientMessages;
using Tozawa.Bff.Portal.Configuration;
using Tozawa.Bff.Portal.Models.Enums;
using Tozawa.Bff.Portal.Services;

namespace Tozawa.Bff.Portal.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ObjectTextController : InitController
    {
        public ObjectTextController(IMediator mediator, ICurrentUserService currentUserService, ILanguageService LanguageService, AppSettings appSettings)
        : base(mediator, currentUserService)
        {
            UpdateMessages.Configure(LanguageService, appSettings, currentUserService);
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