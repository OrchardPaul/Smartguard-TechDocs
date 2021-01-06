using Gizmo.Context.Gizmo_Authentification;
using Gizmo_V1_02.Services.SessionState;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Pages.Admin.UserManagement
{
    public partial class ManageUsersDropDownMenu
    {
        [Parameter]
        public Action ToggleDetail { get; set; }

        [Parameter]
        public AspNetUsers selectedUser { get; set; }

        [Inject]
        public IUserSessionState sessionState { get; set; }

        [Inject]
        private IUserManagementSelectedUserState selectedUserState { get; set; }

        protected void PrepareForEdit()
        {
            selectedUser.SelectedUri = (sessionState.selectedSystem is null) ? "Live" : sessionState.selectedSystem;
            selectedUser.PasswordHash = "PasswordNotChanged115592!";

            selectedUserState.TaskObject = selectedUser;
            selectedUserState.selectedOption = "Edit";

            ToggleDetail?.Invoke();
        }

    }
}
