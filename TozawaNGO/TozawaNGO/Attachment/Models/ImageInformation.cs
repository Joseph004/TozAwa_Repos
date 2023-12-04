namespace TozawaNGO.Attachment.Models
{
    [Obsolete("This is only used for convertion of old data. Will be removed in near feature.")]
    public class ImageInformation
    {
        public Guid Id { get; set; }
        public virtual FileAttachment FileAttachment { get; set; }
        public Guid FileAttachmentId { get;set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public Guid EditorBlobId { get; set; }
        public List<ImageArea> Areas { get; set; }

        public ImageInformation Clone()
        {
            return (ImageInformation) MemberwiseClone();
        }
    }
}