using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ShareRazorClassLibrary.Services;

namespace TozawaNGO.Shared
{
    public partial class NavMenu : BaseComponent
    {
        [Inject] private NavMenuTabState NavMenuTabState { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }
        [Inject] private ISessionStorageService SessionStorageService { get; set; }

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
        [Inject] FirsloadState FirsloadState { get; set; }
        [Inject] NavigationManager NavManager { get; set; }

        protected async override Task OnInitializedAsync()
        {
            FirsloadState.OnChange += FirsLoadChanged;
            _translationService.LanguageChanged += _translationService_LanguageChanged;
            _authStateProvider.UserAuthenticationChanged += _authStateProvider_UserAuthChanged;

            await base.OnInitializedAsync();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var currentTab = await SessionStorageService.GetItemAsync<ActiveTab?>(nameof(ActiveTab));
                if (currentTab.HasValue)
                {
                    NavMenuTabState.SetActiveTab(currentTab.Value);
                }
                _currentUser = await _currentUserService.GetCurrentUser();
            }
            await base.OnAfterRenderAsync(firstRender);
        }
        private async Task OnClickTab(string link)
        {
            var sizeWidth = await JSRuntime.InvokeAsync<int>("getScreeenSize");
            if (sizeWidth <= 980)
            {
                SideBarOpen = !SideBarOpen;
                StateHasChanged();
            }
            NavMenuTabState.SetMenuOpen(SideBarOpen);
            var tab = link switch
            {
                "/" => ActiveTab.Home,
                "/fetchdata1" => ActiveTab.Counter,
                "/settings" => ActiveTab.Settings,
                "/fetchdata" => ActiveTab.Weather,
                _ => ActiveTab.Home,
            };
            NavMenuTabState.SetActiveTab(tab);
            await SessionStorageService.SetItemAsync(nameof(ActiveTab), NavMenuTabState.ActiveTab);
            NavManager.NavigateTo(link);
        }
        private void FirsLoadChanged()
        {
            StateHasChanged();
        }
        private void _authStateProvider_UserAuthChanged(object sender, EventArgs e)
        {
            StateHasChanged();
        }
        private void _translationService_LanguageChanged(object sender, EventArgs e)
        {
            StateHasChanged();
        }

        protected override void Dispose(bool disposed)
        {
            FirsloadState.OnChange -= FirsLoadChanged;
            _translationService.LanguageChanged -= _translationService_LanguageChanged;
            _authStateProvider.UserAuthenticationChanged -= _authStateProvider_UserAuthChanged;
            base.Dispose(disposed);
        }
    }
}
