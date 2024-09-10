
namespace TozawaNGO.StateHandler
{
    public class ScrollTopState
    {
        public Dictionary<string, double> ScrollTop { get; private set; } = [];
        public string Source { get; private set; }

        public event Action OnChange;

        public void SetScrollTop(double scrollTop, string source)
        {
            ScrollTop[source] = scrollTop;
            NotifyStateChanged();
        }
        public void SetSource(string source)
        {
            Source = source;
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}