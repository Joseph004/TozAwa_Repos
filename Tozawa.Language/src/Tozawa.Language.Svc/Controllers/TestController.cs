using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Tozawa.Language.Svc.Controllers.LanguageControllers
{
    //[AllowAnonymous]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class TestController : Controller
    {
        public TestController()
        {
        }

        [HttpGet, Route("")]
        public async Task<IActionResult> Get() => Ok("Hejsan");

    }
}
