using GadjIT_AppContext.GadjIT_App;
using GadjIT_App.Services.SessionState;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GadjIT_App.Pages.Admin.UserManagement
{
    public partial class ManageUsersDropDownMenu
    {
        [Parameter]
        public Action ToggleDetail { get; set; }

        [Parameter]
        public AspNetUsers SelectedUser { get; set; }

        [Inject]
        public IUserSessionState SessionState { get; set; }

        [Inject]
        private IUserManagementSelectedUserState SelectedUserState { get; set; }

        protected void PrepareForEdit()
        {
            SelectedUser.SelectedUri = (SessionState.SelectedSystem is null) ? "Live" : SessionState.SelectedSystem;
            SelectedUser.PasswordHash = "PasswordNotChanged115592!";

            SelectedUserState.TaskObject = SelectedUser;
            SelectedUserState.selectedOption = "Edit";

            ToggleDetail?.Invoke();
        }

    }
}
