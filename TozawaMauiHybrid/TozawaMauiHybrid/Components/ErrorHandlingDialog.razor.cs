using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace TozawaMauiHybrid.Component
{
    public partial class ErrorHandlingDialog : BaseDialog<ErrorHandlingDialog>
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
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
        private void Confirm()
        {
            MudDialog.Close(DialogResult.Ok(true));
        }
        private void Reload()
        {
            NavManager.NavigateTo(NavManager.Uri, true);
        }
        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await base.OnAfterRenderAsync(firstRender);
            }
        }
    }
}