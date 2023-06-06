using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Tozawa.Language.Svc.extension;
using Tozawa.Language.Svc.Models.Enums;
using Tozawa.Language.Svc.Services;

namespace Tozawa.Language.Svc.Controllers.XliffControllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class XliffDistributionFilesLogsController : InitController
    {
        public XliffDistributionFilesLogsController(IMediator mediator, ICurrentUserService currentUserService)
      : base(mediator, currentUserService)
        {
        }

        [HttpGet, Route("paged"), CheckRole(FunctionType.ReadLanguage)]
        public async Task<IActionResult> GetPaged()
        {
            var query = new GetXliffDistributionFilesLogsQueryPaged(
                Request.QueryString.HasValue
                ? QueryHelpers.ParseQuery(Request.QueryString.Value)
                : null);
            return Ok(await _mediator.Send(query));
        }
    }
}
