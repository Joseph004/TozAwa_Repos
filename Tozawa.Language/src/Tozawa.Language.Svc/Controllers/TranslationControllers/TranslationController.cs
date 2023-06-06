using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tozawa.Language.Svc.extension;
using Tozawa.Language.Svc.Models.Enums;
using Tozawa.Language.Svc.Services;

namespace Tozawa.Language.Svc.Controllers.TranslationControllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class TranslationController : InitController
    {
        public TranslationController(IMediator mediator, ICurrentUserService currentUserService)
        : base(mediator, currentUserService)
        {
        }

        [HttpPost, Route("add"), CheckRole(FunctionType.WriteLanguage)]
        public async Task<IActionResult> AddText([FromBody] AddTextCommand request) => Ok(await _mediator.Send(request));

        [HttpPut, Route("update"), CheckRole(FunctionType.WriteLanguage)]
        public async Task<IActionResult> UpdateText([FromBody] UpdateTextCommand request) => Ok(await _mediator.Send(request));

        [HttpPut, Route("batchupdate"), CheckRole(FunctionType.WriteLanguage)]
        public async Task<IActionResult> UpdateText([FromBody] BatchUpdateTextCommand request) => Ok(await _mediator.Send(request));

        [HttpDelete, Route("{id}"), CheckRole(FunctionType.WriteLanguage)]
        public async Task<IActionResult> Delete(Guid id) => Ok(await _mediator.Send(new DeleteTranslationCommand(id)));

        [HttpPut, Route("AddDeleteMarking"), CheckRole(FunctionType.WriteLanguage)]
        public async Task<IActionResult> AddDeleteMark([FromBody] UpdateTranslationDeletedCommand request) => Ok(await _mediator.Send(request));

        [HttpPut, Route("RemoveDeleteMarking/{translationId}"), CheckRole(FunctionType.WriteLanguage)]
        public async Task<IActionResult> RemoveDeleteMark(Guid translationId) => Ok(await _mediator.Send(new UpdateTranslationDeletedCommand(translationId, false)));

        // Not used by Tozawa.Language Client

        [HttpGet, Route("GetBySystemTypeLanguage/{systemTypeId:Guid}/{languageId:Guid}")]
        public async Task<IActionResult> GetBySystemTypeLanguage(Guid systemTypeId, Guid languageId) => Ok(await _mediator.Send(new GetFilteredTranslations(systemTypeId, languageId)));
    }
}