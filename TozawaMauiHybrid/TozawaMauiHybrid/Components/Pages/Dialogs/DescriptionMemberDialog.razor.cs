using Microsoft.AspNetCore.Components;
using MudBlazor;
using TozawaMauiHybrid.Models;
using TozawaMauiHybrid.Component;

namespace TozawaMauiHybrid.Pages
{
    public partial class DescriptionMemberDialog : BaseDialog<DescriptionMemberDialog>
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Parameter] public ITextEntity Entity { get; set; }
        [Parameter] public string Title { get; set; }

        void Cancel() => MudDialog.Cancel();
    }
}