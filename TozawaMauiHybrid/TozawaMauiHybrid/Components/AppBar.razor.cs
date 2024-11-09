using System.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Microsoft.JSInterop;
using TozawaMauiHybrid.Helpers;
using TozawaMauiHybrid.Models.Dtos;
using TozawaMauiHybrid.Services;
using MudBlazor.Extensions.Options;
using MudBlazor.Extensions;

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
        [Inject] FirstloadState FirstloadState { get; set; }
        [Inject] NavigationManager _navigationManager { get; set; }
        [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        public string _loginUrl { get; set; } = $"";
        private bool _showLogo = DeviceInfo.Platform != DevicePlatform.WinUI;

        protected async override Task OnInitializedAsync()
        {
            NavMenuTabState.OnChange += HandleLogo;
            FirstloadState.OnChange += FirsLoadChanged;
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
            InvokeAsync(() =>
          {
              StateHasChanged();
          });
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
            if (context.User.Identity == null || !context.User.Identity.IsAuthenticated)
            {
                var parameters = new DialogParameters
                {
                    ["Title"] = "Login"
                };
                var options = new DialogOptionsEx
                {
                    Resizeable = true,
                    DragMode = MudDialogDragMode.Simple,
                    BackdropClick = false,
                    Position = DialogPosition.Center,
                    CloseButton = false,
                    MaxWidth = MaxWidth.Small
                };

                var dialog = await DialogService.ShowEx<LoginViewModal>("Login", parameters, options);
                var result = await dialog.Result;

                if (!result.Canceled)
                {
                    var userResponse = (LoginResponseDto)result.Data;

                    if (userResponse.LoginSuccess)
                    {
                        _storage.Set("authToken", userResponse.Token);
                        _storage.Set("refreshToken", userResponse.RefreshToken);

                        LoadingState.SetRequestInProgress(false);
                        _authStateProvider.SetFirstLoad(true);
                        await ((AuthStateProvider)_authStateProvider).NotifyUserAuthentication();

                        _currentUser = await _currentUserService.GetCurrentUser();
                        FirstloadState.SetFirsLoad(true);
                    }
                }
            }
        }
        private async Task Logout()
        {
            _storage.Delete("authToken");
            _storage.Delete("refreshToken");

            ((AuthStateProvider)_authStateProvider).NotifyUserLogout();
            StateHasChanged();

            FirstloadState.SetFirsLoad(true);
        }
        private async Task Register()
        {
            await Task.FromResult(1);
        }
        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _authStateProvider.SetFirstLoad(firstRender);
                var auth = await _authStateProvider.GetAuthenticationStateAsync();
                if (auth.User.Identity != null && auth.User.Identity.IsAuthenticated)
                {
                    await ((AuthStateProvider)_authStateProvider).NotifyUserAuthentication();

                    _currentUser = await _currentUserService.GetCurrentUser();
                }

                await Task.Delay(new TimeSpan(0, 0, Convert.ToInt32(0.1))).ContinueWith(o =>
                {
                    InvokeAsync(() =>
                    {
                        FirstloadState.SetFirsLoad(true);
                    });
                });
                await base.OnAfterRenderAsync(firstRender);
            }
        }
        public override void Dispose()
        {
            NavMenuTabState.OnChange -= HandleLogo;
            FirstloadState.OnChange -= FirsLoadChanged;
            _translationService.LanguageChanged -= _translationService_LanguageChanged;
            _authStateProvider.UserAuthenticationChanged -= _authStateProvider_UserAuthChanged;
        }
    }
}
