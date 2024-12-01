
using Blazored.SessionStorage;

namespace ShareRazorClassLibrary.Services
{
	public class NavMenuTabState(ISessionStorageService sessionStorageService)
	{
		private readonly ISessionStorageService _sessionStorageService = sessionStorageService;

		public ActiveTab ActiveTab { get; private set; } = ActiveTab.Home;
		public ActiveTab PreviousTab { get; private set; } = ActiveTab.Home;
		public bool IsMenuOpen { get; private set; }
		public event Action OnChange;

		public void SetActiveTab(ActiveTab tab)
		{
			ActiveTab = tab;
			_sessionStorageService.SetItemAsync(nameof(ActiveTab), tab);
			NotifyStateChanged();
		}
		public void SetPreviousTab(ActiveTab tab)
		{
			PreviousTab = tab;
		}
		public void SetMenuOpen(bool isMenuOpen)
		{
			IsMenuOpen = isMenuOpen;
			NotifyStateChanged();
		}
		public string GetTabPath(ActiveTab tab)
		{
			return tab switch
			{
				ActiveTab.Home => "/",
				ActiveTab.Counter => "/fetchdata1",
				ActiveTab.Settings => "/settings",
				ActiveTab.Weather => "/fetchdata",
				_ => "/",
			};
		}
		public string GetActivePath()
		{
			return ActiveTab switch
			{
				ActiveTab.Home => "/",
				ActiveTab.Counter => "/fetchdata1",
				ActiveTab.Settings => "/settings",
				ActiveTab.Weather => "/fetchdata",
				_ => "/",
			};
		}
		public string GetPreviousPath()
		{
			return PreviousTab switch
			{
				ActiveTab.Home => "/",
				ActiveTab.Counter => "/fetchdata1",
				ActiveTab.Settings => "/settings",
				ActiveTab.Weather => "/fetchdata",
				_ => "/",
			};
		}
		private void NotifyStateChanged() => OnChange?.Invoke();
	}

	public enum ActiveTab
	{
		Home = 1,
		Counter = 2,
		Weather = 3,
		Settings = 4
	}
}
