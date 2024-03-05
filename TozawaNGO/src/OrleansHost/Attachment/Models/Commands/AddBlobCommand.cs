using MediatR;
using Microsoft.AspNetCore.Http;
using OrleansHost.Attachment.Models.Dtos;

namespace OrleansHost.Attachment.Models.Commands
{
    public class AddBlobCommand(IFormFile file) : IRequest<AddBlobResponse>
    {
        public IFormFile File { get; set; } = file;
    }
}