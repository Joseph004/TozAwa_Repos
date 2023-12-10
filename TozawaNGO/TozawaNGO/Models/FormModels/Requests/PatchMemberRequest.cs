
using TozawaNGO.HttpClients;

namespace TozawaNGO.Models.FormModels
{
    public class PatchMemberRequest : PatchBase
    {
        public string FirstName { get; set; } = null;
        public string LastName { get; set; } = null;
        public string Email { get; set; } = null;
        public string Description { get; set; } = null;
        public bool? SoftDeleted { get; set; } = null;
        public bool? HardDeleted { get; set; } = null;
    }
}