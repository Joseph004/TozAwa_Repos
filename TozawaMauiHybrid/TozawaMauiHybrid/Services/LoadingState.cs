using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TozawaMauiHybrid.Services
{
	public class LoadingState
	{
		public bool RequestInProgress { get; private set; }

		public event Action OnChange;

		public void SetRequestInProgress(bool requestInProgress)
		{
			RequestInProgress = requestInProgress;
			NotifyStateChanged();
		}
		private void NotifyStateChanged() => OnChange.Invoke();
	}
}
