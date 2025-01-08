namespace Grains.Auth.Models.Dtos
{
    public class StationDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid DescriptionTextId { get; set; }
        public bool Deleted { get; set; }
        public string Adresse { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public Guid OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public List<ReportDto> Reports { get; set; }
        public List<AddressDto> Addresses { get; set; }
    }
}