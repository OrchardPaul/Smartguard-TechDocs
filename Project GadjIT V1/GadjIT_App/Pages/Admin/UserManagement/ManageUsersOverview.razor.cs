using Blazored.Modal;
using Blazored.Modal.Services;
using GadjIT_AppContext.GadjIT_App;
using GadjIT_AppContext.GadjIT_App.Custom;
using GadjIT_App.Data.Admin;
using GadjIT_App.Pages._Shared.Modals;
using GadjIT_App.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GadjIT_App.Pages.Admin.UserManagement
{
    public partial class ManageUsersOverview
    {
        [Inject]
        IModalService Modal { get; set; }

        [Parameter]
        public Action ToggleDetail { get; set; }

        [Inject]
        private IUserManagementSelectedUserState SelectedUserState { get; set; }

        [Inject]
        private IIdentityUserAccess UserAccess { get; set; }

        [Inject]
        private IIdentityRoleAccess RoleAccess { get; set; }

        [Inject]
        private ICompanyDbAccess CompanyDbAccess { get; set; }

        [Inject]
        private IUserSessionState SessionState { get; set; }

        public AspNetUser EditObject { get; set; } = new AspNetUser();

        protected IList<string> EditObjectRoles { get; set; }

        public string EditOption { get; set; }

        public List<RoleItem> Roles { get; set; } = new List<RoleItem>();

        public List<AspNetRoles> LstRoles { get; set; }

        protected List<UserDataCollectionItem> LstUserDataItems { get; set; }

        public List<AppCompanyDetails> Companies { get; set; }

        public string SelectedRole { get; set; } = "None";

        public string FilterName { get; set; } = "";

        protected override async Task OnInitializedAsync()
        {

            //Wait for session state to finish to prevent concurrency error on refresh
            bool gotLock = SessionState.Lock;
            while (gotLock)
            {
                await Task.Yield();
                gotLock = SessionState.Lock;
            }

            LstUserDataItems = await UserAccess.GetUsersWithCompanyInfo();
            LstRoles = await RoleAccess.GetUserRoles();
            Companies = await CompanyDbAccess.GetCompanies();

            SelectedUserState.DataChanged = DataChanged;
            SelectedUserState.allCompanies = Companies;
            SelectedUserState.allRoles = LstRoles;
        }


        protected void PrepareForEdit(AspNetUser selectedUser)
        {
            EditOption = "Edit";
            EditObject = selectedUser;
            EditObject.SelectedUri = (SessionState.SelectedSystem is null) ? "Live" : SessionState.SelectedSystem;
            EditObject.PasswordHash = "PasswordNotChanged115592!";

            SelectedUserState.TaskObject = EditObject;
            SelectedUserState.selectedOption = EditOption;

            ToggleDetail?.Invoke();
        }

        protected void PrepareForInsert()
        {
            EditOption = "Insert";
            EditObject = new AspNetUser();
            EditObject.SelectedUri = "Live";
            EditObject.SelectedCompanyId = SessionState.User.SelectedCompanyId;

            SelectedUserState.TaskObject = EditObject;
            SelectedUserState.selectedOption = EditOption;

            ToggleDetail?.Invoke();
        }

        private async void DataChanged()
        {
            LstUserDataItems = await UserAccess.GetUsersWithCompanyInfo();
            LstRoles = await RoleAccess.GetUserRoles();

            StateHasChanged();
        }

        private void changeNameFilter(ChangeEventArgs eventArgs)
        {
            FilterName = eventArgs.Value.ToString();
        }

        private void ToggleMoreOption(UserDataCollectionItem hoveredItem)
        {
            hoveredItem.OnHover = !hoveredItem.OnHover;
        }

        protected void PrepareModalForDelete(AspNetUser selectedUser)
        {
            EditObject = selectedUser;

            Action SelectedDeleteAction = HandleValidDelete;
            var parameters = new ModalParameters();
            parameters.Add("InfoHeader", "Delete?");
            parameters.Add("ModalHeight", "300px");
            parameters.Add("ModalWidth", "500px");
            parameters.Add("DeleteAction", SelectedDeleteAction);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal"
            };

            Modal.Show<ModalDelete>("Delete?", parameters, options);
        }

        private async void HandleValidDelete()
        {
            await UserAccess.Delete(EditObject);

            DataChanged();
        }

    }
}
