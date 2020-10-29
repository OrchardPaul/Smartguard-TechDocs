using Gizmo.Context.Gizmo_Authentification;
using Gizmo_V1_02.Data.Admin;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Services.SessionState
{
    public interface IUserSessionState
    {
        IList<Claim> allClaims { get; }
        string baseUri { get; }
        AppCompanyDetails Company { get; }
        string FullName { get; }
        SpinLock IdentityLock { get; set; }
        bool Lock { get; set; }
        string selectedSystem { get; }

        event Action OnChange;

        Claim getCompanyClaim();
        bool GetLock();
        void SetBaseUri(string baseUri);
        void SetClaims(IList<Claim> claims);
        void SetCompany(AppCompanyDetails companyDetails);
        void SetFullName(string FullName);
        void SetSelectedSystem(string selectedSystem);
        Task<string> SetSessionState();
    }

    public class UserSessionState : IUserSessionState
    {
        private readonly AuthenticationStateProvider authenticationStateProvider;
        private readonly IIdentityUserAccess userAccess;
        private readonly ICompanyDbAccess companyDbAccess;

        public IList<Claim> allClaims { get; protected set; }

        public string FullName { get; protected set; }

        public AppCompanyDetails Company { get; protected set; }

        public string baseUri { get; protected set; }

        public string selectedSystem { get; protected set; }

        public SpinLock IdentityLock { get; set; }

        public bool Lock { get; set; }

        public event Action OnChange;

        private string sessionStateSet;

        public UserSessionState(AuthenticationStateProvider authenticationStateProvider
                                , IIdentityUserAccess userAccess
                                , ICompanyDbAccess companyDbAccess)
        {
            this.authenticationStateProvider = authenticationStateProvider;
            this.userAccess = userAccess;
            this.companyDbAccess = companyDbAccess;
        }

        public bool GetLock()
        {
            return Lock;
        }

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

        public void SetSelectedSystem(string selectedSystem)
        {
            this.selectedSystem = selectedSystem;
        }

        public void SetClaims(IList<Claim> claims)
        {
            allClaims = claims;
            NotifyStateChanged();
        }

        public void SetCompany(AppCompanyDetails companyDetails)
        {
            Company = companyDetails;
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

        public async Task<string> SetSessionState()
        {
            Lock = true;

            try
            {
                var auth = await authenticationStateProvider.GetAuthenticationStateAsync();

                sessionStateSet = "Not Set";
                SetBaseUri("Not Set");

                if (!(auth is null))
                {
                    var user = auth.User;
                    var userName = user.Identity.Name;

                    if (!(userName is null))
                    {
                        var currentUser = await userAccess.GetUserByName(userName);

                        if (!(currentUser is null))
                        {
                            SetFullName(currentUser.FullName);

                            var allClaims = await userAccess.GetSignedInUserClaims();

                            if (!(allClaims.Count() == 0))
                            {
                                SetClaims(allClaims);

                                var companyClaim = allClaims.Where(A => A.Type == "Company").SingleOrDefault();
                                var baseUri = await companyDbAccess.GetCompanyBaseUri(Int32.Parse(companyClaim.Value)
                                                                                        , (currentUser.SelectedUri is null) ? "" : currentUser.SelectedUri);


                                if (!(companyClaim is null))
                                {
                                    var companyDetails = await companyDbAccess.GetCompanyById(Int32.Parse(companyClaim.Value));

                                    SetCompany(companyDetails);
                                }


                                if (!(baseUri is null))
                                {
                                    SetBaseUri(baseUri);
                                    SetSelectedSystem(currentUser.SelectedUri);
                                    sessionStateSet = "Success";
                                }
                            }
                        }
                    }
                }
                
            }
            finally
            {
                Lock = false;
            }

            NotifyStateChanged();
            return sessionStateSet;

        }



        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
