using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Tozawa.Language.Client.Pages.Components
{
    public partial class AddSystemTypeDialog : ComponentBase
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Parameter] public string Name { get; set; }

        void Add() => MudDialog.Close(DialogResult.Ok(Name));
        void Cancel() => MudDialog.Cancel();
    }
}
