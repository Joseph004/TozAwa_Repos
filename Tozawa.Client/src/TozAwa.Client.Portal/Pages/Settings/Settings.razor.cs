using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tozawa.Client.Portal.Shared;

namespace Tozawa.Client.Portal.Pages
{
    public partial class Settings : BasePage
    {
        private ActiveTab _activeTab;
        void Activate(ActiveTab activeTab)
        {
            _activeTab = activeTab;
        }
    }

    public enum ActiveTab
    {
        Members = 1,
        Organizations = 2
    }
}