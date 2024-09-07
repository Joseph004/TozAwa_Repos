using Microsoft.AspNetCore.Components;
using MudBlazor;
using TozawaNGO.Models;
using TozawaNGO.Shared;

namespace TozawaNGO.Pages
{
    public partial class DescriptionMemberDialog : BaseDialog
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Parameter] public ITextEntity Entity { get; set; }

        void Cancel() => MudDialog.Cancel();
    }
}