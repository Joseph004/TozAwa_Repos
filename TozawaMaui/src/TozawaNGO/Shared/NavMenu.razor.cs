using Microsoft.AspNetCore.Components;
using ShareRazorClassLibrary.Services;

namespace TozawaNGO.Shared
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

        protected override void Dispose(bool disposed)
        {
            FirsloadState.OnChange -= FirsLoadChanged;
            _translationService.LanguageChanged -= _translationService_LanguageChanged;
            _authStateProvider.UserAuthenticationChanged -= _authStateProvider_UserAuthChanged;
            base.Dispose(disposed);
        }
    }
}
