using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace TozawaNGO.Shared
{
    public partial class Footer : BaseComponent
    {
        [Inject] IJSRuntime JS { get; set; }

        protected async override Task OnInitializedAsync()
        {
            _translationService.LanguageChanged += _translationService_LanguageChanged;
            _authStateProvider.UserAuthenticationChanged += _authStateProvider_UserAuthChanged;

            await base.OnInitializedAsync();
        }
        private void _authStateProvider_UserAuthChanged(object sender, EventArgs e)
        {
            StateHasChanged();
        }
        private void _translationService_LanguageChanged(object sender, EventArgs e)
        {
            StateHasChanged();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await Task.FromResult(1);
            /*  await JS.InvokeAsync<string>("FooterResized", DotNetObjectReference.Create(this)); */
        }

        protected async Task ToggleSocialIcon()
        {
            await Task.FromResult(1);
        }

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
        public override void Dispose()
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
        {
            _translationService.LanguageChanged -= _translationService_LanguageChanged;
            _authStateProvider.UserAuthenticationChanged -= _authStateProvider_UserAuthChanged;
        }
    }
}

