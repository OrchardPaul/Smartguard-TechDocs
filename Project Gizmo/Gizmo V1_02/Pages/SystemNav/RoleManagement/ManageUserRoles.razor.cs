using Blazored.Modal;
using Blazored.Modal.Services;
using Gizmo.Context.Gizmo_Authentification;
using Gizmo_V1_02.Data.Admin;
using Gizmo_V1_02.Pages.Shared.Modals;
using Gizmo_V1_02.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Pages.SystemNav.RoleManagement
{
    public partial class ManageUserRoles
    {
        [Inject]
        IIdentityRoleAccess IdentityService { get; set; }

        [Inject]
        IModalService Modal { get; set; }

        [Inject]
        IUserSessionState sessionState { get; set; }

        private List<AspNetRoles> lstRoles;

        public AspNetRoles editRole = new AspNetRoles();

        protected override async Task OnInitializedAsync()
        {
            bool gotLock = sessionState.Lock;
            while (gotLock)
            {
                await Task.Yield();
                gotLock = sessionState.Lock;
            }


            lstRoles = await IdentityService.GetUserRoles();

        }

        private async void HandleValidDelete()
        {
            await IdentityService.Delete(editRole);

            DataChanged();
        }

        private async void DataChanged()
        {
            lstRoles = await IdentityService.GetUserRoles();

            StateHasChanged();
        }

        protected void PrepareForEdit(AspNetRoles seletedRole)
        {
            editRole = seletedRole;

            ShowEditModal();
        }

        protected void PrepareForDelete(AspNetRoles seletedRole)
        {
            editRole = seletedRole;

            Action SelectedDeleteAction = HandleValidDelete;
            var parameters = new ModalParameters();
            parameters.Add("InfoHeader", "Delete?");
            parameters.Add("ModalHeight", "300px");
            parameters.Add("ModalWidth", "500px");
            parameters.Add("DeleteAction", SelectedDeleteAction);

            Modal.Show<ModalDelete>("Delete?", parameters);
        }

        protected void PrepareForInsert()
        {
            editRole = new AspNetRoles();

            ShowEditModal();
        }

        protected void ShowEditModal()
        {
            Action Action = DataChanged;

            var parameters = new ModalParameters();
            parameters.Add("TaskObject", editRole);
            parameters.Add("DataChanged", Action);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal"
            };

            Modal.Show<RoleDetail>("Edit Role", parameters, options);
        }

    }
}
