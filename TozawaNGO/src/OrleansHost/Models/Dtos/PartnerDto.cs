using System.ComponentModel.DataAnnotations;

namespace OrleansHost.Models.Dtos
{
    public class PartnerDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Adress { get; set; }
        public string PhoneNumber { get; set; }
    }
}