
using Microsoft.AspNetCore.Components.Forms;
using TozawaMauiHybrid.Models.Dtos;
using TozawaMauiHybrid.Models.Enums;
using TozawaMauiHybrid.Helpers;

namespace TozawaMauiHybrid.Models.FormModels
{
    public class AttachmentUploadRequest
    {
        public List<AttachmentUploadDto> Files { get; set; } = [];
        public AttachmentType FileAttachmentType { get; set; }
        public string FolderName { get; set; }
        public string Source { get; set; }

        public AttachmentUploadRequest()
        {
        }
        public async Task<byte[]> GetBytes(IBrowserFile file)
        {
            await using Stream stream = file.OpenReadStream(FileValidator.MaxAllowedSize);
            await using MemoryStream ms = new(100 * 1024 * 1024);
            await stream.CopyToAsync(ms);
            stream.Close();
            return ms.ToArray();
        }
        public void AddFiles(List<(byte[] bytes, string type, string name)> files)
        {
            foreach (var (bytes, type, name) in files)
            {
                Files.Add(new AttachmentUploadDto
                {
                    ContentType = type,
                    Content = bytes,
                    Name = name
                });
            }
        }
    }
}