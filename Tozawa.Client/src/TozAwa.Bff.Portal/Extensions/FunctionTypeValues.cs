using Tozawa.Bff.Portal.Models.Enums;

namespace Tozawa.Bff.Portal.extension
{
    public class FunctionTypeValues : Attribute
    {
        public string PairId { get; set; } = "";
        public AccessType AccessType { get; set; }
        public bool OnlyVisibleForRoot { get; set; }
        public bool Obsolete { get; set; }
    }
}
