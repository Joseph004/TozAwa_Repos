using System.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System.Timers;
using Timer = System.Timers.Timer;
using System.Text;
using Microsoft.AspNetCore.Components.Web;
using TozawaMauiHybrid.Component;
using TozawaMauiHybrid.Helpers;
using TozawaMauiHybrid.Services;
using TozawaMauiHybrid.Configurations;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using MudBlazor.Extensions.Options;
using MudBlazor.Extensions;
using TozawaMauiHybrid.Models.Dtos;
using MudBlazor.Extensions.Core;
using Nextended.Core.Extensions;

namespace TozawaMauiHybrid.Components.Layout
{
    public partial class MainLayout : BaseComponentLayout
    {
        [Inject] private NavMenuTabState NavMenuTabState { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }
        [Inject] NavigationManager _navigationManager { get; set; }
        [Inject] ICurrentUserService CurrentUserService { get; set; }
        [Inject] AppSettings _appSettings { get; set; }
        [Inject] private IDialogService DialogService { get; set; }
        [Inject] LoadingState LoadingState { get; set; }
        [Inject] FirstloadState FirstloadState { get; set; }
        [Inject] PreferencesStoreClone _storage { get; set; }
        [Inject] AuthenticationService AuthenticationService { get; set; }
        [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        private Timer _timer;
        private readonly int _timerInterval = 15 * 60 * 1000; //15 min
        private bool _disabledPage = false;
        private string _disableAttrString = "";
        private bool _firstLoaded = false;
        private bool _sidebarOpen = DeviceInfo.Platform == DevicePlatform.WinUI;
        private void ToggleSidebar()
        {
            _sidebarOpen = !_sidebarOpen;
            NavMenuTabState.SetMenuOpen(_sidebarOpen);
            InvokeAsync(() =>
        {
            StateHasChanged();
        });
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
        private MudTheme _currentTheme = new()
        {
            PaletteLight = new PaletteLight()
            {
                AppbarBackground = "#000000"
            }
        };
        protected async override Task OnInitializedAsync()
        {
            LoadingState.OnChange += DisabledPage;

            NavMenuTabState.OnChange += StateHasChanged;
            FirstloadState.OnChange += ReloadPage;
            await base.OnInitializedAsync();
        }
        public void ReloadPage()
        {
            InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }
        private void RefreshTimer(EventArgs e)
        {
            NavMenuTabState.SetMenuOpen(_sidebarOpen);
            if (_timer != null)
            {
                _timer.Interval = _timerInterval;
            }
        }
        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var userResponse = new LoginResponseDto();
                if (_storage.Exists("auth_loggedIn"))
                {
                    var idBytes = Cryptography.Decrypt(_storage.Get<byte[]>("auth_loggedIn"),
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
                                response.Entity.RefreshToken, _authStateProvider.UserLoginStateDto.WorkOrganizationId);
                                await ((AuthStateProvider)_authStateProvider).NotifyUserAuthentication(response.Entity.Token, response.Entity.RefreshToken);
                            }
                            else
                            {
                                ((AuthStateProvider)_authStateProvider).NotifyUserLogout();
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
                await InvokeAsync(() =>
         {
             StateHasChanged();
         });
                await LogoutIfUserExipired(userResponse);
                await base.OnAfterRenderAsync(firstRender);
            }
        }
        private async Task Login()
        {
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
                b.WithBackgroundImage("url('/images/plain-white-background.jpg')")
              .WithBackgroundSize("cover")
              .WithBackgroundPosition("center center")
              .WithBackgroundRepeat("no-repeat")
              .WithOpacity(0.9);
            });

            var parameters = new DialogParameters
            {
                ["Title"] = "Login"
            };

            var dialog = await DialogService.ShowEx<LoginViewModal>("Login", parameters, options);
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                var userResponse = (LoginResponseDto)result.Data;

                if (userResponse.LoginSuccess)
                {
                    LoadingState.SetRequestInProgress(false);
                    ((AuthStateProvider)_authStateProvider).UserLoginStateDto.Set(true, userResponse.Token, userResponse.RefreshToken, _authStateProvider.UserLoginStateDto.WorkOrganizationId);
                    await ((AuthStateProvider)_authStateProvider).NotifyUserAuthentication(userResponse.Token, userResponse.RefreshToken);
                }
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
            else
            {
                await Login();
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
                  if (FirstloadState.IsFirstLoaded)
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
                              Resizeable = true,
                              DragMode = MudDialogDragMode.Simple,
                              Position = DialogPosition.Center,
                              CloseButton = false,
                              MaxWidth = MaxWidth.Small
                          };

                          options.SetProperties(ex => ex.Resizeable = true);
                          options.DialogAppearance = MudExAppearance.FromStyle(b =>
                          {
                              b.WithBackgroundImage("url('/images/plain-white-background.jpg')")
                            .WithBackgroundSize("cover")
                            .WithBackgroundPosition("center center")
                            .WithBackgroundRepeat("no-repeat")
                            .WithOpacity(0.9);
                          });
                          var dialog = await DialogService.ShowEx<ExpireModal>("Logout", parameters, options);
                          var result = await dialog.Result;

                          if (!result.Canceled)
                          {
                              await Logout();
                          }
                      }
                  }
              });
        }
        private async Task Logout()
        {
            var user = await ((AuthStateProvider)_authStateProvider).GetUserFromToken();
            await AuthenticationService.PostLogout(user.Id);
            ((AuthStateProvider)_authStateProvider).NotifyUserLogout();
            await Login();
        }
        public override void Dispose()
        {
            NavMenuTabState.OnChange -= StateHasChanged;
            FirstloadState.OnChange -= ReloadPage;
            LoadingState.OnChange -= DisabledPage;
            if (_timer != null)
            {
                _timer.Elapsed -= LogoutTimeout;
            }
        }
    }
}
