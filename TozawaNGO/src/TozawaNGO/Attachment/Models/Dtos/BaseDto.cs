using System;

namespace TozawaNGO.Attachment.Models.Dtos
{
    public class BaseDto
    {
        public Guid Id { get; set; }
        public bool Deleted { get; set; }
        public string Code { get; set; } = "";
    }
}