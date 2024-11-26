using Microsoft.AspNetCore.Components;
using MudBlazor;
using Microsoft.JSInterop;
using ShareRazorClassLibrary.Helpers;
using ShareRazorClassLibrary.Models.Dtos;
using ShareRazorClassLibrary.Services;
using MudBlazor.Extensions.Options;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Core;
using MudBlazor.Extensions.Helper;
using Nextended.Core.Extensions;
using Microsoft.AspNetCore.Components.Web;

namespace TozawaNGO.Shared
{
    public partial class Login : BaseComponent<Login>, IDisposable
    {
        [Parameter]
        public EventCallback OnSidebarToggled { get; set; }
        [Inject] private IDialogService DialogService { get; set; }
        [Inject] LoadingState LoadingState { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }
        [Inject] FirstloadState FirstloadState { get; set; }
        private ErrorBoundary _errorBoundary;
        public DialogOptionsEx Options { get; set; }

        protected async override Task OnInitializedAsync()
        {
            _translationService.LanguageChanged += _translationService_LanguageChanged;
            _authStateProvider.UserAuthenticationChanged += _authStateProvider_UserAuthChanged;

            await base.OnInitializedAsync();
        }
        protected override void OnParametersSet()
        {
            _errorBoundary?.Recover();
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
        private void CreateOptions()
        {
            Options = new DialogOptionsEx
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

            Options.SetProperties(ex => ex.Resizeable = true);
            Options.DialogAppearance = MudExAppearance.FromStyle(b =>
            {
                b.WithBackgroundColor("gold")
                .WithOpacity(0.9);
            });
            Options.DialogBackgroundAppearance = MudExAppearance.FromCss(MudExCss.Classes.Backgrounds.EmptyIndicator)
                .WithStyle(b => b.WithOpacity(0.5));
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

                var dialog = await DialogService.ShowEx<LoginViewModal>("Login", parameters, Options);
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
                CreateOptions();
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
