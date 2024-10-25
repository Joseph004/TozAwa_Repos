
namespace TozawaMauiHybrid.Models
{
    public interface ITextEntity
    {
        Guid Id { get; set; }
        string Text { get; set; }
        string Description { get; set; }
    }
}