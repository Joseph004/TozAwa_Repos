using TozawaNGO.Shared;

namespace TozawaNGO.Pages
{
    public partial class Settings : BasePage
    {
        private ActiveTab _activeTab;

        protected override void OnInitialized()
        {
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