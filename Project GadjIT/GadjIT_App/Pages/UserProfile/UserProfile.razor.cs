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
        public List<CompanyItem> companyItems { get; set; }

        [Parameter]
        public List<RoleItem> selectedRoles { get; set; }

        [Parameter]
        public string selectedOption { get; set; }

        [Parameter]
        public bool enablePasswordSet { get; set; } = false;

        [Inject]
        private IIdentityUserAccess service { get; set; }

        [Inject]
        private IIdentityRoleAccess roleAccess { get; set; }

        [Inject]
        private IUserSessionState sessionState { get; set; }

        [Inject]
        private NavigationManager navigationManager { get; set; }

        [Inject]
        private ICompanyDbAccess companyDbAccess { get; set; }

        private IList<string> editObjectRoles { get; set; }

        private List<AppCompanyDetails> companies { get; set; }

        private List<AspNetRoles> lstRoles { get; set; }

        private List<string> usersClaimId { get; set; }
        
        [Inject]
        public IFileHelper FileHelper { get; set; }

        private List<FileDesc> ListFileDescriptions { get; set; }

        

        protected override async Task OnInitializedAsync()
        {

            //Wait for session state to finish to prevent concurrency error on refresh
            bool gotLock = sessionState.Lock;
            while (gotLock)
            {
                await Task.Yield();
                gotLock = sessionState.Lock;
            }
            sessionState.OnChange += StateHasChanged;

            TaskObject = sessionState.User;
            TaskObject.PasswordHash = "PasswordNotChanged115592!";


            companies = await companyDbAccess.GetCompanies();
            lstRoles = await roleAccess.GetUserRoles();

            editObjectRoles = await roleAccess.GetCurrentUserRolesForCompany(TaskObject, TaskObject.SelectedCompanyId);

            //editObjectRoles = await service.GetSelectedUserRoles(selectedUser);
            var userCliams = await service.GetCompanyClaims(TaskObject);
            usersClaimId = userCliams.Select(U => U.Value).ToList();

            companyItems = companies.Select(C => new CompanyItem
            {
                Id = C.Id,
                Company = C,
                IsSubscribed = (usersClaimId.Contains(C.Id.ToString())) ? true : false
            }).ToList();

            selectedRoles = lstRoles
                .Select(L => new RoleItem
                {
                    IsSubscribed = (editObjectRoles.Contains(L.Name)) ? true : L.Name == "Super User" && sessionState.isSuperUser ? true : false,
                    RoleName = L.Name,
                    RoleId = L.Id
                })
                .ToList();


            GetBackgroundFileList();

        }

        public void Dispose()
        {
            sessionState.OnChange -= StateHasChanged;
        }


        private void TogglePasswordSet()
        {
            enablePasswordSet = !enablePasswordSet;

            if (enablePasswordSet)
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

            editObjectRoles = await roleAccess.GetCurrentUserRolesForCompany(TaskObject, TaskObject.SelectedCompanyId);

            selectedRoles = lstRoles
                .Select(L => new RoleItem
                {
                    IsSubscribed = (editObjectRoles.Contains(L.Name)) ? true : false,
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
            navigationManager.NavigateTo(sessionState.userProfileReturnURI, true);
        }

        private async Task<AspNetUsers> SubmitChange()
        {

            var returnObject = await service.SubmitChanges(TaskObject, selectedRoles);
            await service.SubmitCompanyCliams(companyItems, returnObject);

            await sessionState.SetSessionState();

            return returnObject;
        }

        private void GetBackgroundFileList()
        {
            FileHelper.CustomPath = $"wwwroot/images/BackgroundImages";

            ListFileDescriptions = FileHelper.GetFileList();


        }

    }
}
