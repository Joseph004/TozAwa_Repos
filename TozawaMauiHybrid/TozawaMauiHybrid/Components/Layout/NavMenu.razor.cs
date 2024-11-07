using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TozawaMauiHybrid.Component;
using TozawaMauiHybrid.Helpers;
using TozawaMauiHybrid.Services;

namespace TozawaMauiHybrid.Components.Layout
{
    public partial class NavMenu : BaseComponent
    {
        [Inject] private NavMenuTabState NavMenuTabState { get; set; }
        [Inject] PreferencesStoreClone _storage { get; set; }
        private bool _value;
        [Parameter]
        public bool SideBarOpen
        {
            get => _value;
            set
            {
                if (_value == value) return;
                _value = value;
                SideBarOpenChanged.InvokeAsync(value);
            }
        }
        [Parameter]
        public EventCallback<bool> SideBarOpenChanged { get; set; }
        [Inject] FirstloadState FirstloadState { get; set; }
        [Inject] NavigationManager NavManager { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }


        protected async override Task OnInitializedAsync()
        { 
            FirstloadState.OnChange += FirsLoadChanged;
            _translationService.LanguageChanged += _translationService_LanguageChanged;
            _authStateProvider.UserAuthenticationChanged += _authStateProvider_UserAuthChanged;

            await base.OnInitializedAsync();
        }
        private async void FirsLoadChanged()
        {
           await InvokeAsync(async() =>
            {
                _currentUser = await _currentUserService.GetCurrentUser();
                StateHasChanged();
            });
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var currentTab = _storage.Get<ActiveTab?>(nameof(ActiveTab));
                if (currentTab.HasValue)
                {
                    NavMenuTabState.SetActiveTab(currentTab.Value);
                }
                await Task.Delay(new TimeSpan(0, 0, Convert.ToInt32(0.1))).ContinueWith(o => { FirstloadState.SetFirsLoad(true); });
                _currentUser = await _currentUserService.GetCurrentUser();
                await base.OnAfterRenderAsync(firstRender);
            }
        }
        private async Task OnClickTab(string link)
        {
            var previousTab = NavMenuTabState.ActiveTab;
            if (NavMenuTabState.GetTabPath(previousTab) != link)
            {
                var sizeWidth = await JSRuntime.InvokeAsync<int>("getScreeenSize");
                if (sizeWidth <= 980)
                {
                    SideBarOpen = !SideBarOpen;
                    StateHasChanged();
                }
                NavMenuTabState.SetMenuOpen(SideBarOpen);
                NavMenuTabState.SetPreviousTab(previousTab);
                NavManager.NavigateTo(link);
            }
        }
        private void _authStateProvider_UserAuthChanged(object sender, EventArgs e)
        {
            StateHasChanged();
        }
        private void _translationService_LanguageChanged(object sender, EventArgs e)
        {
            StateHasChanged();
        }

        public override void Dispose()
        {
            FirstloadState.OnChange -= FirsLoadChanged;
            _translationService.LanguageChanged -= _translationService_LanguageChanged;
            _authStateProvider.UserAuthenticationChanged -= _authStateProvider_UserAuthChanged;
        }
    }
}
