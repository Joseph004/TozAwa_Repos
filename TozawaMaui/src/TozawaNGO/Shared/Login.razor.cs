using Microsoft.AspNetCore.Components;
using MudBlazor;
using Microsoft.JSInterop;
using ShareRazorClassLibrary.Helpers;
using ShareRazorClassLibrary.Models.Dtos;
using ShareRazorClassLibrary.Services;
using MudBlazor.Extensions.Options;
using MudBlazor.Extensions;

namespace TozawaNGO.Shared
{
    public partial class Login : ComponentBase, IDisposable
    {
        [Parameter]
        public EventCallback OnSidebarToggled { get; set; }
        [Inject] private IDialogService DialogService { get; set; }
        [Inject] LoadingState LoadingState { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }
        [Inject] protected ITranslationService _translationService { get; set; }
        [Inject] protected AuthStateProvider _authStateProvider { get; set; }
        [Inject] FirstloadState FirstloadState { get; set; }
        public string _loginUrl { get; set; } = $"";
        private bool _showLogo = false;

        protected async override Task OnInitializedAsync()
        {
            _translationService.LanguageChanged += _translationService_LanguageChanged;
            _authStateProvider.UserAuthenticationChanged += _authStateProvider_UserAuthChanged;

            await base.OnInitializedAsync();
        }
        private async void _authStateProvider_UserAuthChanged(object sender, EventArgs e)
        {
            await Task.FromResult(1);
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
        private async Task LoginBtn()
        {
            var auth = await _authStateProvider.GetAuthenticationStateAsync();
            if (auth.User.Identity == null || !auth.User.Identity.IsAuthenticated)
            {
                var parameters = new DialogParameters
                {
                    ["Title"] = "Login"
                };
                var options = new DialogOptionsEx
                {
                    Resizeable = true,
                    BackdropClick = false,
                    DragMode = MudDialogDragMode.Simple,
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
                        LoadingState.SetRequestInProgress(false);
                        ((AuthStateProvider)_authStateProvider).UserLoginStateDto.Set(true, userResponse.Token, userResponse.RefreshToken);
                        await ((AuthStateProvider)_authStateProvider).NotifyUserAuthentication(userResponse.Token, userResponse.RefreshToken);
                    }
                }
            }
        }
        private void Logout()
        {
            ((AuthStateProvider)_authStateProvider).UserLoginStateDto.Set(false, null, null);
            ((AuthStateProvider)_authStateProvider).NotifyUserLogout();
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
            }
            await base.OnAfterRenderAsync(firstRender);
        }
        public virtual void Dispose()
        {
            _translationService.LanguageChanged -= _translationService_LanguageChanged;
            _authStateProvider.UserAuthenticationChanged -= _authStateProvider_UserAuthChanged;
        }
    }
}
