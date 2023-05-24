using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using TozAwaHome.Models.Dtos;
using TozAwaHome.Models.FormModels;
using TozAwaHome.Services;
using TozAwaHome.Shared;

namespace TozAwaHome.Pages
{
    public partial class LoadingDialog : BaseDialog
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Parameter] public string body { get; set; }

        protected override void OnInitialized()
        {
        }
		void Submit() => MudDialog.Close(DialogResult.Ok(true));
    }
}