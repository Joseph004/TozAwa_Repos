using System;

namespace Tozawa.Auth.Svc.Models
{
    public class CreatedModified
    {
        public string CreatedBy { get; set; } = "";
        public DateTime CreateDate { get; set; }
        public string ModifiedBy { get; set; } = "";
        public DateTime? ModifiedDate { get; set; }

    }
}
