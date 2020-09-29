using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Services.SessionState
{
    public interface IUserSessionState
    {
        string FullName { get; }

        event Action OnChange;

        void SetFullName(string FullName);
    }

    public class UserSessionState : IUserSessionState
    {
        public string FullName { get; protected set; }

        public event Action OnChange;

        public void SetFullName(string FullName)
        {
            this.FullName = FullName;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
