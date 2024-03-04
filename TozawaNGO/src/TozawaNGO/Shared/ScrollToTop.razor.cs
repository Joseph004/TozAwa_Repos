using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using TozawaNGO.StateHandler;

namespace TozawaNGO.Shared
{
    public partial class ScrollToTop
    {
        [Inject] ScrollTopState ScrollTopState { get; set; }
        public MudScrollToTop _mudScrollToTop;
        [Parameter]
        public double ScrollTop
        {
            get; set;
        }
        private void OnScroll(ScrollEventArgs e)
        {
            ScrollTop = e.FirstChildBoundingClientRect.Top;
            ScrollTopState.SetScrollTop(ScrollTop);
        }
    }
}

