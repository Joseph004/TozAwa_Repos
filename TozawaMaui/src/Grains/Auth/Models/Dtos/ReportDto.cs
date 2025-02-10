namespace Grains.Auth.Models.Dtos;

public class ReportDto
{
    public Guid Id { get; set; }
    public string Comment { get; set; }
    public Guid CommentTextId { get; set; }
    public Guid StationId { get; set; }
    public string StationName { get; set; }
}