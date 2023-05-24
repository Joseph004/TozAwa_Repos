using MediatR;
using Microsoft.AspNetCore.Http;
using Tozawa.Attachment.Svc.Models.Dtos;

namespace Tozawa.Attachment.Svc.Models.Commands
{
    public class AddBlobCommand : IRequest<AddBlobResponse>
    {
        public AddBlobCommand(IFormFile file)
        {
            File = file;
        }
        public IFormFile File { get; set; }
    }
}