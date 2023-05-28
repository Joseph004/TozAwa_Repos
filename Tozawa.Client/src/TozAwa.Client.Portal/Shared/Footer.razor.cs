using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MudBlazor;
using Tozawa.Client.Portal.HttpClients.Helpers;
using Tozawa.Client.Portal.Models.Dtos;
using Tozawa.Client.Portal.Services;

namespace Tozawa.Client.Portal.Shared
{
    public partial class Footer : BaseComponent
    {
        [Inject] IJSRuntime JS { get; set; }

        private string _flexColumn = "flex-column";

        protected async override Task OnInitializedAsync()
        {
            _translationService.LanguageChanged += _translationService_LanguageChanged;

            await base.OnInitializedAsync();
        }

        private void _translationService_LanguageChanged(object sender, EventArgs e)
        {
            JS.InvokeVoidAsync("AddTitleToInnerEmailIconSend", Translate(SystemTextId.Send, "Send"));
            StateHasChanged();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JS.InvokeVoidAsync("AddTitleToInnerEmailIconSend", Translate(SystemTextId.Send, "Send"));
            }
            await JS.InvokeAsync<string>("FooterSetToColumn", DotNetObjectReference.Create(this));

            await base.OnAfterRenderAsync(firstRender);
        }
        protected async Task ToggleSocialIcon()
        {
        }
        protected async Task ButtonEmailclick()
        {

        }

        public override void Dispose()
        {
            _translationService.LanguageChanged -= _translationService_LanguageChanged;
        }
    }
}
