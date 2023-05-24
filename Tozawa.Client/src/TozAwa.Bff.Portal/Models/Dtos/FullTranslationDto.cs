using System;

namespace Tozawa.Bff.Portal.Models;

public class FullTranslationDto
{
    public Guid Id { get; set; }
    public string Text { get; set; }
    public bool Proposed { get; set; }
    public Guid CategoryId { get; set; }
}
