
namespace Grains.Models.Attachment.Store
{
    [GenerateSerializer]
    public class AttachmentStates
    {
        public HashSet<string> Items { get; set; }
    }

    [GenerateSerializer]
    public class AttachmentState
    {
        public AttachmentItem Attachment { get; set; }
    }
}