using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MudBlazor;
using Newtonsoft.Json;
using Tozawa.Client.Portal.Models.Dtos;
using Tozawa.Client.Portal.Services;
using TozAwa.Client.Portal;

namespace Tozawa.Client.Portal.Shared
{
    public partial class AppBar : BaseComponent
    {
        [Parameter]
        public EventCallback OnSidebarToggled { get; set; }
        [Parameter]
        public EventCallback<MudTheme> OnThemeToggled { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }
        [Inject] NavigationManager _navigationManager { get; set; }
        [Inject] ICurrentUserService CurrentUserService { get; set; }
        [Inject] private IDialogService DialogService { get; set; }
        [Inject] ILocalStorageService _localStorage { get; set; }
        [Inject] AuthenticationStateProvider _authStateProvider { get; set; }
        [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        private MudTheme _currentTheme = new MudTheme();
        public string _loginUrl { get; set; } = $"";
        private bool _isLightMode = true;
        private CurrentUserDto _currentUser { get; set; } = new();
        private bool _showLogo = true;

        protected async override Task OnInitializedAsync()
        {
            _translationService.LanguageChanged += _translationService_LanguageChanged;

            await base.OnInitializedAsync();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await SetUserRoot();
                StateHasChanged();
            }
        }
        private async Task SetUserRoot()
        {
            var context = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            if (context.User.Identity.IsAuthenticated)
            {
                _currentUser = await CurrentUserService.GetCurrentUser();
                if (_currentUser != null && _currentUser.RootUser)
                {
                    if (context.User.Claims.Any(x => x.Type == "root-user"))
                    {
                        if (!context.User.Claims.Where(x => x.Type == "root-user").Any(z => z.Value == "UserIsRoot"))
                        {
                            var claim = context.User.Claims.Where(x => x.Type == "root-user").Where(x => x.Type != "root-user");
                            context.User.Claims.Append(new Claim("root-user", "UserIsRoot"));
                        }
                    }
                    else
                    {
                        context.User.Claims.Append(new Claim("root-user", "UserIsRoot"));
                    }
                }
            }
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
                Palette = new Palette()
                {
                    AppbarBackground = "#000000"
                }
            };
            await OnThemeToggled.InvokeAsync(_currentTheme);
        }
        private string Encode(string param)
        {
            return HttpUtility.UrlEncode(param);
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
                DialogOptions options = new DialogOptions()
                {
                    DisableBackdropClick = true,
                    Position = DialogPosition.TopCenter,
                    MaxWidth = MaxWidth.Small,
                    CloseButton = false
                };
                var dialog = DialogService.Show<LoginViewModal>("Login", parameters, options);
                var result = await dialog.Result;

                if (!result.Cancelled)
                {
                    var userResponse = (LoginResponseDto)result.Data;
                    await _localStorage.SetItemAsync("authToken", userResponse.Token);
                    ((AuthStateProvider)_authStateProvider).NotifyUserAuthentication(userResponse.Token);

                    NavigateToReturnPage();
                }
            }
        }
        private void NavigateToReturnPage()
        {
            var currentPath = _navigationManager.Uri.Split(_navigationManager.BaseUri)[1];

            if (string.IsNullOrEmpty(currentPath))
            {
                _navigationManager.NavigateTo("/home");
            }
            else
            {
                _navigationManager.NavigateTo($"/{currentPath}");
            }
        }
        private async Task Logout()
        {
            await CurrentUserService.RemoveCurrentUser();
            await _localStorage.RemoveItemAsync("authToken");
            ((AuthStateProvider)_authStateProvider).NotifyUserLogout();

            NavigateToReturnPage();
        }
        private MudTheme GenerateDarkTheme() =>
            new MudTheme
            {
                Palette = new Palette()
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

        public override void Dispose()
        {
            _translationService.LanguageChanged -= _translationService_LanguageChanged;
        }
    }
}
