using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Tozawa.Language.Client.Pages.Components
{
    public partial class ConfirmationDialog : ComponentBase
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        private void Yes() => MudDialog.Close(DialogResult.Ok(true));
        void No() => MudDialog.Cancel();
    }
}
