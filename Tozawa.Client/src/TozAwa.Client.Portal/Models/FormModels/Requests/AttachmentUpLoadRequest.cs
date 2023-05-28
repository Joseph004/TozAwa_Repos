
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;
using Tozawa.Client.Portal.Models.Dtos;
using Tozawa.Client.Portal.Models.Enums;

namespace Tozawa.Client.Portal.Models.FormModels
{
    public class AttachmentUploadRequest
    {
        public List<AttachmentUploadDto> Files { get; set; } = new();
        public AttachmentType? FileAttachmentType { get; set; }
        public string FolderName { get; set; }

        public AttachmentUploadRequest()
        {
        }

        public async Task AddFiles(List<IBrowserFile> files)
        {
            foreach (var file in files)
            {
                Stream stream = file.OpenReadStream(file.Size);
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