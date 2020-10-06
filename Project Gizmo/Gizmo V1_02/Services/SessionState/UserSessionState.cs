using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Gizmo_V1_02.Services.SessionState
{
    public interface IUserSessionState
    {
        IList<Claim> allClaims { get; }
        string baseUri { get; }
        string FullName { get; }

        event Action OnChange;

        Claim getCompanyClaim();
        void SetBaseUri(string baseUri);
        void SetClaims(IList<Claim> claims);
        void SetFullName(string FullName);
    }

    public class UserSessionState : IUserSessionState
    {
        public IList<Claim> allClaims { get; protected set; }

        public string FullName { get; protected set; }

        public string baseUri { get; protected set; }

        public event Action OnChange;

        public void SetFullName(string FullName)
        {
            this.FullName = FullName;
            NotifyStateChanged();
        }

        public void SetBaseUri(string baseUri)
        {
            this.baseUri = baseUri;
            NotifyStateChanged();
        }

        public void SetClaims(IList<Claim> claims)
        {
            allClaims = claims;
            NotifyStateChanged();
        }

        public Claim getCompanyClaim()
        {
            if (allClaims is null)
            {
                return null;
            }
            else
            {
                var companyClaim = allClaims.Where(A => A.Type == "Company").SingleOrDefault();

                return (companyClaim is null) ? null : companyClaim;
            }
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
