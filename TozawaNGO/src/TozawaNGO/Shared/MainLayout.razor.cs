using System.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MudBlazor;
using TozawaNGO.Services;
using System.Timers;
using Timer = System.Timers.Timer;
using Blazored.LocalStorage;
using TozawaNGO.Helpers;
using TozawaNGO.Configurations;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Components.Web;

namespace TozawaNGO.Shared
{
    public partial class MainLayout : BaseComponentLayout
    {
        [Inject] IJSRuntime JSRuntime { get; set; }
        [Inject] NavigationManager _navigationManager { get; set; }
        [Inject] ICurrentUserService CurrentUserService { get; set; }
        [Inject] AppSettings _appSettings { get; set; }
        [Inject] private IDialogService DialogService { get; set; }
        [Inject] LoadingState LoadingState { get; set; }
        [Inject] ILocalStorageService _localStorageService { get; set; }
        [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; }
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
        public string OnError(Exception ex)
        {
            return _translationService.Translate(SystemTextId.ErrorOccursPleaseContactSupport, "Opps, something went wrong. Please contact support!").Text;
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
            LoadingState.OnChange += DisabledPage;
            await base.OnInitializedAsync();
        }
        private void DisabledPage()
        {
            _disabledPage = LoadingState.RequestInProgress;

            _disableAttrString = _disabledPage ? "disabledPage" : "";

            StateHasChanged();
        }
        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _timer = new Timer(_timerInterval);
                _timer.Elapsed += LogoutTimeout;
                _timer.AutoReset = false;
                _timer.Start();

                await LogoutIfUserExipired();
                StateHasChanged();
            }

            await base.OnAfterRenderAsync(firstRender);
        }
        private async Task LogoutIfUserExipired()
        {
            var auth = await _authStateProvider.GetAuthenticationStateAsync();

            if (auth.User.Identity.IsAuthenticated)
            {
                var token = await _localStorageService.GetItemAsync<string>("authToken");
                var refreshToken = await _localStorageService.GetItemAsync<string>("refreshToken");

                if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(refreshToken) && !ValidateCurrentToken(token))
                {
                    var exp = auth.User.Claims.First(x => x.Type == "logoutexpat").Value;
                    var currentDate = DateTime.UtcNow;
                    var expDate = DateTime.Parse(exp);

                    if (currentDate > expDate)
                    {
                        await Logout();
                    }
                }
            }
        }
        private bool ValidateCurrentToken(string token)
        {
            var myIssuer = _appSettings.JWTSettings.ValidIssuer;
            var myAudience = _appSettings.JWTSettings.ValidAudience;

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = myIssuer,
                    ValidAudience = myAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JWTSettings.SecurityKey))
                }, out SecurityToken validatedToken);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        private static string Decode(string param)
        {
            return HttpUtility.UrlDecode(param);
        }

        private void LogoutTimeout(Object source, ElapsedEventArgs e)
        {
            InvokeAsync(async () =>
              {
                  var context = await AuthenticationStateProvider.GetAuthenticationStateAsync();
                  if (context.User.Identity.IsAuthenticated)
                  {
                      var parameters = new DialogParameters
                      {
                          ["Title"] = "Logout"
                      };
                      DialogOptions options = new() { DisableBackdropClick = true, Position = DialogPosition.TopCenter };
                      var dialog = DialogService.Show<ExpireModal>("Logout", parameters, options);
                      var result = await dialog.Result;

                      if (!result.Canceled)
                      {
                          await Logout();
                      }
                  }
              });
        }
        private async Task Logout()
        {
            await _localStorageService.RemoveItemAsync("authToken");
            await _localStorageService.RemoveItemAsync("refreshToken");

            ((AuthStateProvider)_authStateProvider).NotifyUserLogout();

            var logoutUrl = $"logout{NavigateToReturnPage()}";
            await JSRuntime.InvokeVoidAsync("open", Decode(logoutUrl), "_top");
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
        private void RefreshTimer(EventArgs e)
        {
            _timer.Interval = _timerInterval;
        }
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
        public override void Dispose()
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
        {
            LoadingState.OnChange -= DisabledPage;
            if (_timer != null)
            {
                _timer.Elapsed -= LogoutTimeout;
            }
        }
    }
}
