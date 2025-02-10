using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace TozawaNGO.Shared
{
    public partial class ErrorHandlingDialog : BaseDialog<ErrorHandlingDialog>
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }
        [Inject] NavigationManager NavManager { get; set; }
        [Parameter] public string title { get; set; }
        [Parameter] public string body { get; set; }

        private void Cancel()
        {
            MudDialog.Cancel();
        }
        protected override void OnInitialized()
        {
        }
        private void Reload()
        {
            NavManager.NavigateTo(NavManager.Uri, true);
        }
        private void Confirm()
        {
            MudDialog.Close(DialogResult.Ok(true));
        }
        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            return base.OnAfterRenderAsync(firstRender);
        }
    }
}