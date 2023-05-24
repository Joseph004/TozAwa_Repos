using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Timers;
using Timer = System.Timers.Timer;
using MudBlazor;
using Tozawa.Language.Client.AuthenticationServices;
using System.Threading.Tasks;
using System;
using System.Linq;
using Tozawa.Language.Client.Services;
using Microsoft.JSInterop;

namespace Tozawa.Language.Client.Shared;
public partial class MainLayout : LayoutComponentBase
{
    [Inject] protected AuthenticationStateProvider AuthenticationStateProvider { get; set; }
    [Inject] private IDialogService DialogService { get; set; }
    [Inject] protected NavigationManager NavigationManager { get; set; }
    [Inject] ICurrentUserService CurrentUserService { get; set; }
    [Inject] IJSRuntime JSRuntime { get; set; }

    protected bool _drawerOpen = true;
    private static string CacheKey => "registry";

    private void DrawerToggle()
    {
        _drawerOpen = !_drawerOpen;
    }
    private MudTheme _currentTheme = new MudTheme
    {
        Palette = new Palette()
        {
            AppbarBackground = "#000000"
        }
    };
    private bool _sidebarOpen = false;
    private void ToggleTheme(MudTheme changedTheme) => _currentTheme = changedTheme;
    private void ToggleSidebar() => _sidebarOpen = !_sidebarOpen;
    private Timer _timer;
    private readonly int _timerInterval = 30 * 60 * 1000; //30 min

    protected async override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _timer = new Timer(_timerInterval);
            _timer.Elapsed += Logout;
            _timer.AutoReset = false;
            _timer.Start();
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private void Logout(Object source, ElapsedEventArgs e)
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
                  DialogOptions options = new DialogOptions() { DisableBackdropClick = true, Position = DialogPosition.TopCenter };
                  var dialog = DialogService.Show<ExpireModal>("Logout", parameters, options);
                  var result = await dialog.Result;

                  if (!result.Cancelled)
                  {
                      await CurrentUserService.RemoveCurrentUser();

                      var _logoutUrl = $"/logout?returnUrl=/";
                      await JSRuntime.InvokeVoidAsync("open", _logoutUrl, "_top");
                  }
              }
          });
    }

    private void RefreshTimer(EventArgs e)
    {
        _timer.Interval = _timerInterval;
    }

    void IDisposable.Dispose()
    {
        if (_timer != null)
        {
            _timer.Elapsed -= Logout;
        }
    }
}