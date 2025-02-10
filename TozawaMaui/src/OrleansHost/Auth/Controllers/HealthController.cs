using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Grains.Auth.Controllers;

[ApiController]
[Produces("application/json")]
[Route("api/[controller]")]
[AllowAnonymous]
public class HealthController : Controller
{
    [HttpGet, Route("")]
    public string Get() => "helth";
}