
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;
using TozawaNGO.Models.Dtos;
using TozawaNGO.Models.Enums;
using System;

namespace TozawaNGO.Models.FormModels
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
            try
            {
                foreach (var file in files)
                {
                    await using Stream stream = file.OpenReadStream(file.Size);
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
            catch (Exception ex)
            {

            }
        }
    }
}