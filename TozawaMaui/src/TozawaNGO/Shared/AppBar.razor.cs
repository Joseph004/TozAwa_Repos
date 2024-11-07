using System.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Blazored.LocalStorage;
using Microsoft.JSInterop;
using ShareRazorClassLibrary.Helpers;
using ShareRazorClassLibrary.Models.Dtos;
using ShareRazorClassLibrary.Services;

namespace TozawaNGO.Shared
{
    public partial class AppBar : BaseComponent
    {
        [Parameter]
        public EventCallback OnSidebarToggled { get; set; }
        [Inject] ILocalStorageService _localStorageService { get; set; }
        [Inject] private IDialogService DialogService { get; set; }
        [Inject] LoadingState LoadingState { get; set; }
        [Inject] FirstloadState FirstloadState { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }
        [Inject] private NavMenuTabState NavMenuTabState { get; set; }
        [Inject] NavigationManager _navigationManager { get; set; }
        [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        public string _loginUrl { get; set; } = $"";
        private bool _showLogo = false;

        protected async override Task OnInitializedAsync()
        {
            NavMenuTabState.OnChange += HandleLogo;
            FirstloadState.OnChange += FirsLoadChanged;
            _translationService.LanguageChanged += _translationService_LanguageChanged;
            _authStateProvider.UserAuthenticationChanged += _authStateProvider_UserAuthChanged;

            await base.OnInitializedAsync();
        }

        private void FirsLoadChanged()
        {
            InvokeAsync(() =>
          {
              StateHasChanged();
          });
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
            _showLogo = !_showLogo;
            await OnSidebarToggled.InvokeAsync();
        }
        private static string Decode(string param)
        {
            return HttpUtility.UrlDecode(param);
        }
        private async Task Login()
        {
            var context = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            if (context.User.Identity == null || !context.User.Identity.IsAuthenticated)
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

                        LoadingState.SetRequestInProgress(false);
                        _authStateProvider.SetFirstLoad(true);

                        ((AuthStateProvider)_authStateProvider).NotifyUserAuthentication();

                        _currentUser = await _currentUserService.GetCurrentUser();
                        FirstloadState.SetFirsLoad(true);
                    }
                }
            }
        }
        private async Task Logout()
        {
            await _localStorageService.RemoveItemAsync("authToken");
            await _localStorageService.RemoveItemAsync("refreshToken");

            ((AuthStateProvider)_authStateProvider).NotifyUserLogout();

            FirstloadState.SetFirsLoad(true);
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
                _authStateProvider.SetFirstLoad(firstRender);
                var auth = await _authStateProvider.GetAuthenticationStateAsync();
                if (auth.User.Identity != null && auth.User.Identity.IsAuthenticated)
                {
                    ((AuthStateProvider)_authStateProvider).NotifyUserAuthentication();

                    _currentUser = await _currentUserService.GetCurrentUser();
                }

                await Task.Delay(new TimeSpan(0, 0, Convert.ToInt32(0.1))).ContinueWith(o =>
                {
                    InvokeAsync(() =>
                    {
                        FirstloadState.SetFirsLoad(true);
                    });
                });
            }
            await base.OnAfterRenderAsync(firstRender);
        }
        protected override void Dispose(bool disposed)
        {
            NavMenuTabState.OnChange -= HandleLogo;
            FirstloadState.OnChange -= FirsLoadChanged;
            _translationService.LanguageChanged -= _translationService_LanguageChanged;
            _authStateProvider.UserAuthenticationChanged -= _authStateProvider_UserAuthChanged;
            base.Dispose(disposed);
        }
    }
}
