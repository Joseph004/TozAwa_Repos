using Tozawa.Language.Svc.Context;

namespace Tozawa.Language.Svc.Models
{
    public class TranslateableText
    {
        public string Id { get; set; }
        public string SystemTypeName { get; set; }
        public string Context { get; set; }
        public string Source { get; set; }
        public string Target { get; set; }

        public XliffState TranslationState { get; set; }
    }
}