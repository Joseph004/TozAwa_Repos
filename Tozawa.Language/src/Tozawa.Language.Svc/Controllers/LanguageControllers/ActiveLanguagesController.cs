using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Tozawa.Language.Svc.Models.Enums;
using Tozawa.Language.Svc.Services;

namespace Tozawa.Language.Svc.Controllers.LanguageControllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ActiveLanguagesController : InitController
    {
        public ActiveLanguagesController(IMediator mediator, ICurrentUserService currentUserService)
         : base(mediator, currentUserService)
        {
        }

        [HttpGet, Route("")]
        public async Task<IActionResult> Get() => Ok(await _mediator.Send(new GetLanguagesQuery()));

        [HttpGet, Route("paged")]
        public async Task<IActionResult> GetPaged() => Ok(await _mediator.Send(new GetLanguagesPagedQuery(Request.QueryString.HasValue ? QueryHelpers.ParseQuery(Request.QueryString.Value) : null)));

        [HttpGet, Route("includeDeleted")]
        public async Task<IActionResult> GetIncludingDeleted() => Ok(await _mediator.Send(new GetLanguagesQuery(true)));

        [HttpPost, Route(""), CheckRole(FunctionType.WriteLanguage)]
        public async Task<IActionResult> CreateLanguage([FromBody] CreateLanguageCommand request) => Ok(await _mediator.Send(request));

        [HttpPut, Route(""), CheckRole(FunctionType.WriteLanguage)]
        public async Task<IActionResult> UpdateLanguage([FromBody] UpdateLanguageCommand request) => Ok(await _mediator.Send(request));

        [HttpPut, Route("default"), CheckRole(FunctionType.WriteLanguage)]
        public async Task<IActionResult> SetDefault([FromBody] SetLanguageAsDefaultCommand request) => Ok(await _mediator.Send(request));

        [HttpDelete, Route("{id}")]
        public async Task<IActionResult> Delete(Guid id) => Ok(await _mediator.Send(new DeleteLanguageCommand(id)));

    }
}
