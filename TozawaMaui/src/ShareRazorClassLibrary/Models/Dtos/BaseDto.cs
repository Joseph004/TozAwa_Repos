using System;

namespace ShareRazorClassLibrary.Models.Dtos
{
    public class BaseDto : ICloneable
    {
        public Guid Id { get; set; }
        public string CreatedBy { get; set; }
        public DateTime Timestamp { get; set; }
        public bool Deleted { get; set; }
        public string Name { get; set; } = "";

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}