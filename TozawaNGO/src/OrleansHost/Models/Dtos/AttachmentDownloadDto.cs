
using System;

namespace OrleansHost.Models.Dtos
{
    public class AttachmentDownloadDto
    {
        public string Name { get; set; }
        public byte[] Content { get; set; }
        public string MineType { get; set; }
    }
}
