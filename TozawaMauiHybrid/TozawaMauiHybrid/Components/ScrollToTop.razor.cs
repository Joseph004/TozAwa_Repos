using Microsoft.AspNetCore.Components;
using MudBlazor;
using TozawaMauiHybrid.Services;

namespace TozawaMauiHybrid.Component
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
            ScrollTopState.SetScrollTop(ScrollTop, ScrollTopState.Source);
        }
    }
}

