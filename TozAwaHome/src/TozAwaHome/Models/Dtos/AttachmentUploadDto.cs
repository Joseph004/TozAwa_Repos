
using System;

namespace TozAwaHome.Models.Dtos
{
    public class AttachmentUploadDto
    {
        public string Name { get; set; } = "";
        public byte[] Content { get; set; } = Array.Empty<byte>();
        public string ContentType { get; set; } = "";
    }
}
