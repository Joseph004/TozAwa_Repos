using MediatR;
using Microsoft.AspNetCore.Http;

namespace Tozawa.Bff.Portal.Controllers
{
    public class ConvertImageToPngCommand : IRequest< byte[]>
    {
        public ConvertImageToPngCommand(IFormFile file)
        {
            File = file;
        }
        public IFormFile File { get; set; }
    }
}