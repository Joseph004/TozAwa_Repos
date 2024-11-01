using System.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Microsoft.JSInterop;
using TozawaMauiHybrid.Helpers;
using TozawaMauiHybrid.Models.Dtos;
using TozawaMauiHybrid.Services;

namespace TozawaMauiHybrid.Component
{
    public partial class AppBar : BaseComponent
    {
        [Parameter]
        public EventCallback OnSidebarToggled { get; set; }
        [Inject] PreferencesStoreClone _storage { get; set; }
        [Inject] private IDialogService DialogService { get; set; }
        [Inject] LoadingState LoadingState { get; set; }
        [Inject] private NavMenuTabState NavMenuTabState { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }
        [Inject] FirsloadState FirsloadState { get; set; }
        [Inject] NavigationManager _navigationManager { get; set; }
        [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        public string _loginUrl { get; set; } = $"";
        private bool _showLogo = DeviceInfo.Platform != DevicePlatform.WinUI;

        protected async override Task OnInitializedAsync()
        {
            NavMenuTabState.OnChange += HandleLogo;
            FirsloadState.OnChange += FirsLoadChanged;
            _translationService.LanguageChanged += _translationService_LanguageChanged;
            _authStateProvider.UserAuthenticationChanged += _authStateProvider_UserAuthChanged;

            await base.OnInitializedAsync();
        }
        private async void _authStateProvider_UserAuthChanged(object sender, EventArgs e)
        {
            await Task.FromResult(1);
            StateHasChanged();
        }
        private void FirsLoadChanged()
        {
            StateHasChanged();
        }
        private void _translationService_LanguageChanged(object sender, EventArgs e)
        {
            StateHasChanged();
        }
        private void HandleLogo()
        {
            if (NavMenuTabState.IsMenuOpen && _showLogo)
            {
                _showLogo = false;
            }
            else if (!NavMenuTabState.IsMenuOpen && !_showLogo)
            {
                _showLogo = true; 
            }
            StateHasChanged();
        }
        private async Task ToggleSideBar()
        {
            if (DeviceInfo.Platform == DevicePlatform.WinUI)
            {
                _showLogo = !_showLogo;
            }
            await OnSidebarToggled.InvokeAsync();
        }
        private static string Decode(string param)
        {
            return HttpUtility.UrlDecode(param);
        }
        private async Task Login()
        {
            var context = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            if (!context.User.Identity.IsAuthenticated)
            {
                var parameters = new DialogParameters
                {
                    ["Title"] = "Login"
                };
                DialogOptions options = new()
                {
                    DisableBackdropClick = true,
                    Position = DialogPosition.Center,
                    MaxWidth = MaxWidth.Small,
                    CloseButton = false
                };
                var dialog = DialogService.Show<LoginViewModal>("Login", parameters, options);
                var result = await dialog.Result;

                if (!result.Canceled)
                {
                    var userResponse = (LoginResponseDto)result.Data;

                    if (userResponse.LoginSuccess)
                    {
                        _storage.Set("authToken", userResponse.Token);
                        _storage.Set("refreshToken", userResponse.RefreshToken);

                        _loginUrl = $"login{NavigateToReturnPage()}";
                        LoadingState.SetRequestInProgress(false);
                        await JSRuntime.InvokeVoidAsync("open", Decode(_loginUrl), "_top");
                    }
                }
            }
        }
        private string NavigateToReturnPage()
        {
            var currentPath = "";//_navigationManager.Uri.Split(_navigationManager.BaseUri)[1];

            if (string.IsNullOrEmpty(currentPath))
            {
                return "/home";
            }
            else
            {
                return $"/{currentPath}";
            }
        }
        private async Task Logout()
        {
            _storage.Delete("authToken");
            _storage.Delete("refreshToken");

            ((AuthStateProvider)_authStateProvider).NotifyUserLogout();
            StateHasChanged();

            var logoutUrl = $"logout{NavigateToReturnPage()}";
            await JSRuntime.InvokeVoidAsync("open", Decode(logoutUrl), "_top");
        }
        private async Task Register()
        {
            await Task.FromResult(1);
        }
        private static MudTheme GenerateDarkTheme() =>
            new()
            {
                Palette = new PaletteDark()
                {
                    Black = "#27272f",
                    Background = "#32333d",
                    BackgroundGrey = "#27272f",
                    Surface = "#373740",
                    TextPrimary = "#ffffffb3",
                    TextSecondary = "rgba(255,255,255, 0.50)",
                    AppbarBackground = "#000000",
                    AppbarText = "#ffffffb3",
                    DrawerBackground = "#27272f",
                    DrawerText = "#ffffffb3",
                    DrawerIcon = "#ffffffb3"
                }
            };

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var auth = await _authStateProvider.GetAuthenticationStateAsync();
                if (auth.User.Identity.IsAuthenticated)
                {
                    await ((AuthStateProvider)_authStateProvider).NotifyUserAuthentication();

                    _currentUser = await _currentUserService.GetCurrentUser();
                    StateHasChanged();
                }
            }
            await base.OnAfterRenderAsync(firstRender);
        }
        public override void Dispose()
        {
            NavMenuTabState.OnChange -= HandleLogo;
            FirsloadState.OnChange -= FirsLoadChanged;
            _translationService.LanguageChanged -= _translationService_LanguageChanged;
            _authStateProvider.UserAuthenticationChanged -= _authStateProvider_UserAuthChanged;
        }
    }
}
