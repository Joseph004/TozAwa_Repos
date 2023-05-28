using Tozawa.Auth.Svc.Models.Enums;

namespace Tozawa.Auth.Svc.extension
{
    public class FunctionTypeValues : Attribute
    {
        public string PairId { get; set; } = "";
        public AccessType AccessType { get; set; }
        public bool OnlyVisibleForRoot { get; set; }
        public bool Obsolete { get; set; }
    }
}
