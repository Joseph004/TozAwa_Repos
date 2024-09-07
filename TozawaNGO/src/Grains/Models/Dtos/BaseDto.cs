using System;

namespace Grains.Models.Dtos
{
    public class BaseDto : ICloneable
    {
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public string CreatedBy { get; set; }
        public bool Deleted { get; set; }
        public string Name { get; set; } = "";

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}