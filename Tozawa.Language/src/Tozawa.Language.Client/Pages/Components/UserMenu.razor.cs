using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Tozawa.Language.Client.Models.Dtos;
using Tozawa.Language.Client.Services;

namespace Tozawa.Language.Client.Pages.Components
{
    public partial class UserMenu : ComponentBase
    {
        [Inject] private ICurrentUserService CurrentUserService { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }
        public CurrentUserDto CurrentUser { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
                CurrentUser = await CurrentUserService.GetCurrentUser();
        }

        private async Task Logout()
        {
            await CurrentUserService.RemoveCurrentUser();
            var _logoutUrl = $"/logout?returnUrl=/";
            await JSRuntime.InvokeVoidAsync("open", _logoutUrl, "_top");
        }
    }
}
