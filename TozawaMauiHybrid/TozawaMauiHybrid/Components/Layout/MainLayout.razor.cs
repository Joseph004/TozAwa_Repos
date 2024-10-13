using System.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System.Timers;
using Timer = System.Timers.Timer;
using System.Text;
using Microsoft.AspNetCore.Components.Web;

namespace TozawaMauiHybrid.Components.Layout
{
    public partial class MainLayout : LayoutComponentBase
    {
        public string _loginUrl { get; set; } = $"";
        private Timer _timer;
        private readonly int _timerInterval = 15 * 60 * 1000; //15 min
        private bool _disabledPage = false;
        private string _disableAttrString = "";
        private ErrorBoundary _errorBoundary;

        private bool _sidebarOpen = true;
        private void ToggleTheme(MudTheme changedTheme) => _currentTheme = changedTheme;
        private void ToggleSidebar()
        {
            _sidebarOpen = !_sidebarOpen;
            StateHasChanged();
        }
        protected override void OnParametersSet()
        {
            _errorBoundary?.Recover();
        }
        private MudTheme _currentTheme = new()
        {
            Palette = new PaletteLight()
            {
                AppbarBackground = "#000000"
            }
        };
        protected async override Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }
        private void RefreshTimer(EventArgs e)
        {
            _timer.Interval = _timerInterval;
        }
    }
}
