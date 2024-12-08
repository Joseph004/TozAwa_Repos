using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Core;
using MudBlazor.Extensions.Options;
using Nextended.Core.Extensions;
using TozawaMauiHybrid.Component;
using TozawaMauiHybrid.Helpers;
using TozawaMauiHybrid.Services;

namespace TozawaMauiHybrid.Components;

public class ErrorHandling<T> : ErrorBoundary
{
    [Parameter]
    public T TCategoryName { get; set; }
    [Inject] ILogger<T> _logger { get; set; }
    [Inject] IDialogService DialogService { get; set; }
    [Inject] protected ITranslationService _translationService { get; set; }
    protected async override Task OnErrorAsync(Exception exception)
    {
        await ProcessError(exception, _translationService.Translate(SystemTextId.Error, "Error").Text, _translationService.Translate(SystemTextId.ErrorOccursPleaseContactSupport, "Opps, something went wrong. Please contact support!").Text);
        Recover();
        await InvokeAsync(() =>
         {
             StateHasChanged();
         });
        await base.OnErrorAsync(exception);
    }

    public async Task ProcessError(Exception ex, string title, string body)
    {
        _logger.LogError("Error:ProcessError - Type: {Type} Message: {Message}",
        ex.GetType(), ex.Message);
        var options = new DialogOptionsEx
        {
            BackgroundClass = "tz-mud-overlay",
            BackdropClick = false,
            CloseButton = false,
            MaxWidth = MaxWidth.Small,
            MaximizeButton = true,
            FullHeight = true,
            FullWidth = true,
            DragMode = MudDialogDragMode.Simple,
            Animations = [AnimationType.Pulse],
            Position = DialogPosition.Center
        };

        options.SetProperties(ex => ex.Resizeable = true);
        options.DialogAppearance = MudExAppearance.FromStyle(b =>
        {
            b.WithBackgroundColor("gold")
            .WithOpacity(0.9);
        });

        var parameters = new DialogParameters
        {
            ["body"] = body,
            ["title"] = title
        };

        var dialog = await DialogService.ShowEx<ErrorHandlingDialog>(title, parameters, options);
        var result = await dialog.Result;
    }
}