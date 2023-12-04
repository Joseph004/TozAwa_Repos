using System.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MudBlazor;
using TozawaNGO.Services;
using System.Timers;
using Timer = System.Timers.Timer;
using TozawaNGO.Auth.Models;

namespace TozawaNGO.Shared
{
    public partial class MainLayout : BaseComponentLayout
    {
        [Inject] IJSRuntime JSRuntime { get; set; }
        [Inject] NavigationManager _navigationManager { get; set; }
        [Inject] ICurrentUserService CurrentUserService { get; set; }
        [Inject] private IDialogService DialogService { get; set; }
        [Inject] LoadingState LoadingState { get; set; }
        [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        public string _loginUrl { get; set; } = $"";
        private Timer _timer;
        private readonly int _timerInterval = 15 * 60 * 1000; //15 min
        private bool _disabledPage = false;
        private string _disableAttrString = "";

        private bool _sidebarOpen = true;
        private void ToggleTheme(MudTheme changedTheme) => _currentTheme = changedTheme;
        private void ToggleSidebar()
        {
            _sidebarOpen = !_sidebarOpen;
            StateHasChanged();
        }


        private MudTheme _currentTheme = new MudTheme
        {
            Palette = new Palette()
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
            try
            {
                if (firstRender)
                {
                    _timer = new Timer(_timerInterval);
                    _timer.Elapsed += LogoutTimeout;
                    _timer.AutoReset = false;
                    _timer.Start();

                    //await JSRuntime.InitializeMudBlazorExtensionsAsync();

                    //await SetUserClaims();
                    StateHasChanged();
                }

                await base.OnAfterRenderAsync(firstRender);
            }
            catch (JSDisconnectedException)
            {
            }
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
        private void RefreshTimer(EventArgs e)
        {
            _timer.Interval = _timerInterval;
        }
        public override void Dispose()
        {
            LoadingState.OnChange -= DisabledPage;
            if (_timer != null)
            {
                _timer.Elapsed -= LogoutTimeout;
            }
        }
    }
}
