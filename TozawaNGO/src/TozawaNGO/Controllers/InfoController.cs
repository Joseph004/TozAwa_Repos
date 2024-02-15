using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Orleans;

namespace TozawaNGO.Controllers
{
    // The home controller generates the initial home page, as wel as the aspnet-javascript serverside fallback pages (mostly for seo)
    public class InfoController : Controller
    {
        readonly IClusterClient clusterClient;
        readonly IHostEnvironment env;
        readonly IConfiguration config;

        public InfoController(IClusterClient clusterClient, IHostEnvironment env, IConfiguration config)
        {
            this.clusterClient = clusterClient;
            this.env = env;
            this.config = config;
        }

        [HttpGet, Route("~/info")]
        public IActionResult Index()
        {
            return Content($"TozawaNGO is alive. Id = {config["Id"]}, Version = {config["Version"]}");
            // todo connect to orleanshost and get its version and Id
        }
    }
}