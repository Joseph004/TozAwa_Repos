using MediatR;
using Microsoft.AspNetCore.Http;

namespace TozawaNGO.Attachment.Models.Commands
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