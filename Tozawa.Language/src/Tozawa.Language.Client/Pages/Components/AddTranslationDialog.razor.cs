using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Tozawa.Language.Client.Pages.Components
{
    public partial class AddTranslationDialog : ComponentBase
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Parameter] public string Text { get; set; }

        void Add() => MudDialog.Close(DialogResult.Ok(Text));
        void Cancel() => MudDialog.Cancel();
    }
}
