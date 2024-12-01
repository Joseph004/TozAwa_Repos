
using Microsoft.AspNetCore.Components;

namespace ShareRazorClassLibrary.Services
{
	public class FirstloadState
	{
		public bool IsFirstLoaded { get; private set; } = false;
		public event Action OnChange;

		public void SetFirsLoad(bool isFirstLoaded)
		{
			IsFirstLoaded = isFirstLoaded;
			NotifyStateChanged();
		}
		private void NotifyStateChanged() => OnChange?.Invoke();
	}
}
