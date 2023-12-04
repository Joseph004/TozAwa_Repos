using TozawaNGO.Attachment.Types;

namespace TozawaNGO.Attachment.Models
{
    [Obsolete("This is only used for convertion of old data. Will be removed in near feature.")]
    public class ImageArea {
        public Guid Id { get; set; }

        public Guid DataId { get; set; }
        public virtual ImageInformation ImageInformation { get; set; }
        public Guid ImageInformationId { get;set; }
        
        public AreaDataType DataType { get; set; }
        public string PathData { get; set; }

        public ImageArea Clone()
        {
            return (ImageArea) MemberwiseClone();
        }
    }
}