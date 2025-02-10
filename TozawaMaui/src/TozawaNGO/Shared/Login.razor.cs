using Microsoft.AspNetCore.Components;
using MudBlazor;
using ShareRazorClassLibrary.Helpers;
using ShareRazorClassLibrary.Models.Dtos;
using ShareRazorClassLibrary.Services;
using MudBlazor.Extensions.Options;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Core;
using Nextended.Core.Extensions;
using TozawaNGO.Services;
using System.Threading.Tasks;

namespace TozawaNGO.Shared
{
    public partial class Login : BaseComponent<Login>, IDisposable
    {
        [Parameter]
        public EventCallback OnSidebarToggled { get; set; }
        [Inject] private IDialogService DialogService { get; set; }
        [Inject] private MemberService MemberService { get; set; }
        [Inject] private NavMenuTabState NavMenuTabState { get; set; }
        [Inject] LoadingState LoadingState { get; set; }
        [Inject] public ISnackbar Snackbar { get; set; }
        [Inject] FirstloadState FirstloadState { get; set; }
        [Inject] AuthenticationService AuthenticationService { get; set; }
        public DialogOptionsEx Options { get; set; }
        public CurrentUserOrganizationDto SelectedOrganization = new();
        public List<CurrentUserOrganizationDto> CurrentUserOrganizations = [];
        public SelectionMode SelectionMode = SelectionMode.SingleSelection;
        public bool _disabledMenu = false;
        MudMenu _mudMenuRef = new();
        protected async override Task OnInitializedAsync()
        {
            _translationService.LanguageChanged += _translationService_LanguageChanged;
            _authStateProvider.UserAuthenticationChanged += _authStateProvider_UserAuthChanged;

            await base.OnInitializedAsync();
        }
        private async void _authStateProvider_UserAuthChanged(object sender, EventArgs e)
        {
            await Task.FromResult(1);
            await InvokeAsync(async () =>
           {
               await SetCurrentUser(SelectedOrganization?.Id == Guid.Empty ? null : SelectedOrganization?.Id);
               RedirectPage();
               StateHasChanged();
           });
        }
        private void RedirectPage()
        {
            if (NavMenuTabState.ActiveTab == ActiveTab.Register)
            {
                NavManager.NavigateTo(NavMenuTabState.GetTabPath(ActiveTab.Home));
            }
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
                b.WithBackgroundImage("url('/images/plain-white-background.jpg')")
                .WithBackgroundSize("cover")
                .WithBackgroundPosition("center center")
                .WithBackgroundRepeat("no-repeat")
                .WithOpacity(0.9);
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

                var dialog = await DialogService.ShowEx<LoginViewModal>("Login", parameters, Options);
                var result = await dialog.Result;

                if (!result.Canceled)
                {
                    var userResponse = (LoginResponseDto)result.Data;

                    if (userResponse.LoginSuccess)
                    {
                        LoadingState.SetRequestInProgress(false);
                        ((AuthStateProvider)_authStateProvider).UserLoginStateDto.Set(true, userResponse.Token, userResponse.RefreshToken, Guid.Empty);
                        await ((AuthStateProvider)_authStateProvider).NotifyUserAuthentication(userResponse.Token, userResponse.RefreshToken);
                        await SetCurrentUser();
                    }
                }
            }
        }
        private async Task Logout()
        {
            var user = await ((AuthStateProvider)_authStateProvider).GetUserFromToken();
            await AuthenticationService.PostLogout(user.Id);
            ((AuthStateProvider)_authStateProvider).UserLoginStateDto.Set(false, null, null, Guid.Empty);
            await ((AuthStateProvider)_authStateProvider).NotifyUserLogout();
        }
        private async Task OnClickTab(string link)
        {
            var previousTab = NavMenuTabState.ActiveTab;
            if (NavMenuTabState.GetTabPath(previousTab) != link)
            {
                NavMenuTabState.SetPreviousTab(previousTab);
                NavManager.NavigateTo(link);
            }
            await Task.CompletedTask;
        }
        private async Task SetCurrentUser(Guid? orgId = null)
        {
            var user = await _currentUserService.GetCurrentUser();
            CurrentUserOrganizations = user.Organizations;
            SelectedOrganization = CurrentUserOrganizations?.FirstOrDefault(x => (orgId.HasValue ? x.Id == orgId : x.PrimaryOrganization));
            if (user != null && user.Id != Guid.Empty)
            {
                ((AuthStateProvider)_authStateProvider).UserLoginStateDto.Set(true, _authStateProvider.UserLoginStateDto?.JWTToken, _authStateProvider.UserLoginStateDto?.JWTRefreshToken, SelectedOrganization.Id);
            }
        }
        private async Task SetWorkingOrganization(CurrentUserOrganizationDto org)
        {
            LoadingState.SetRequestInProgress(true);
            _disabledMenu = true;
            StateHasChanged();
            var response = await MemberService.SwitchOrganization(org.Id);
            if (response != null && response.Entity != null)
            {
                SelectedOrganization = org;
                var userResponse = response.Entity;
                ((AuthStateProvider)_authStateProvider).UserLoginStateDto.Set(true, userResponse.Token, userResponse.RefreshToken, SelectedOrganization.Id);
                await ((AuthStateProvider)_authStateProvider).NotifyUserAuthentication(userResponse.Token, userResponse.RefreshToken);
                await SetCurrentUser(SelectedOrganization.Id);
                await _mudMenuRef.CloseMenuAsync();
                Snackbar.Add("Your organization has been switched.", Severity.Info);
            }
            else
            {
                Snackbar.Add(Translate(SystemTextId.Error, "Error"), Severity.Info);
            }
            LoadingState.SetRequestInProgress(false);
            _disabledMenu = false;
            StateHasChanged();
        }
        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await SetCurrentUser();
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
        protected override void Dispose(bool disposed)
        {
            _translationService.LanguageChanged -= _translationService_LanguageChanged;
            _authStateProvider.UserAuthenticationChanged -= _authStateProvider_UserAuthChanged;
            base.Dispose(disposed);
        }
    }
}
