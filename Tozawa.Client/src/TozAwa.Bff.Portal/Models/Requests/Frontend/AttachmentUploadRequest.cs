using Microsoft.AspNetCore.Components.Forms;
using Tozawa.Bff.Portal.Models.Dtos;

namespace Tozawa.Bff.Portal.Models.Request.Frontend
{
    public class AttachmentUploadRequest
    {
        public List<AttachmentUploadDto> Files { get; set; } = new();

        public AttachmentUploadRequest()
        {
        }

        public async Task AddFiles(List<IBrowserFile> files)
        {
            foreach (var file in files)
            {
                long maxSize = 10 * 1024 * 1024;
                Stream stream = file.OpenReadStream(maxSize);
                using MemoryStream ms = new();
                await stream.CopyToAsync(ms);
                stream.Close();
                Files.Add(new AttachmentUploadDto
                {
                    ContentType = file.ContentType,
                    Content = ms.ToArray(),
                    Name = file.Name
                });
            }
        }
    }
}