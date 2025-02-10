namespace TozawaMauiHybrid.Models.Dtos;

public class FeatureDto : ITextEntity
{
    public int Id { get; set; }
    public Guid TextId { get; set; }
    public string Text { get; set; }
    public Guid DescriptionTextId { get; set; }
    public string Description { get; set; }
    public string Comment { get; set; }
    public bool Deleted { get; set; }
    public DateTime Timestamp { get; set; }
}