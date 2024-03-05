using System;

namespace OrleansHost.Attachment.Models.Dtos
{
    public class BaseDto
    {
        public Guid Id { get; set; }
        public bool Deleted { get; set; }
        public string Code { get; set; } = "";
    }
}