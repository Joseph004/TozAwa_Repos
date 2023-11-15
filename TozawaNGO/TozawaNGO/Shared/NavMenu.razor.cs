using System.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MudBlazor;
using TozawaNGO.Services;
using System.Timers;
using Timer = System.Timers.Timer;
using MudBlazor.Extensions.Helper;

namespace TozawaNGO.Shared
{
    public partial class NavMenu : BaseComponent
    {
        [Parameter]
        public bool SideBarOpen { get; set; }

        protected async override Task OnInitializedAsync()
        {
            _translationService.LanguageChanged += _translationService_LanguageChanged;

            await base.OnInitializedAsync();
        }

        private void _translationService_LanguageChanged(object sender, EventArgs e)
        {
            StateHasChanged();
        }
    }
}
