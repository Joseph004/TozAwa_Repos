using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using Tozawa.Bff.Portal.ClientMessages;
using Tozawa.Bff.Portal.Configuration;
using Tozawa.Bff.Portal.Models.Enums;
using Tozawa.Bff.Portal.Services;

namespace Tozawa.Bff.Portal.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TestController : ControllerBase
    {
        public TestController(AppSettings appSettings)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Get() => Ok(new List<int> { 1, 2, 3 });
    }
}