using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrleansHost.Auth.Models.Authentication
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
        public string Country { get; set; } = "Congo-Kinshasa";
        public ICollection<Establishment> Establishments { get; set; }
        public ICollection<Report> Reports { get; set; }
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