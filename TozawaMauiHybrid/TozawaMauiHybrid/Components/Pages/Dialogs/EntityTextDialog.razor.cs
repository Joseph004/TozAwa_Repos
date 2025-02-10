using Microsoft.AspNetCore.Components;
using MudBlazor;
using TozawaMauiHybrid.Models;
using TozawaMauiHybrid.Component;
using TozawaMauiHybrid.Models.Enums;
using TozawaMauiHybrid.Helpers;

namespace TozawaMauiHybrid.Pages
{
    public partial class EntityTextDialog : BaseDialog<EntityTextDialog>
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Parameter] public ITextEntity Entity { get; set; }
        [Parameter] public TypeOfText TypeOfText { get; set; }
        [Parameter] public string Title { get; set; }

        private string GetText()
        {
            if (TypeOfText == TypeOfText.Description) return Entity.Description;
            if (TypeOfText == TypeOfText.Text) return Entity.Text;
            if (TypeOfText == TypeOfText.Comment) return Entity.Comment;

            return "";
        }
        private string GetLabel()
        {
            if (TypeOfText == TypeOfText.Description) return Translate(SystemTextId.Description);
            if (TypeOfText == TypeOfText.Text) return Translate(SystemTextId.Name);
            if (TypeOfText == TypeOfText.Comment) return "Comment";

            return "";
        }
        void Cancel() => MudDialog.Cancel();
    }
}