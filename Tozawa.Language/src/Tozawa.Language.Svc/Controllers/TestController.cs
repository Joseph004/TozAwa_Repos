/* using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tozawa.Language.Svc.Context;
using Tozawa.Language.Svc.Helper;

namespace Tozawa.Language.Svc.Controllers.LanguageControllers
{
    [AllowAnonymous]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [EnableCors("TozAwaCorsPolicyBff")]
    public class TestController : Controller
    {
        private readonly LanguageContext _context;
        public TestController(LanguageContext context)
        {
            _context = context;
        }

        [HttpGet, Route("")]
        public async Task<IActionResult> Get()
        {
            var translations = await _context.Translations.ToListAsync();

            var fileName = "Translations.json";
            JsonFileUtils.SimpleWrite(translations, fileName);

            return Ok(translations);
        }

    }
}
 */