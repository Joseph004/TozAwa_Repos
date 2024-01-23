using System.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using TozawaNGO.Models.Dtos;
using TozawaNGO.Helpers;
using Blazored.LocalStorage;
using Microsoft.JSInterop;
using TozawaNGO.Services;

namespace TozawaNGO.Shared
{
    public partial class AppBar : BaseComponent
    {
        [Parameter]
        public EventCallback OnSidebarToggled { get; set; }
        [Parameter]
        public EventCallback<MudTheme> OnThemeToggled { get; set; }
        [Inject] ILocalStorageService _localStorageService { get; set; }
        [Inject] private IDialogService DialogService { get; set; }
        [Inject] LoadingState LoadingState { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }
        [Inject] NavigationManager _navigationManager { get; set; }
        [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        private MudTheme _currentTheme = new();
        public string _loginUrl { get; set; } = $"";
        private bool _isLightMode = true;
        private bool _showLogo = false;

        protected async override Task OnInitializedAsync()
        {
            _translationService.LanguageChanged += _translationService_LanguageChanged;
            _authStateProvider.UserAuthenticationChanged += _authStateProvider_UserAuthChanged;

            await base.OnInitializedAsync();
        }
        private async void _authStateProvider_UserAuthChanged(object sender, EventArgs e)
        {
            await Task.FromResult(1);
            StateHasChanged();
        }
        private void _translationService_LanguageChanged(object sender, EventArgs e)
        {
            StateHasChanged();
        }
        private async Task ToggleSideBar()
        {
            _showLogo = !_showLogo;
            await OnSidebarToggled.InvokeAsync();
        }
        private async Task ToggleTheme()
        {
            _isLightMode = !_isLightMode;
            _currentTheme = !_isLightMode ? GenerateDarkTheme() : new MudTheme
            {
                Palette = new PaletteLight()
                {
                    AppbarBackground = "#000000"
                }
            };
            await OnThemeToggled.InvokeAsync(_currentTheme);
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
                    Position = DialogPosition.TopCenter,
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
                        await _localStorageService.SetItemAsync("authToken", userResponse.Token);
                        await _localStorageService.SetItemAsync("refreshToken", userResponse.RefreshToken);

                        _loginUrl = $"login{NavigateToReturnPage()}";
                        LoadingState.SetRequestInProgress(false);
                        await JSRuntime.InvokeVoidAsync("open", Decode(_loginUrl), "_top");
                    }
                }
            }
        }
        private string NavigateToReturnPage()
        {
            var currentPath = _navigationManager.Uri.Split(_navigationManager.BaseUri)[1];

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
            await _localStorageService.RemoveItemAsync("authToken");
            await _localStorageService.RemoveItemAsync("refreshToken");

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
                    IsFirstLoaded = true;
                    ((AuthStateProvider)_authStateProvider).NotifyUserAuthentication();

                    _currentUser = await _currentUserService.GetCurrentUser();
                    StateHasChanged();
                }

                await base.OnAfterRenderAsync(firstRender);
            }
        }
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
        public override void Dispose()
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
        {
            _translationService.LanguageChanged -= _translationService_LanguageChanged;
            _authStateProvider.UserAuthenticationChanged -= _authStateProvider_UserAuthChanged;
        }
    }
}
