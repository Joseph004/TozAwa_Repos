using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using ShareRazorClassLibrary.Models.Dtos;
using TozawaNGO.Shared;

namespace TozawaNGO.Pages
{
    public partial class DeleteEntityDialog : BaseDialog<DeleteEntityDialog>
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }
        [Parameter] public BaseDto item { get; set; }
        [Parameter] public string title { get; set; }
        [Parameter] public bool hardDelete { get; set; }
        [Parameter] public string body { get; set; }

        private void Cancel()
        {
            MudDialog.Cancel();
        }
        protected override void OnInitialized()
        {
        }
        private void Confirm(bool softDeleted = false, bool hardDeleted = false)
        {
            var request = new ShareRazorClassLibrary.Models.FormModels.DeleteRequest
            {
                SoftDeleted = softDeleted,
                HardDeleted = hardDeleted
            };
            MudDialog.Close(DialogResult.Ok(request));
        }
        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
                JSRuntime.InvokeVoidAsync($"setModalDraggableAndResizable");

            return base.OnAfterRenderAsync(firstRender);
        }
    }
}