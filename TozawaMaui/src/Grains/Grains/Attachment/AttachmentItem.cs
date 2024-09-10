using Grains.Helpers;
using Grains.Models.Enums;

namespace Grains
{
    [GenerateSerializer]
    [Immutable]
    public class AttachmentItem : IEquatable<AttachmentItem>
    {
        public AttachmentItem(
        Guid id,
        DateTime createdDate,
         DateTime? modifiedDate,
       string modifiedBy,
       string createdBy,
        Guid ownerId,
        string blobId,
        string miniatureId,
       string name,
       string extension,
         string mimeType,
      double size,
       string attachmentType,
        AttachmentType fileAttachmentType,
        string metaData,
        List<Guid> ownerIds,
        string thumbnail,
        string miniatureBlobUrl
        )
            : this(
             id,
        createdDate,
         modifiedDate,
       modifiedBy,
       createdBy,
        ownerId,
        blobId,
        miniatureId,
      name,
       extension,
         mimeType,
      size,
       attachmentType,
        fileAttachmentType,
        metaData,
        ownerIds,
        thumbnail,
        miniatureBlobUrl,
        DateTime.UtcNow)
        {
        }

        protected AttachmentItem(
            Guid id,
        DateTime createdDate,
         DateTime? modifiedDate,
       string modifiedBy,
       string createdBy,
        Guid ownerId,
        string blobId,
        string miniatureId,
       string name,
       string extension,
         string mimeType,
      double size,
       string attachmentType,
        AttachmentType fileAttachmentType,
        string metaData,
        List<Guid> ownerIds,
        string thumbnail,
        string miniatureBlobUrl,
         DateTime timestamp)
        {
            Id = id;
            CreatedDate = createdDate;
            ModifiedDate = modifiedDate;
            ModifiedBy = modifiedBy;
            CreatedBy = createdBy;
            OwnerId = SystemTextId.AttachmentOwnerId;
            BlobId = blobId;
            MiniatureId = miniatureId;
            Name = name;
            Extension = extension;
            MimeType = mimeType;
            Size = size;
            AttachmentType = attachmentType;
            FileAttachmentType = fileAttachmentType;
            MetaData = metaData;
            OwnerIds = ownerIds;
            Thumbnail = thumbnail;
            MiniatureBlobUrl = miniatureBlobUrl;
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
        public List<Guid> OwnerIds { get; }
        [Id(16)]
        public string Thumbnail { get; }
        [Id(17)]
        public string MiniatureBlobUrl { get; }
        [Id(18)]
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
           && Thumbnail == item.Thumbnail
           && MiniatureBlobUrl == item.MiniatureBlobUrl
           && Timestamp == item.Timestamp;
        }
    }
}