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

            await base.OnInitializedAsync();
        }

        private void _translationService_LanguageChanged(object sender, EventArgs e)
        {
            StateHasChanged();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
           /*  await JS.InvokeAsync<string>("FooterResized", DotNetObjectReference.Create(this)); */
        }

        protected async Task ToggleSocialIcon()
        {
        }

        public override void Dispose()
        {
            _translationService.LanguageChanged -= _translationService_LanguageChanged;
        }
    }
}

