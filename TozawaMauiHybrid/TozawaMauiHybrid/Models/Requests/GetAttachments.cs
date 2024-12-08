namespace TozawaMauiHybrid.Models.Requests;

public class GetAttachments
{
    public string SearchString { get; set; } = null;
    public Guid OwnerId { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }
    public List<Guid> AttachmentIds { get; set; }
}