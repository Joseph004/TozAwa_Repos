using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tozawa.Language.Svc.extension;
using Tozawa.Language.Svc.Models.Enums;
using Tozawa.Language.Svc.Services;

namespace Tozawa.Language.Svc.Controllers.XliffControllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class XliffExportController : InitController
    {
        public XliffExportController(IMediator mediator, ICurrentUserService currentUserService)
             : base(mediator, currentUserService)
        {
        }


        [HttpGet, Route("{sourceLanguageId}/{targetLanguageId}/{systemTypeId}/{fileName}"), CheckRole(FunctionType.ReadLanguage)]
        public async Task<IActionResult> Get(Guid sourceLanguageId, Guid targetLanguageId, Guid systemTypeId, string fileName)
        {
            return Ok(await _mediator.Send(new GetXliffFileQuery(sourceLanguageId, targetLanguageId, systemTypeId, fileName)));
        }

        [HttpGet, Route("{filename}"), CheckRole(FunctionType.ReadLanguage)]
        public async Task<IActionResult> GetByFileName(string filename)
        {
            return Ok(await _mediator.Send(new GetXliffFileByNameQuery(filename, XliffFileType.Export)));
        }
    }
}
