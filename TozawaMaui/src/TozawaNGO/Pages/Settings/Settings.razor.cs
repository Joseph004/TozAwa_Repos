using Microsoft.AspNetCore.Components;
using ShareRazorClassLibrary.Services;
using TozawaNGO.Shared;

namespace TozawaNGO.Pages
{
    public partial class Settings : BasePage
    {
        [Inject] NavMenuTabState NavMenuTabState { get; set; }
        private ActiveTab _activeTab;

        protected override void OnInitialized()
        {
            NavMenuTabState.SetActiveTab(ShareRazorClassLibrary.Services.ActiveTab.Settings);
            base.OnInitialized();
        }
        void Activate(ActiveTab activeTab)
        {
            _activeTab = activeTab;
        }
    }

    public enum ActiveTab
    {
        Members = 1,
        Organizations = 2,
        Files = 3
    }
}