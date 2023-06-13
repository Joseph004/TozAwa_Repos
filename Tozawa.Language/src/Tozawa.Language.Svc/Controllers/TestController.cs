using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Tozawa.Language.Svc.Controllers.LanguageControllers
{
    //[AllowAnonymous]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [EnableCors("TozAwaCorsPolicyBff")]
    public class TestController : Controller
    {
        public TestController()
        {
        }

        [HttpGet, Route("")]
        public async Task<IActionResult> Get() => Ok("Hejsan");

    }
}
