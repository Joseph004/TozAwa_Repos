namespace Grains.Attachment.Models.Dtos
{
    public class OwnerFileAttachmentCountDto
    {
        public Guid OwnerId { get; set; }

        public int TotalCount { get; set; }

        public List<OwnerFileAttachmentTypeCountDto> TypeCounts { get; set; }
    }
    
    public class OwnerFileAttachmentTypeCountDto
    {
        public Guid? FileAttachmentTypeId { get; set; }

        public int Count { get; set; }
    }
}