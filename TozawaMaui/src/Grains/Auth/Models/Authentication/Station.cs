using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Grains.Auth.Models.Authentication
{
    public class Station : CreatedModified
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid DescriptionTextId { get; set; }
        public bool Deleted { get; set; }
        public string Adresse { get; set; }
        public string City { get; set; }
        public string Commun { get; set; }
        public string Country { get; set; } = "Congo-Kinshasa";
        public Guid OrganizationId { get; set; }
        [ForeignKey("OrganizationId")]
        public Organization Organization { get; set; }
        public ICollection<Establishment> Establishments { get; set; }
        public ICollection<Report> Reports { get; set; }
        public ICollection<StationAddress> Addresses { get; set; }
    }
    public class StationAddress : CreatedModified
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string CityCode { get; set; }
        public string CountryCode { get; set; }
        public string City { get; set; }
        public string Commun { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public bool Active { get; set; }
        public Guid StationId { get; set; }
        [ForeignKey("StationId")]
        public Station Station { get; set; }
    }

    public class Establishment : CreatedModified
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string COO { get; set; }
        public string Description { get; set; }
        public Guid DescriptionTextId { get; set; }
        public bool Deleted { get; set; }
        public Guid StationId { get; set; }
        [ForeignKey("StationId")]
        public virtual Station Station { get; set; }
    }
}