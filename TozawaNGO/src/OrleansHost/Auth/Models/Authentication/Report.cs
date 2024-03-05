using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrleansHost.Auth.Models.Authentication
{
    public class Report : CreatedModified
    {
        [Key]
        public Guid Id { get; set; }
        public string Comment { get; set; }
        public Guid CommentTextId { get; set; }
        public Guid StationId { get; set; }
        [ForeignKey("StationId")]
        public virtual Station Station { get; set; }
    }
}