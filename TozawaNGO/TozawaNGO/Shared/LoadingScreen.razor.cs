using Microsoft.AspNetCore.Components;
using TozawaNGO.Models.Dtos;
using TozawaNGO.Services;

namespace TozawaNGO.Shared
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

        private void CloseLoading()
        {
            _RequestInProgress = LoadingState.RequestInProgress;

            StateHasChanged();
        }
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
        public void Dispose()
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
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
                StateHasChanged();
            }
        }
    }
}