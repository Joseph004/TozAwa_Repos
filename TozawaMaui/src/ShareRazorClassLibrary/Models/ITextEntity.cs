using ShareRazorClassLibrary.Models.Dtos;

namespace ShareRazorClassLibrary.Models
{
    public interface ITextEntity
    {
        Guid Id { get; set; }
        string Text { get; set; }
        string Description { get; set; }
    }
}