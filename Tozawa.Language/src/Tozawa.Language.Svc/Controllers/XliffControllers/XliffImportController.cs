using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Tozawa.Language.Svc.extension;
using Tozawa.Language.Svc.Models.Enums;
using Tozawa.Language.Svc.Services;

namespace Tozawa.Language.Svc.Controllers.XliffControllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [EnableCors("TozAwaCorsPolicyBff")]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class XliffImportController : InitController
    {
        public XliffImportController(IMediator mediator, ICurrentUserService currentUserService, IUserTokenService userTokenService)
             : base(mediator, currentUserService, userTokenService)
        {
        }


        [HttpGet, Route("{filename}"), CheckRole(FunctionType.ReadLanguage)]
        public async Task<IActionResult> GetByFileName(string filename)
        {
            return Ok(await _mediator.Send(new GetXliffFileByNameQuery(filename, XliffFileType.Import)));
        }

        [HttpPost, Route(""), CheckRole(FunctionType.WriteLanguage)]
        public async Task<IActionResult> Post(IFormFile file)
        {
            await _mediator.Send(new ImportXliffFilesCommand { Files = new List<IFormFile> { file } });
            return Ok();
        }
    }
}