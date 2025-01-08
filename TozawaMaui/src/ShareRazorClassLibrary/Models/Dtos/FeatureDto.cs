namespace ShareRazorClassLibrary.Models.Dtos;

public class FeatureDto
{
    public int Id { get; set; }
    public Guid TextId { get; set; }
    public string Text { get; set; }
    public Guid DescriptionTextId { get; set; }
    public string Description { get; set; }
    public bool Deleted { get; set; }
}