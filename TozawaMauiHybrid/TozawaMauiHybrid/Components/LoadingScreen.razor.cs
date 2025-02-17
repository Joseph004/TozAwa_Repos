using Microsoft.AspNetCore.Components;
using TozawaMauiHybrid.Services;

namespace TozawaMauiHybrid.Component
{
    public partial class LoadingScreen : ComponentBase
    {
        bool isLoaded;

        [Parameter]
        public RenderFragment ChildContent { get; set; }
        [Inject] LoadingState LoadingState { get; set; }
        [Inject] private ITranslationService _traslationService { get; set; }
        private bool _RequestInProgress = false;

        protected override async Task OnInitializedAsync()
        {
            LoadingState.OnChange += CloseLoading;

            await base.OnInitializedAsync();
        }

        private async void CloseLoading()
        {
            _RequestInProgress = LoadingState.RequestInProgress;

            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }
        public virtual void Dispose()
        {
            LoadingState.OnChange -= CloseLoading;
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                //await Task.Delay(4000); // TODO actual initialization job
                await _traslationService.EnsureTranslations();
                isLoaded = true;
                await InvokeAsync(() =>
         {
             StateHasChanged();
         });
            }
        }
    }
}