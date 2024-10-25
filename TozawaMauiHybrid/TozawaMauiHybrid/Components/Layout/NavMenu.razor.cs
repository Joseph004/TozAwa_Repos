using Microsoft.AspNetCore.Components;
using TozawaMauiHybrid.Component;
using TozawaMauiHybrid.Services;

namespace TozawaMauiHybrid.Components.Layout
{
    public partial class NavMenu : BaseComponent
    {
        [Parameter]
        public bool SideBarOpen { get; set; }
        [Inject] FirsloadState FirsloadState { get; set; }


        protected async override Task OnInitializedAsync()
        {
            FirsloadState.OnChange += FirsLoadChanged;
            _translationService.LanguageChanged += _translationService_LanguageChanged;
            _authStateProvider.UserAuthenticationChanged += _authStateProvider_UserAuthChanged;

            await base.OnInitializedAsync();
        }
        private void FirsLoadChanged()
        {
            StateHasChanged();
        }
        private void _authStateProvider_UserAuthChanged(object sender, EventArgs e)
        {
            StateHasChanged();
        }
        private void _translationService_LanguageChanged(object sender, EventArgs e)
        {
            StateHasChanged();
        }

        public virtual void Dispose()
        {
            FirsloadState.OnChange -= FirsLoadChanged;
            _translationService.LanguageChanged -= _translationService_LanguageChanged;
            _authStateProvider.UserAuthenticationChanged -= _authStateProvider_UserAuthChanged;
        }
    }
}
