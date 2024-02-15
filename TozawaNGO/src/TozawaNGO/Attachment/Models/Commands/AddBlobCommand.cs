using MediatR;
using Microsoft.AspNetCore.Http;
using TozawaNGO.Attachment.Models.Dtos;

namespace TozawaNGO.Attachment.Models.Commands
{
    public class AddBlobCommand(IFormFile file) : IRequest<AddBlobResponse>
    {
        public IFormFile File { get; set; } = file;
    }
}