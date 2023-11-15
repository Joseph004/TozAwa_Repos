
namespace TozawaNGO.Models.FormModels
{
    public class DeleteRequest
    {
        public bool SoftDeleted { get; set; }
        public bool HardDeleted { get; set; }
    }
}