using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MudBlazor;
using Tozawa.Language.Client.Models.Dtos;
using Tozawa.Language.Client.Services;

namespace Tozawa.Language.Client.Shared
{
    public partial class AppBar : ComponentBase
    {
        [Parameter]
        public EventCallback OnSidebarToggled { get; set; }
        [Parameter]
        public EventCallback<MudTheme> OnThemeToggled { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }
        [Inject] ICurrentUserService CurrentUserService { get; set; }
        [Inject] private IDialogService DialogService { get; set; }
        [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        private MudTheme _currentTheme = new MudTheme();
        public string _loginUrl { get; set; } = $"";

        private async Task ToggleSideBar()
        {
            await OnSidebarToggled.InvokeAsync();
        }
        private string Encode(string param)
        {
            return HttpUtility.UrlEncode(param);
        }
        private async Task Login()
        {
            var context = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            if (!context.User.Identity.IsAuthenticated)
            {
                var parameters = new DialogParameters
                {
                    ["Title"] = "Login"
                };
                DialogOptions options = new DialogOptions()
                {
                    DisableBackdropClick = true,
                    Position = DialogPosition.TopCenter,
                    FullWidth = true,
                    MaxWidth = MaxWidth.Medium,
                    CloseButton = false
                };
                var dialog = DialogService.Show<LoginViewModal>("Login", parameters, options);
                var result = await dialog.Result;

                if (!result.Cancelled)
                {
                    var user = (CurrentUserDto)result.Data;
                    if (user != null)
                    {
                        var rootUser = user.RootUser ? "UserIsRoot" : "NotNotRoot";
                        await CurrentUserService.SetCurrentUser(user);
                        _loginUrl = $"/login?paramUserName={Encode(user.UserName)}&paramRootUser={Encode(rootUser)}&paramUserId={Encode(user.Id.ToString())}";
                        await JSRuntime.InvokeVoidAsync("open", _loginUrl, "_top");
                    }
                }
            }
        }

        private MudTheme GenerateDarkTheme() =>
            new MudTheme
            {
                Palette = new Palette()
                {
                    Black = "#27272f",
                    Background = "#32333d",
                    BackgroundGrey = "#27272f",
                    Surface = "#373740",
                    TextPrimary = "#ffffffb3",
                    TextSecondary = "rgba(255,255,255, 0.50)",
                    AppbarBackground = "#000000",
                    AppbarText = "#ffffffb3",
                    DrawerBackground = "#27272f",
                    DrawerText = "#ffffffb3",
                    DrawerIcon = "#ffffffb3"
                }
            };
    }
}
