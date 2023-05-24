using System.ComponentModel;

namespace Tozawa.Language.Svc.Context
{
    public enum XliffFileState
    {
        [Description("export")]
        Export = 0,

        [Description("import")]
        Import = 1,
    }
}
