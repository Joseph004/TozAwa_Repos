using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using OrleansHost.Auth.Controllers;
using System.Security.Cryptography;

namespace Grains.Auth.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]

    [ApiController]
    public class TzKeyController() : ControllerBase
    {

        [HttpGet, Route("")]
        public async Task<ActionResult> GetKey()
        {
            var assembly = Directory.GetCurrentDirectory();
            string path = Path.Combine(assembly, @"Auth\Controllers\mycert.pem");

            byte[] fileBytes;
            List<byte> totalStream = [];
            using (FileStream source = System.IO.File.OpenRead(path))
            {
                byte[] buffer = new byte[2048];
                int bytesRead;
                while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
                {
                    totalStream.AddRange(buffer.Take(bytesRead));
                }
            }
            fileBytes = [.. totalStream];
            var key = "~pg:K5;>L^/;j=xy[1ut]zlsp0[5'#p>";
            var iv = "ymUGsm9mI57fc5Xr";
            var encrypted = Cryptography.Encrypt(fileBytes, key, iv);

            return await Task.FromResult(Ok(encrypted));
        }
    }
}