using System.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MudBlazor;
using TozawaNGO.Auth.Models;
using TozawaNGO.Models.Dtos;
using TozawaNGO.Helpers;
using Newtonsoft.Json;
using TozawaNGO.Auth.Services;
using Blazored.LocalStorage;

namespace TozawaNGO.Shared
{
    public partial class AppBar : BaseComponent
    {
        [Parameter]
        public EventCallback OnSidebarToggled { get; set; }
        [Parameter]
        public EventCallback<MudTheme> OnThemeToggled { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }
        [Inject] TokenProvider _tokenProvider { get; set; }
        [Inject] NavigationManager _navigationManager { get; set; }
        [Inject] ILocalStorageService _localStorageService { get; set; }
        [Inject] TozawaNGO.Services.ICurrentUserService CurrentUserService { get; set; }
        [Inject] IDataProtectionProviderService _provider { get; set; }
        [Inject] private IDialogService DialogService { get; set; }
        [Inject] AuthStateProvider _authStateProvider { get; set; }
        [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        private MudTheme _currentTheme = new MudTheme();
        public string _loginUrl { get; set; } = $"";
        private bool _isLightMode = true;
        private CurrentUserDto _currentUser { get; set; } = new();
        private bool _showLogo = false;

        protected async override Task OnInitializedAsync()
        {
            _translationService.LanguageChanged += _translationService_LanguageChanged;

            await base.OnInitializedAsync();
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
                        _tokenProvider.Token = userResponse.Token;
                        var userAboutToLogin = await _authStateProvider.GetUserFromToken(userResponse.Token);
                        _tokenProvider.Email = userAboutToLogin.Email;
                        _tokenProvider.UserName = userAboutToLogin.FirstName + " " + userAboutToLogin.LastName;
                        _tokenProvider.IsAdmin = userAboutToLogin.Admin;
                        _tokenProvider.Id = userAboutToLogin.Id.ToString();
                        _tokenProvider.ReturnUrl = NavigateToReturnPage();
                        _tokenProvider.RefreshToken = userResponse.RefreshToken;
                        _tokenProvider.ExpiresIn = userResponse.ExpiresIn;

                        var tokenSerialize = JsonConvert.SerializeObject(_tokenProvider);

                        var encryptToken = _provider.EncryptString("vtLJA1vT^qwrqhgtrdfvcj7_", tokenSerialize);

                        await _authStateProvider.SetCurrentUser(userAboutToLogin);
                        await _localStorageService.SetItemAsync("authToken", userResponse.Token);
                        await _localStorageService.SetItemAsync("refreshToken", userResponse.RefreshToken);

                        _loginUrl = $"login/{encryptToken.Replace("/", "_")}";
                        await JSRuntime.InvokeVoidAsync("open", _loginUrl, "_top");
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
            await CurrentUserService.RemoveCurrentUser();

            var logoutUrl = $"logout{NavigateToReturnPage()}";
            await JSRuntime.InvokeVoidAsync("open", Decode(logoutUrl), "_top");
        }
        private async Task Register()
        {

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

        public override void Dispose()
        {
            _translationService.LanguageChanged -= _translationService_LanguageChanged;
        }
    }
}
