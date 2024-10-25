using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace TozawaMauiHybrid.Component
{
    public partial class ErrorHandling : BaseComponent
    {
        [Inject] ILogger<ErrorHandling> Logger { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; } = null;
        [Inject] IDialogService DialogService { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
        }
        public async Task ProcessError(Exception ex, string title, string body)
        {
            Logger.LogError("Error:ProcessError - Type: {Type} Message: {Message}",
            ex.GetType(), ex.Message);

            var options = new DialogOptions
            {
                MaxWidth = MaxWidth.Small
            };

            var parameters = new DialogParameters
            {
                ["body"] = body,
                ["title"] = title
            };

            var dialog = DialogService.Show<ErrorHandlingDialog>(title, parameters, options);
            var result = await dialog.Result;
        }
    }
}