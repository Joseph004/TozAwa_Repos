using MediatR;
using Microsoft.AspNetCore.Http;
using TozawaNGO.Attachment.Models.Dtos;

namespace TozawaNGO.Attachment.Models.Commands
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