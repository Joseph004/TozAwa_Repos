using System.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MudBlazor;
using System.Timers;
using Timer = System.Timers.Timer;
using Blazored.LocalStorage;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Components.Web;
using ShareRazorClassLibrary.Services;
using ShareRazorClassLibrary.Configurations;
using ShareRazorClassLibrary.Helpers;
using MudBlazor.Extensions.Options;
using MudBlazor.Extensions;
using TozawaNGO.Helpers;
using Nextended.Core.Extensions;
using MudBlazor.Extensions.Core;
using ShareRazorClassLibrary.Models.Dtos;

namespace TozawaNGO.Shared
{
    public partial class MainLayout : BaseComponentLayout
    {
        [Inject] private NavMenuTabState NavMenuTabState { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }
        [Inject] ILogger<MainLayout> _logger { get; set; }
        [Inject] NavigationManager _navigationManager { get; set; }
        [Inject] ICurrentUserService CurrentUserService { get; set; }
        [Inject] AppSettings _appSettings { get; set; }
        [Inject] private IDialogService DialogService { get; set; }
        [Inject] LoadingState LoadingState { get; set; }
        [Inject] ILocalStorageService _localStorageService { get; set; }
        [Inject] AuthenticationService AuthenticationService { get; set; }
        [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        public string _loginUrl { get; set; } = $"";
        private string _containerPaddingClass { get; set; } = "pt-16 px-16 flex-1 d-flex";
        private Timer _timer;
        private readonly int _timerInterval = 15 * 60 * 1000; //15 min
        private bool _disabledPage = false;
        private string _disableAttrString = "";
        private ErrorBoundary _errorBoundary;
        private bool _sidebarOpen = true;
        private bool _firstLoaded = false;

        private void ToggleSidebar()
        {
            _sidebarOpen = !_sidebarOpen;
            NavMenuTabState.SetMenuOpen(_sidebarOpen);
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
            PaletteLight = new PaletteLight()
            {
                AppbarBackground = "#000000"
            }
        };
        protected async override Task OnInitializedAsync()
        {
            NavMenuTabState.OnChange += StateHasChanged;
            LoadingState.OnChange += DisabledPage;
            await base.OnInitializedAsync();
        }
        private async void DisabledPage()
        {
            _disabledPage = LoadingState.RequestInProgress;

            _disableAttrString = _disabledPage ? "disabledPage" : "";
            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var userResponse = new LoginResponseDto();
                if (await _localStorageService.ContainKeyAsync("auth_loggedIn"))
                {
                    var idBytes = EncryptDecrypt.Decrypt(await _localStorageService.GetItemAsync<byte[]>("auth_loggedIn"),
                    "PG=?1PowK<ai57:t%`Ro}L9~1q2&-i/H", "HK2nvSMadZRDeTbB");
                    if (!string.IsNullOrEmpty(Encoding.UTF8.GetString(idBytes)))
                    {
                        var response = await AuthenticationService.GetLoggedIn(Guid.Parse(Encoding.UTF8.GetString(idBytes)));
                        if (response.Success && response.Entity != null)
                        {
                            userResponse = response.Entity;
                            if (((AuthStateProvider)_authStateProvider).ValidateCurrentToken(response.Entity.Token))
                            {
                                ((AuthStateProvider)_authStateProvider).UserLoginStateDto.Set(true, response.Entity.Token,
                                response.Entity.RefreshToken);
                                await ((AuthStateProvider)_authStateProvider).NotifyUserAuthentication(response.Entity.Token, response.Entity.RefreshToken);
                            }
                            else
                            {
                                await ((AuthStateProvider)_authStateProvider).NotifyUserLogout();
                            }
                        }
                    }
                }

                NavMenuTabState.SetMenuOpen(_sidebarOpen);
                _timer = new Timer(_timerInterval);
                _timer.Elapsed += LogoutTimeout;
                _timer.AutoReset = false;
                _timer.Start();
                _firstLoaded = true;
                StateHasChanged();
                await LogoutIfUserExipired(userResponse);
                await base.OnAfterRenderAsync(firstRender);
            }
        }
        private async Task LogoutIfUserExipired(LoginResponseDto loginResponse)
        {
            var auth = await _authStateProvider.GetAuthenticationStateAsync();

            if (auth.User.Identity != null && auth.User.Identity.IsAuthenticated)
            {
                if (!string.IsNullOrEmpty(loginResponse.Token) && !string.IsNullOrEmpty(loginResponse.RefreshToken) && !ValidateCurrentToken(loginResponse.Token))
                {
                    var exp = auth.User.Claims.First(x => x.Type == "logoutexpat").Value;
                    var year = Convert.ToInt32(exp.Split("/")[2].Split(":")[0].Split(" ")[0]);
                    var month = Convert.ToInt32(exp.Split("/")[1]);
                    var day = Convert.ToInt32(exp.Split("/")[0]);
                    var hour = Convert.ToInt32(exp.Split("/")[2].Split(":")[0].Split(" ")[1]);
                    var minute = Convert.ToInt32(exp.Split("/")[2].Split(":")[1]);
                    var seconde = Convert.ToInt32(exp.Split("/")[2].Split(":")[2]);
                    var currentDate = DateTime.UtcNow;
                    var expDate = new DateTime(year, month, day, hour, minute, seconde);

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not logout user");
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
                  if (context.User.Identity != null && context.User.Identity.IsAuthenticated)
                  {
                      var parameters = new DialogParameters
                      {
                          ["Title"] = "Logout"
                      };
                      var options = new DialogOptionsEx
                      {
                          BackgroundClass = "tz-mud-overlay",
                          BackdropClick = false,
                          CloseButton = false,
                          MaxWidth = MaxWidth.Small,
                          MaximizeButton = true,
                          FullHeight = false,
                          FullWidth = true,
                          DragMode = MudDialogDragMode.Simple,
                          Animations = [AnimationType.Pulse],
                          Position = DialogPosition.Center
                      };

                      options.SetProperties(ex => ex.Resizeable = true);
                      options.DialogAppearance = MudExAppearance.FromStyle(b =>
                      {
                          b.WithBackgroundColor("gold")
                          .WithOpacity(0.9);
                      });

                      var dialog = await DialogService.ShowEx<ExpireModal>("Logout", parameters, options);
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
            var user = await ((AuthStateProvider)_authStateProvider).GetUserFromToken();
            await AuthenticationService.PostLogout(user.Id);
            await ((AuthStateProvider)_authStateProvider).NotifyUserLogout();
        }
        private void RefreshTimer(EventArgs e)
        {
            NavMenuTabState.SetMenuOpen(_sidebarOpen);
            if (_timer != null)
            {
                _timer.Interval = _timerInterval;
            }
        }

        public override void Dispose()
        {
            NavMenuTabState.OnChange -= StateHasChanged;
            LoadingState.OnChange -= DisabledPage;
            if (_timer != null)
            {
                _timer.Elapsed -= LogoutTimeout;
            }
            base.Dispose();
        }
    }
}
