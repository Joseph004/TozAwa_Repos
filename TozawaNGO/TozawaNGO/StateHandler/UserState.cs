using System;
using TozawaNGO.Models.Dtos;

namespace TozawaNGO.StateHandler
{
    public class UserState
    {
        public UserDto User { get; private set; }

        public event Action OnChange;

        public void SetUserAthentication(UserDto user)
        {
            User = user;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}