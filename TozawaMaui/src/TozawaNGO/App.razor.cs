namespace TozawaNGO
{
    public partial class App
    {
        private bool _firstLoaded = false;
        private bool _RequestInProgress = false;
        protected override async Task OnInitializedAsync()
        {
            _RequestInProgress = true;
            StateHasChanged();
            await base.OnInitializedAsync();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _firstLoaded = true;
                _RequestInProgress = false;
                StateHasChanged();
            }
            await base.OnAfterRenderAsync(firstRender);
        }
        public virtual void Dispose()
        {
        }

    }
}
