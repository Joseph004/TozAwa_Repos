using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ShareRazorClassLibrary.Models.Dtos;
using ShareRazorClassLibrary.Services;

namespace TozawaNGO.Shared
{
    public partial class NavMenu : BaseComponent<NavMenu>
    {
        [Inject] private NavMenuTabState NavMenuTabState { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }
        [Inject] private ISessionStorageService SessionStorageService { get; set; }
        private Dictionary<string, string> _logos => new() { { "fr", "462577779_1085853349907518_967484155474395323_n.png" }, { "gb", "462580031_550604717852502_2939675846526858774_n.png" } };
        private string _logo = "462577779_1085853349907518_967484155474395323_n.png";
        private bool EndOpen = true;
        private (bool isOpen, int leftSize, int rightSize) _valueSettings;
        [Parameter]
        public (bool isOpen, int leftSize, int rightSize) Settings
        {
            get => _valueSettings;
            set
            {
                if (_valueSettings == value) return;
                _valueSettings = value;
                SettingsChanged.InvokeAsync(value);
            }
        }
        private void GoToHome()
        {
            var homePage = NavManager.ToBaseRelativePath(NavManager.Uri);
            var activePath = NavMenuTabState.GetActivePath();
            if (!string.IsNullOrEmpty(homePage) && activePath != "/")
            {
                NavManager.NavigateTo("/");
            }
        }
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
        public EventCallback<(bool isOpen, int leftSize, int rightSize)> SettingsChanged { get; set; }
        [Parameter]
        public EventCallback<bool> SideBarOpenChanged { get; set; }
        [Inject] FirstloadState FirstloadState { get; set; }
        [Inject] NavigationManager NavManager { get; set; }

        protected async override Task OnInitializedAsync()
        {
            FirstloadState.OnChange += FirsLoadChanged;
            _translationService.LanguageChanged += _translationService_LanguageChanged;
            _authStateProvider.UserAuthenticationChanged += _authStateProvider_UserAuthChanged;

            await base.OnInitializedAsync();
        }
        private void SetLogo()
        {
            var currentLanguage = _translationService.ActiveLanguage();
            _logo = _logos.FirstOrDefault(x => x.Key == currentLanguage.ShortName).Value;
            StateHasChanged();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                SetLogo();
                var currentTab = await SessionStorageService.GetItemAsync<ActiveTab?>(nameof(ActiveTab));
                if (currentTab.HasValue)
                {
                    NavMenuTabState.SetActiveTab(currentTab.Value);
                }
                await Task.Delay(new TimeSpan(0, 0, Convert.ToInt32(0.1))).ContinueWith(o => { FirstloadState.SetFirsLoad(true); });
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
        private void FirsLoadChanged()
        {
            InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }
        private string GetNavMudGroupRoles()
        {
            return $"{Enum.GetName(typeof(Role), Role.LandLoard)}";
        }
        private void _authStateProvider_UserAuthChanged(object sender, EventArgs e)
        {
            InvokeAsync(() =>
          {
              StateHasChanged();
          });
        }
        private void _translationService_LanguageChanged(object sender, EventArgs e)
        {
            InvokeAsync(() =>
          {
              SetLogo();
              StateHasChanged();
          });
        }

        protected override void Dispose(bool disposed)
        {
            FirstloadState.OnChange -= FirsLoadChanged;
            _translationService.LanguageChanged -= _translationService_LanguageChanged;
            _authStateProvider.UserAuthenticationChanged -= _authStateProvider_UserAuthChanged;
            base.Dispose(disposed);
        }
    }
}
