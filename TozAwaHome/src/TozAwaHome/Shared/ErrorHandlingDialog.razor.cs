using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace TozAwaHome.Shared
{
    public partial class ErrorHandlingDialog : BaseDialog
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Parameter] public string title { get; set; }
        [Parameter] public string body { get; set; }

        private void Cancel()
        {
            MudDialog.Cancel();
        }
        protected override void OnInitialized()
        {
        }
        private void Confirm()
        {
            MudDialog.Close(DialogResult.Ok(true));
        }
    }
}