using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace TozawaNGO.Shared
{
    public partial class ErrorHandlingDialog : BaseDialog
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }
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
        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
                JSRuntime.InvokeVoidAsync($"setModalDraggableAndResizable");

            return base.OnAfterRenderAsync(firstRender);
        }
    }
}