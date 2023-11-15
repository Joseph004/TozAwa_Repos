using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Timers;
using Timer = System.Timers.Timer;
using MudBlazor;
using Tozawa.Client.Portal.Services;
using Microsoft.JSInterop;
using MudBlazor.Extensions.Helper;
using System.Web;
using TozAwa.Client.Portal.Services;

namespace Tozawa.Client.Portal.Shared;
public partial class MainLayout : LayoutComponentBase
{
    [Inject] protected AuthenticationStateProvider AuthenticationStateProvider { get; set; }
    [Inject] private IDialogService DialogService { get; set; }
    [Inject] NavigationManager _navigationManager { get; set; }
    [Inject] ICurrentUserService CurrentUserService { get; set; }
    [Inject] IJSRuntime JSRuntime { get; set; }
    [Inject] ISnackbar Snackbar { get; set; }

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
    private readonly int _timerInterval = 15 * 60 * 1000; //15 min
    private bool _disabledPage = false;
    private string _disableAttrString = "";
    [Inject] LoadingState LoadingState { get; set; }

    protected override void OnInitialized()
    {
        LoadingState.OnChange += DisabledPage;
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
            _timer.Elapsed += Logout;
            _timer.AutoReset = false;
            _timer.Start();

            await JSRuntime.InitializeMudBlazorExtensionsAsync();

            await JSRuntime.InvokeVoidAsync("AddTextTest");
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

                      var currentPath = _navigationManager.Uri.Split(_navigationManager.BaseUri)[1];
                      var returnUrl = string.IsNullOrEmpty(currentPath) ? "" : currentPath;
                      var _logoutUrl = $"/logout?returnUrl={Encode(returnUrl)}";
                      await JSRuntime.InvokeVoidAsync("open", _logoutUrl, "_top");
                  }
              }
          });
    }
    private string Encode(string param)
    {
        return HttpUtility.UrlEncode(param);
    }
    private void RefreshTimer(EventArgs e)
    {
        _timer.Interval = _timerInterval;
    }

    public void Dispose()
    {
        LoadingState.OnChange -= DisabledPage;
        if (_timer != null)
        {
            _timer.Elapsed -= Logout;
        }
    }
}