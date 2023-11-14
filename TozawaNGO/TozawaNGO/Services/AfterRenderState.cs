using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TozawaNGO.Services
{
    public class AfterRenderState
    {
        public bool IsFirstLoaded { get; private set; }

        public event Action OnChange;

        public void SetRequestFirstLoaded(bool isFirstLoaded)
        {
            IsFirstLoaded = isFirstLoaded;
            NotifyStateChanged();
        }
        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
