namespace TozawaMauiHybrid.Components
{
    public partial class Routes
    {
        private bool _firstLoaded = false;
        private bool _RequestInProgress = false;
        protected override async Task OnInitializedAsync()
        {
            _RequestInProgress = true;
            await InvokeAsync(() =>
         {
             StateHasChanged();
         });
            await base.OnInitializedAsync();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _firstLoaded = true;
                _RequestInProgress = false;
                await InvokeAsync(() =>
         {
             StateHasChanged();
         });
            }
            await base.OnAfterRenderAsync(firstRender);
        }
        public virtual void Dispose()
        {
        }

    }
}
