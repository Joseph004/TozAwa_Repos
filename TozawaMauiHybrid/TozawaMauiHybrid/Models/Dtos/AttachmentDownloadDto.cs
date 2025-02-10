
using System;

namespace TozawaMauiHybrid.Models.Dtos
{
    public class AttachmentDownloadDto
    {
        public string Name { get; set; }
        public byte[] Content { get; set; }
        public string MineType { get; set; }
        public byte[] PdfConvertedContent { get; set; }
    }
}
