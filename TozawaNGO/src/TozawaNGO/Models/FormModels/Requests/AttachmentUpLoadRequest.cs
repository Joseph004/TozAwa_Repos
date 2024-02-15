
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;
using TozawaNGO.Models.Dtos;
using TozawaNGO.Models.Enums;
using System;
using TozawaNGO.Helpers;

namespace TozawaNGO.Models.FormModels
{
    public class AttachmentUploadRequest
    {
        public List<AttachmentUploadDto> Files { get; set; } = [];
        public AttachmentType? FileAttachmentType { get; set; }
        public string FolderName { get; set; }

        public AttachmentUploadRequest()
        {
        }

        public async Task AddFiles(List<IBrowserFile> files)
        {
            foreach (var file in files)
            {
                await using Stream stream = file.OpenReadStream(FileValidator.MaxAllowedSize);
                await using MemoryStream ms = new(100 * 1024 * 1024);
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