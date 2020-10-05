using Gizmo.Context.Gizmo_Authentification;
using Gizmo_V1_02.Data;
using Gizmo_V1_02.Data.Admin;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Services.SessionState
{
    public interface IUserSessionState
    {
        IList<Claim> allClaims { get; }
        string FullName { get; }

        event Action OnChange;

        void SetClaims(IList<Claim> claims);
        void SetFullName(string FullName);
    }

    public class UserSessionState : IUserSessionState
    {
        public IList<Claim> allClaims { get; protected set; }

        public string FullName { get; protected set; }

        public event Action OnChange;

        public void SetFullName(string FullName)
        {
            this.FullName = FullName;
            NotifyStateChanged();
        }

        public void SetClaims(IList<Claim> claims)
        {
            allClaims = claims;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
