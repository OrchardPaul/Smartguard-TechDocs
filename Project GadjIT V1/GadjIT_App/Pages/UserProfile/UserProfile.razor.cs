using GadjIT_AppContext.GadjIT_App;
using GadjIT_AppContext.GadjIT_App.Custom;
using GadjIT_App.Data.Admin;
using GadjIT_App.Services.SessionState;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GadjIT_App.FileManagement.FileClassObjects;
using GadjIT_App.FileManagement.FileProcessing.Interface;


namespace GadjIT_App.Pages.UserProfile
{
    public partial class UserProfile
    {
        [Parameter]
        public Action ToggleDetail { get; set; }

        [Parameter]
        public AspNetUsers TaskObject { get; set; }

        [Parameter]
        public List<CompanyItem> CompanyItems { get; set; }

        [Parameter]
        public List<RoleItem> SelectedRoles { get; set; }

        [Parameter]
        public string SelectedOption { get; set; }

        [Parameter]
        public bool EnablePasswordSet { get; set; } = false;

        [Inject]
        private IIdentityUserAccess UserAccess { get; set; }

        [Inject]
        private IIdentityRoleAccess RoleAccess { get; set; }

        [Inject]
        private IUserSessionState SessionState { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [Inject]
        private ICompanyDbAccess CompanyDbAccess { get; set; }

        private IList<string> EditObjectRoles { get; set; }

        private List<AppCompanyDetails> LstCompanies { get; set; }

        private List<AspNetRoles> LstRoles { get; set; }

        private List<string> UsersClaimId { get; set; }
        
        [Inject]
        public IFileHelper FileHelper { get; set; }

        private List<FileDesc> ListFileDescriptions { get; set; }

        

        protected override async Task OnInitializedAsync()
        {

            //Wait for session state to finish to prevent concurrency error on refresh
            bool gotLock = SessionState.Lock;
            while (gotLock)
            {
                await Task.Yield();
                gotLock = SessionState.Lock;
            }
            SessionState.OnChange += StateHasChanged;

            TaskObject = SessionState.User;
            TaskObject.PasswordHash = "PasswordNotChanged115592!";


            LstCompanies = await CompanyDbAccess.GetCompanies();
            LstRoles = await RoleAccess.GetUserRoles();

            EditObjectRoles = await RoleAccess.GetCurrentUserRolesForCompany(TaskObject, TaskObject.SelectedCompanyId);

            //EditObjectRoles = await UserAccess.GetSelectedUserRoles(selectedUser);
            var userCliams = await UserAccess.GetCompanyClaims(TaskObject);
            UsersClaimId = userCliams.Select(U => U.Value).ToList();

            CompanyItems = LstCompanies.Select(C => new CompanyItem
            {
                Id = C.Id,
                Company = C,
                IsSubscribed = (UsersClaimId.Contains(C.Id.ToString())) ? true : false
            }).ToList();

            SelectedRoles = LstRoles
                .Select(L => new RoleItem
                {
                    IsSubscribed = (EditObjectRoles.Contains(L.Name)) ? true : L.Name == "Super User" && SessionState.IsSuperUser ? true : false,
                    RoleName = L.Name,
                    RoleId = L.Id
                })
                .ToList();


            GetBackgroundFileList();

        }

        public void Dispose()
        {
            SessionState.OnChange -= StateHasChanged;
        }


        private void TogglePasswordSet()
        {
            EnablePasswordSet = !EnablePasswordSet;

            if (EnablePasswordSet)
            {
                TaskObject.PasswordHash = "************";
            }
            else
            {
                TaskObject.PasswordHash = "PasswordNotChanged115592!";
            }

            StateHasChanged();
        }

        private async Task ToggleCompany(int selectedId)
        {
            TaskObject.SelectedCompanyId = selectedId;

            EditObjectRoles = await RoleAccess.GetCurrentUserRolesForCompany(TaskObject, TaskObject.SelectedCompanyId);

            SelectedRoles = LstRoles
                .Select(L => new RoleItem
                {
                    IsSubscribed = (EditObjectRoles.Contains(L.Name)) ? true : false,
                    RoleName = L.Name,
                    RoleId = L.Id
                })
                .ToList();

            StateHasChanged();
        }


        private async Task HandleValidSubmit()
        {
            await SubmitChange();
            NavigateBack();
        }

        public void NavigateBack()
        {
            NavigationManager.NavigateTo(SessionState.UserProfileReturnURI, true);
        }

        private async Task<AspNetUsers> SubmitChange()
        {

            var returnObject = await UserAccess.SubmitChanges(TaskObject, SelectedRoles);
            await UserAccess.SubmitCompanyCliams(CompanyItems, returnObject);

            await SessionState.SetSessionState();

            return returnObject;
        }

        private void GetBackgroundFileList()
        {
            FileHelper.CustomPath = $"wwwroot/images/BackgroundImages";

            ListFileDescriptions = FileHelper.GetFileList();


        }

    }
}
