using Tozawa.Language.Client.Models.DTOs;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Tozawa.Language.Client.Pages.Components
{
    public partial class AddLanguageDialog : ComponentBase
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Parameter] public LanguageDto LanguageDto { get; set; } = new LanguageDto();

        void Add() => MudDialog.Close(DialogResult.Ok(LanguageDto));
        void Cancel() => MudDialog.Cancel();
    }
}
