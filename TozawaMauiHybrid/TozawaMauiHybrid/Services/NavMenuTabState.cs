
using TozawaMauiHybrid.Helpers;

namespace TozawaMauiHybrid.Services
{
	public class NavMenuTabState(PreferencesStoreClone storage)
	{
		private PreferencesStoreClone _storage = storage;

		public ActiveTab ActiveTab { get; private set; } = ActiveTab.Home;
		public ActiveTab PreviousTab { get; private set; } = ActiveTab.Home;
		public bool IsMenuOpen { get; private set; }
		public event Action OnChange;

		public void SetActiveTab(ActiveTab tab)
		{
			ActiveTab = tab;
			_storage.Set(nameof(ActiveTab), tab);
			NotifyStateChanged();
		}
		public void SetMenuOpen(bool isMenuOpen)
		{
			IsMenuOpen = isMenuOpen;
			NotifyStateChanged();
		}
		public void SetPreviousTab(ActiveTab tab)
		{
			PreviousTab = tab;
		}
		public string GetPreviousPath()
		{
			return PreviousTab switch
			{
				ActiveTab.Home => "/",
				ActiveTab.Counter => "/counter",
				ActiveTab.Weather => "/weather",
				_ => "/",
			};
		}
		public string GetTabPath(ActiveTab tab)
		{
			return tab switch
			{
				ActiveTab.Home => "/",
				ActiveTab.Counter => "/counter",
				ActiveTab.Weather => "/weather",
				_ => "/",
			};
		}
		public string GetActivePath()
		{
			return ActiveTab switch
			{
				ActiveTab.Home => "/",
				ActiveTab.Counter => "/counter",
				ActiveTab.Weather => "/weather",
				_ => "/",
			};
		}
		private void NotifyStateChanged() => OnChange.Invoke();
	}

	public enum ActiveTab
	{
		Home = 1,
		Counter = 2,
		Weather = 3
	}
}
