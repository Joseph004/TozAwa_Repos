using System.Web;
using Microsoft.AspNetCore.Components;
using Blazored.LocalStorage;
using ShareRazorClassLibrary.Services;
using MudBlazor;
using Microsoft.AspNetCore.Components.Web;

namespace TozawaNGO.Shared
{
    public partial class AppBar : BaseComponent<AppBar>
    {
        [Parameter]
        public EventCallback OnSidebarToggled { get; set; }
        [Inject] ILocalStorageService _localStorageService { get; set; }
        [Inject] FirstloadState FirstloadState { get; set; }
        [Inject] private NavMenuTabState NavMenuTabState { get; set; }
        SearchCommand model = new();
        private string[] _errors = [];
        private bool _showLogo = false;
        private bool _isSearchOpen = false;

        private void GoToHome()
        {
            var homePage = NavManager.ToBaseRelativePath(NavManager.Uri);
            var activePath = NavMenuTabState.GetActivePath();
            if (!string.IsNullOrEmpty(homePage) && activePath != "/")
            {
                NavManager.NavigateTo("/");
            }
        }
        protected async override Task OnInitializedAsync()
        {
            NavMenuTabState.OnChange += HandleLogo;
            FirstloadState.OnChange += FirsLoadChanged;
            _translationService.LanguageChanged += _translationService_LanguageChanged;
            _authStateProvider.UserAuthenticationChanged += _authStateProvider_UserAuthChanged;

            await base.OnInitializedAsync();
        }
        private async Task OnSearch()
        {
            _errors = [];
            if (!string.IsNullOrEmpty(model.SearchString))
            {

            }
        }
        private async Task OpenSearch()
        {
            _isSearchOpen = !_isSearchOpen;
            await InvokeAsync(() =>
           {
               StateHasChanged();
           });
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
            await InvokeAsync(() =>
           {
               StateHasChanged();
           });
        }
        private void _translationService_LanguageChanged(object sender, EventArgs e)
        {
            InvokeAsync(() =>
          {
              StateHasChanged();
          });
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

        private async Task Register()
        {
            await Task.FromResult(1);
        }
        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
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
        protected override void Dispose(bool disposed)
        {
            NavMenuTabState.OnChange -= HandleLogo;
            FirstloadState.OnChange -= FirsLoadChanged;
            _translationService.LanguageChanged -= _translationService_LanguageChanged;
            _authStateProvider.UserAuthenticationChanged -= _authStateProvider_UserAuthChanged;
            base.Dispose(disposed);
        }
    }
    public class SearchCommand
    {
        public string SearchString { get; set; } = "";
    }
}
