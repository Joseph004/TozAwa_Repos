
using System;

namespace ShareRazorClassLibrary.Models.Dtos
{
    public class AttachmentUploadDto
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public byte[] Content { get; set; } = [];
        public string ContentType { get; set; } = "";
    }
}
