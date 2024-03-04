
namespace TozawaNGO.StateHandler
{
    public class ScrollTopState
    {
        public double ScrollTop { get; private set; }

        public event Action OnChange;

        public void SetScrollTop(double scrollTop)
        {
            ScrollTop = scrollTop;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}