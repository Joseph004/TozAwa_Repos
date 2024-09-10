using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TozawaNGO.Shared;
using TozawaNGO.State.Home.Store;
using TozawaNGO.StateHandler;

namespace TozawaNGO.Pages
{
    public partial class Home : BasePage
    {
        [Inject] IState<HomeState> HomeState { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }
        [Inject] IDispatcher Dispatcher { get; set; }
        [Inject] ScrollTopState ScrollTopState { get; set; }
        protected override void Dispose(bool disposed)
        {
            try
            {
                ScrollTopState.OnChange -= SetScroll;
            }
            catch
            {
            }
            base.Dispose(disposed);
        }
        protected override async Task OnInitializedAsync()
        {
            ScrollTopState.SetSource("homePage");
            ScrollTopState.OnChange += SetScroll;
            await base.OnInitializedAsync();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (HomeState.Value.ScrollTop != 0)
                {
                    await JSRuntime.InvokeAsync<object>("SetScroll", (-1) * HomeState.Value.ScrollTop);
                }
            }
            await base.OnAfterRenderAsync(firstRender);
        }
        private void SetScroll()
        {
            Dispatcher.Dispatch(new ScrollTopAction(ScrollTopState.ScrollTop[ScrollTopState.Source]));
        }
    }
}

