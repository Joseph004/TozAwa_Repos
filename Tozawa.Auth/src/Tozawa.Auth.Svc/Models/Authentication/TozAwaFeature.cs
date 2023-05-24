using System;

namespace Tozawa.Auth.Svc.Models.Authentication
{
    public class TozAwaFeature
    {
        public int Id { get; set; }
        public Guid TextId { get; set; }
        public Guid DescriptionTextId { get; set; }
        public bool Deleted { get; set; }
    }
}
