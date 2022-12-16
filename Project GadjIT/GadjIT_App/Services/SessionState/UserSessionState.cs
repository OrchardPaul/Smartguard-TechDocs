using GadjIT_AppContext.GadjIT_App;
using GadjIT_App.Data.Admin;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace GadjIT_App.Services.SessionState
{
    public interface IUserSessionState
    {
        AspNetUsers User { get; }
        IList<Claim> AllClaims { get; }
        string UserProfileReturnURI { get; }
        string CompCol1 { get; set; }
        string CompCol2 { get; set; }
        string CompCol3 { get; set; }
        string CompCol4 { get; set; }
        string BaseUri { get; }
        AppCompanyDetails Company { get; }
        List<AppCompanyDetails> AllAssignedCompanies { get; }
        bool IsSuperUser { get; }
        bool IsAdminUser { get; }
        string FullName { get; }
        SpinLock IdentityLock { get; set; }
        bool Lock { get; set; }
        string SelectedSystem { get; }
        string TempBackGroundImage { get; set; }

        DateTime ChapterLastCompared { get; set; }     //keeps a record of the last time a Chapter of list of Chapters was compared for updates by another user
                                                       //each time a user navigates within Smartflow this is set to the ChapterLastUpdated value from the local store
                                                       //if the values are not the same the StateHasChanged method is invoked.

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

        bool SuppressChangeSystemError { get; set; }
    }
        

    public class UserSessionState : IUserSessionState
    {
        private readonly AuthenticationStateProvider AuthenticationStateProvider;
        private readonly IIdentityUserAccess UserAccess;
        private readonly ICompanyDbAccess CompanyDbAccess;
        private readonly NavigationManager NavigationManager;
        private readonly ILogger<UserSessionState> Logger;

        public AspNetUsers User { get; protected set; }

        public IList<Claim> AllClaims { get; protected set; }

        public string FullName { get; protected set; }

        public string CompCol1 { get; set; }

        public string CompCol2 { get; set; }

        public string CompCol3 { get; set; }

        public string CompCol4 { get; set; }

        public Action RefreshHome { get; set; }

        public AppCompanyDetails Company { get; protected set; }

        public List<AppCompanyDetails> AllAssignedCompanies { get; protected set; }

        public string BaseUri { get; protected set; }

        public string SelectedSystem { get; protected set; }

        public string UserProfileReturnURI { get; protected set; }

        public SpinLock IdentityLock { get; set; }

        public bool SuppressChangeSystemError { get; set; } = false;

        public bool IsSuperUser { get; protected set; } = false;

        public bool IsAdminUser { get; protected set; } = false;

        public bool Lock { get; set; }

        public event Action OnChange;

        private string SessionStateSet;

        public string TempBackGroundImage { get; set; }

        public string TempBackGroundImageAppliedUri { get; protected set; }

        public string SelectedChapter { get; protected set; }

        public DateTime ChapterLastCompared { get; set; }     
                                                                        
                                                                        

        public Action HomeActionSmartflow { get; set; }

        public UserSessionState(AuthenticationStateProvider _authenticationStateProvider
                                , IIdentityUserAccess _userAccess
                                , ICompanyDbAccess _companyDbAccess
                                , NavigationManager _navigationManager
                                , ILogger<UserSessionState> _logger)
        {
            AuthenticationStateProvider = _authenticationStateProvider;
            UserAccess = _userAccess;
            CompanyDbAccess = _companyDbAccess;
            NavigationManager = _navigationManager;
            Logger = _logger;
        }


        public void SetChapterLastCompared(DateTime chapterLastCompared)
        {
            ChapterLastCompared = chapterLastCompared;
        }

        public void SetTempBackground(string image, string Uri)
        {
            TempBackGroundImage = image;
            TempBackGroundImageAppliedUri = Uri;
        }


        public void SetUserProfileReturnURI(string returnURI)
        {
            UserProfileReturnURI = returnURI;
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
            BaseUri = baseUri;
            NotifyStateChanged();
        }

        public void SetSelectedSystem(string selectedSystem)
        {
            SelectedSystem = selectedSystem;
            NotifyStateChanged();
        }

        public void SetClaims(IList<Claim> claims)
        {
            AllClaims = claims;
            NotifyStateChanged();
        }

        public void SetCompany(AppCompanyDetails companyDetails)
        {
            Company = companyDetails;
            NotifyStateChanged();
        }

        public void SetAllAssignedCompanies(List<AppCompanyDetails> allAssignedCompanies)
        {
            AllAssignedCompanies = allAssignedCompanies;
            NotifyStateChanged();
        }

        public Claim getCompanyClaim()
        {
            if (AllClaims is null)
            {
                return null;
            }
            else
            {
                var companyClaim = AllClaims.Where(A => A.Type == "Company").SingleOrDefault();

                return (companyClaim is null) ? null : companyClaim;
            }
        }

        public async Task<AppCompanyDetails> switchSelectedCompany()
        {
            //Get new 
            var selectedCompany = await CompanyDbAccess.GetCompanyById(User.SelectedCompanyId);
            SetCompany(selectedCompany);

            User = await UserAccess.SwitchSelectedCompany(User);

            await SetSessionState();
            return selectedCompany;
        }

        public async Task<string> SwitchSelectedSystem()
        {
            Lock = true;
            //Get new 
            var baseUri = await CompanyDbAccess.GetCompanyBaseUri(Company.Id, (User.SelectedUri == "Live") ? "Dev" : "Live");
            this.BaseUri = baseUri;
            SelectedSystem = (User.SelectedUri == "Live") ? "Dev" : "Live";

            Lock = false;
            return baseUri;
        }

        public async Task<string> ResetSelectedSystem()
        {
            Lock = true;
            //Get new 
            var baseUri = await CompanyDbAccess.GetCompanyBaseUri(Company.Id, User.SelectedUri);
            this.BaseUri = baseUri;
            SelectedSystem = User.SelectedUri;

            Lock = false;
            return baseUri;
        }


        public async Task<string> SetSessionState()
        {
            Lock = true;

            try
            {
                var auth = await AuthenticationStateProvider.GetAuthenticationStateAsync();

                SessionStateSet = "Not Set";
                SetBaseUri("Not Set");

                if (!(auth is null))
                {
                    var user = auth.User;
                    var userName = user.Identity.Name;

                    if (!(userName is null))
                    {
                        var currentUser = await UserAccess.GetUserByName(userName);
                        
                        if (!(currentUser is null))
                        {
                            SetCurrentUser(currentUser);
                            SetFullName(currentUser.FullName);
                            SetSelectedSystem(currentUser.SelectedUri);

                            IsSuperUser = auth.User.IsInRole("Super User");

                            List<string> adminRoles = new List<string>()
                            {
                                "Super User"
                                ,"Site Admin"
                            };

                            foreach (var role in adminRoles)
                            {
                                if (auth.User.IsInRole(role))
                                {
                                    IsAdminUser = true;
                                    break;
                                }
                            }
                            
                            var allClaims = await UserAccess.GetSignedInUserClaims();

                            if (!(allClaims.Count() == 0))
                            {
                                SetClaims(allClaims);

                                var companies = await CompanyDbAccess.GetCompanies();
                                SetAllAssignedCompanies(companies);
                            }

                            var selectedCompany = await CompanyDbAccess.GetSelectedCompanyOfUser(currentUser);

                            if (!(selectedCompany is null))
                            {
                                SetCompany(selectedCompany);
                                
                                CompCol1 = selectedCompany.CompCol1;
                                CompCol2 = selectedCompany.CompCol2;
                                CompCol3 = selectedCompany.CompCol3;
                                CompCol4 = selectedCompany.CompCol4;

                                var baseUri = await CompanyDbAccess.GetCompanyBaseUri(selectedCompany.Id
                                                                                    , (currentUser.SelectedUri is null) ? "" : currentUser.SelectedUri);
                                
                                if (!(baseUri is null))
                                {
                                    
                                    SetBaseUri(baseUri);
                                    
                                    SessionStateSet = "Success";
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
            return SessionStateSet;

        }



        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
