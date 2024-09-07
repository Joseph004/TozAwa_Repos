using System;

namespace Grains.Attachment.Models.Dtos
{
    public class BaseDto
    {
        public Guid Id { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreatedDate { get; }
        public DateTime? ModifiedDate { get; }
        public string ModifiedBy { get; }
        public string CreatedBy { get; }
        public DateTime Timestamp { get; set; }
    }
}