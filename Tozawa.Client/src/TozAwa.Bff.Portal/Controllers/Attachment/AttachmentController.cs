

using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tozawa.Bff.Portal.ClientMessages;
using Tozawa.Bff.Portal.Configuration;
using Tozawa.Bff.Portal.Services;

namespace Tozawa.Bff.Portal.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AttachmentController : InitController
    {
        public AttachmentController(IMediator mediator, ICurrentUserService currentUserService, ILanguageService LanguageService, AppSettings appSettings)
        : base(mediator, currentUserService)
        {
            UpdateMessages.Configure(LanguageService, appSettings, currentUserService);
        }

        [HttpGet, Route("{id}")]
        public async Task<IActionResult> GetAttachment(Guid id) => Ok(await _mediator.Send(new GetAttachmentQuery { Id = id }));

        [HttpDelete, Route("{id}")]
        public async Task<IActionResult> DeleteAttachment(Guid id) => Ok(await _mediator.Send(new DeleteAttachmentCommand { Id = id }));

        [HttpPost, Route("{id}")]
        public async Task<IActionResult> AddAttachment(Guid id, [FromBody] AddAttachmentsCommand request)
        {
            request.Id = id;
            return Ok(await _mediator.Send(request));
        }
    }
}