using GadjIT.GadjitContext.GadjIT_App;
using Gizmo_V1_02.Data.Admin;
using Microsoft.AspNetCore.Components;
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
        AspNetUsers User { get; }
        IList<Claim> allClaims { get; }
        string userProfileReturnURI { get; }
        string CompCol1 { get; set; }
        string CompCol2 { get; set; }
        string CompCol3 { get; set; }
        string CompCol4 { get; set; }
        string baseUri { get; }
        AppCompanyDetails Company { get; }
        List<AppCompanyDetails> allAssignedCompanies { get; }
        bool isSuperUser { get;  }
        bool isAdminUser { get;  }
        string FullName { get; }
        SpinLock IdentityLock { get; set; }
        bool Lock { get; set; }
        string selectedSystem { get; }
        string TempBackGroundImage { get; set; }
        event Action OnChange;

        Claim getCompanyClaim();
        bool GetLock();
        void SetCurrentUser(AspNetUsers user);
        void SetUserProfileReturnURI(string returnURI);
        void SetBaseUri(string baseUri);
        void SetClaims(IList<Claim> claims);
        void SetCompany(AppCompanyDetails companyDetails);
        void SetAllAssignedCompanies(List<AppCompanyDetails> allAssignedCompanies);
        void SetFullName(string FullName);
        void SetSelectedSystem(string selectedSystem);
        Task<string> SwitchSelectedSystem();
        Task<string> ResetSelectedSystem();
        Task<AppCompanyDetails> switchSelectedCompany();
        Task<string> SetSessionState();
        Action RefreshHome { get; set; }

        Action HomeActionSmartflow { get; set; }

        void SetTempBackground(string image, string Uri);

    }

    public class UserSessionState : IUserSessionState
    {
        private readonly AuthenticationStateProvider authenticationStateProvider;
        private readonly IIdentityUserAccess userAccess;
        private readonly ICompanyDbAccess companyDbAccess;
        private readonly NavigationManager navigationManager;

        public AspNetUsers User { get; protected set; }

        public IList<Claim> allClaims { get; protected set; }

        public string FullName { get; protected set; }

        public string CompCol1 { get; set; }

        public string CompCol2 { get; set; }

        public string CompCol3 { get; set; }

        public string CompCol4 { get; set; }

        public Action RefreshHome { get; set; }

        public AppCompanyDetails Company { get; protected set; }

        public List<AppCompanyDetails> allAssignedCompanies { get; protected set; }

        public string baseUri { get; protected set; }

        public string selectedSystem { get; protected set; }

        public string userProfileReturnURI { get; protected set; }

        public SpinLock IdentityLock { get; set; }

        public bool isSuperUser { get; protected set; } = false;

        public bool isAdminUser { get; protected set; } = false;

        public bool Lock { get; set; }

        public event Action OnChange;

        private string sessionStateSet;

        public string TempBackGroundImage { get; set; }

        public string TempBackGroundImageAppliedUri { get; protected set; }

        public string SelectedChapter { get; protected set; }


        public Action HomeActionSmartflow { get; set; }

        public UserSessionState(AuthenticationStateProvider authenticationStateProvider
                                , IIdentityUserAccess userAccess
                                , ICompanyDbAccess companyDbAccess
                                , NavigationManager navigationManager)
        {
            this.authenticationStateProvider = authenticationStateProvider;
            this.userAccess = userAccess;
            this.companyDbAccess = companyDbAccess;
            this.navigationManager = navigationManager;
        }

        

        public void SetTempBackground(string image, string Uri)
        {
            TempBackGroundImage = image;
            TempBackGroundImageAppliedUri = Uri;
        }


        public void SetUserProfileReturnURI(string returnURI)
        {
            userProfileReturnURI = returnURI;
            NotifyStateChanged();
        }

        public void SetCurrentUser(AspNetUsers user)
        {
            User = user;
            NotifyStateChanged();
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
            NotifyStateChanged();
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

        public void SetAllAssignedCompanies(List<AppCompanyDetails> allAssignedCompanies)
        {
            this.allAssignedCompanies = allAssignedCompanies;
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

        public async Task<AppCompanyDetails> switchSelectedCompany()
        {
            //Get new 
            var selectedCompany = await companyDbAccess.GetCompanyById(User.SelectedCompanyId);
            SetCompany(selectedCompany);

            User = await userAccess.SwitchSelectedCompany(User);

            await SetSessionState();
            return selectedCompany;
        }

        public async Task<string> SwitchSelectedSystem()
        {
            Lock = true;
            //Get new 
            var baseUri = await companyDbAccess.GetCompanyBaseUri(Company.Id, (User.SelectedUri == "Live") ? "Dev" : "Live");
            this.baseUri = baseUri;
            selectedSystem = (User.SelectedUri == "Live") ? "Dev" : "Live";

            Lock = false;
            return baseUri;
        }

        public async Task<string> ResetSelectedSystem()
        {
            Lock = true;
            //Get new 
            var baseUri = await companyDbAccess.GetCompanyBaseUri(Company.Id, User.SelectedUri);
            this.baseUri = baseUri;
            selectedSystem = User.SelectedUri;

            Lock = false;
            return baseUri;
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
                            SetCurrentUser(currentUser);
                            SetFullName(currentUser.FullName);
                            SetSelectedSystem(currentUser.SelectedUri);

                            isSuperUser = auth.User.IsInRole("Super User");

                            List<string> adminRoles = new List<string>()
                            {
                                "Super User"
                                ,"Site Admin"
                            };

                            foreach (var role in adminRoles)
                            {
                                if (auth.User.IsInRole(role))
                                {
                                    isAdminUser = true;
                                    break;
                                }
                            }
                            
                            var allClaims = await userAccess.GetSignedInUserClaims();

                            if (!(allClaims.Count() == 0))
                            {
                                SetClaims(allClaims);

                                var companies = await companyDbAccess.GetCompanies();
                                SetAllAssignedCompanies(companies);
                            }

                            var selectedCompany = await companyDbAccess.GetSelectedCompanyOfUser(currentUser);

                            if (!(selectedCompany is null))
                            {
                                SetCompany(selectedCompany);
                                
                                CompCol1 = selectedCompany.CompCol1;
                                CompCol2 = selectedCompany.CompCol2;
                                CompCol3 = selectedCompany.CompCol3;
                                CompCol4 = selectedCompany.CompCol4;

                                var baseUri = await companyDbAccess.GetCompanyBaseUri(selectedCompany.Id
                                                                                    , (currentUser.SelectedUri is null) ? "" : currentUser.SelectedUri);
                                
                                if (!(baseUri is null))
                                {
                                    
                                    SetBaseUri(baseUri);
                                    
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
