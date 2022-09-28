using Blazored.Modal;
using Blazored.Modal.Services;
using GadjIT_AppContext.GadjIT_App;
using GadjIT_AppContext.GadjIT_App.Custom;
using GadjIT_App.Data.Admin;
using GadjIT_App.Pages.Shared.Modals;
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
        private IUserManagementSelectedUserState selectedUserState { get; set; }

        [Inject]
        private IIdentityUserAccess userAccess { get; set; }

        [Inject]
        private IIdentityRoleAccess roleAccess { get; set; }

        [Inject]
        private ICompanyDbAccess companyDbAccess { get; set; }

        [Inject]
        private IUserSessionState sessionState { get; set; }

        public AspNetUsers editObject { get; set; } = new AspNetUsers();

        protected IList<string> editObjectRoles { get; set; }

        public string editOption { get; set; }

        public List<RoleItem> roles { get; set; } = new List<RoleItem>();

        public List<AspNetRoles> lstRoles { get; set; }

        protected List<UserDataCollectionItem> lstUserDataItems { get; set; }

        public List<AppCompanyDetails> companies { get; set; }

        public string selectedRole { get; set; } = "None";

        public string filterName { get; set; } = "";

        protected override async Task OnInitializedAsync()
        {

            //Wait for session state to finish to prevent concurrency error on refresh
            bool gotLock = sessionState.Lock;
            while (gotLock)
            {
                await Task.Yield();
                gotLock = sessionState.Lock;
            }

            lstUserDataItems = await userAccess.GetUsersWithCompanyInfo();
            lstRoles = await roleAccess.GetUserRoles();
            companies = await companyDbAccess.GetCompanies();

            selectedUserState.DataChanged = DataChanged;
            selectedUserState.allCompanies = companies;
            selectedUserState.allRoles = lstRoles;
        }


        protected void PrepareForEdit(AspNetUsers selectedUser)
        {
            editOption = "Edit";
            editObject = selectedUser;
            editObject.SelectedUri = (sessionState.selectedSystem is null) ? "Live" : sessionState.selectedSystem;
            editObject.PasswordHash = "PasswordNotChanged115592!";

            selectedUserState.TaskObject = editObject;
            selectedUserState.selectedOption = editOption;

            ToggleDetail?.Invoke();
        }

        protected void PrepareForInsert()
        {
            editOption = "Insert";
            editObject = new AspNetUsers();
            editObject.SelectedUri = "Live";
            editObject.SelectedCompanyId = sessionState.User.SelectedCompanyId;

            selectedUserState.TaskObject = editObject;
            selectedUserState.selectedOption = editOption;

            ToggleDetail?.Invoke();
        }

        private async void DataChanged()
        {
            lstUserDataItems = await userAccess.GetUsersWithCompanyInfo();
            lstRoles = await roleAccess.GetUserRoles();

            StateHasChanged();
        }

        private void changeNameFilter(ChangeEventArgs eventArgs)
        {
            filterName = eventArgs.Value.ToString();
        }

        private void ToggleMoreOption(UserDataCollectionItem hoveredItem)
        {
            hoveredItem.OnHover = !hoveredItem.OnHover;
        }

        protected void PrepareModalForDelete(AspNetUsers selectedUser)
        {
            editObject = selectedUser;

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
            await userAccess.Delete(editObject);

            DataChanged();
        }

    }
}
