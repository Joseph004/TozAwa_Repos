using Grains.Attachment.Models.Dtos;
using Grains.Models.Enums;

namespace Grains
{
    [GenerateSerializer]
    [Immutable]
    public class AttachmentItem : IEquatable<AttachmentItem>
    {
        public AttachmentItem(FileAttachmentDto item)
            : this(item, DateTime.UtcNow)
        {
        }

        protected AttachmentItem(FileAttachmentDto item, DateTime timestamp)
        {
            Id = item.Id;
            CreatedDate = item.CreatedDate;
            ModifiedDate = item.ModifiedDate;
            ModifiedBy = item.ModifiedBy;
            CreatedBy = item.CreatedBy;
            OwnerId = item.OwnerIds.First();
            BlobId = item.BlobId;
            MiniatureId = item.MiniatureId;
            Name = item.Name;
            Extension = item.Extension;
            MimeType = item.MimeType;
            Size = item.Size;
            AttachmentType = item.FileAttachmentType;
            MetaData = item.MetaData;
            Timestamp = timestamp;
        }

        [Id(0)]
        public Guid Id { get; }
        [Id(1)]
        public DateTime CreatedDate { get; }
        [Id(2)]
        public DateTime? ModifiedDate { get; }
        [Id(3)]
        public string ModifiedBy { get; }
        [Id(4)]
        public string CreatedBy { get; }
        [Id(5)]
        public Guid OwnerId { get; }
        [Id(6)]
        public string BlobId { get; }
        [Id(7)]
        public string MiniatureId { get; }
        [Id(8)]
        public string Name { get; }
        [Id(9)]
        public string Extension { get; }
        [Id(10)]
        public string MimeType { get; }
        [Id(11)]
        public double Size { get; }
        [Id(12)]
        public string AttachmentType { get; }
        [Id(13)]
        public AttachmentType FileAttachmentType { get; }
        [Id(14)]
        public string MetaData { get; }
        [Id(15)]
        public DateTime Timestamp { get; }

        public bool Equals(AttachmentItem item)
        {
            if (item == null) return false;
            return
            Id == item.Id
           && CreatedDate == item.CreatedDate
           && ModifiedDate == item.ModifiedDate
           && ModifiedBy == item.ModifiedBy
           && CreatedBy == item.CreatedBy
           && OwnerId == item.OwnerId
           && BlobId == item.BlobId
           && MiniatureId == item.MiniatureId
           && Name == item.Name
           && Extension == item.Extension
           && MimeType == item.MimeType
           && Size == item.Size
           && AttachmentType == item.AttachmentType
           && FileAttachmentType == item.FileAttachmentType
           && MetaData == item.MetaData
           && Timestamp == item.Timestamp;
        }
    }
}