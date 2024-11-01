
namespace ShareRazorClassLibrary.Services
{
	public class NavMenuTabState
	{
		public ActiveTab ActiveTab { get; private set; } = ActiveTab.Home;
		public bool IsMenuOpen { get; private set; }
		public event Action OnChange;

		public void SetActiveTab(ActiveTab tab)
		{
			ActiveTab = tab;
			NotifyStateChanged();
		}
		public void SetMenuOpen(bool isMenuOpen)
		{
			IsMenuOpen = isMenuOpen;
			NotifyStateChanged();
		}
		private void NotifyStateChanged() => OnChange.Invoke();
	}

	public enum ActiveTab
	{
		Home = 1,
		Counter = 2,
		Weather = 3,
		Settings = 4
	}
}
