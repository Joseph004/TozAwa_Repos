using System;

namespace Grains.Auth.Models.Dtos.Backend
{
    public class PartnerDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Adress { get; set; }
        public string PhoneNumber { get; set; }
        public string Description { get; set; }
        public bool Deleted { get; set; }
    }
}


