using Microsoft.AspNetCore.Components;
using MudBlazor;
using ShareRazorClassLibrary.Models;
using TozawaNGO.Shared;

namespace TozawaNGO.Pages
{
    public partial class DescriptionMemberDialog : BaseDialog<DescriptionMemberDialog>
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Parameter] public ITextEntity Entity { get; set; }
        [Parameter] public string Title { get; set; }

        void Cancel() => MudDialog.Cancel();
    }
}