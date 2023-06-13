using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Tozawa.Language.Svc.Models.Enums;
using Tozawa.Language.Svc.Services;

namespace Tozawa.Language.Svc.Controllers.XliffControllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [EnableCors("TozAwaCorsPolicyBff")]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class XliffExportController : InitController
    {
        public XliffExportController(IMediator mediator, ICurrentUserService currentUserService, IUserTokenService userTokenService)
             : base(mediator, currentUserService, userTokenService)
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
