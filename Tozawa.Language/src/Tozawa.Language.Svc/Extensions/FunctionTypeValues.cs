using Tozawa.Language.Svc.Models.Enums;

namespace Tozawa.Language.Svc.extension
{
    public class FunctionTypeValues : Attribute
    {
        public string PairId { get; set; } = "";
        public AccessType AccessType { get; set; }
        public bool OnlyVisibleForRoot { get; set; }
        public bool Obsolete { get; set; }
    }
}
