using Blazored.Modal;
using Blazored.Modal.Services;
using Gizmo.Context.Gizmo_Authentification;
using Gizmo.Context.Gizmo_Authentification.Custom;
using Gizmo_V1_02.Data.Admin;
using Gizmo_V1_02.Pages.Shared.Modals;
using Gizmo_V1_02.Services.SessionState;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Pages.SystemNav.CompanyManagement
{
    public partial class ManageCompanies
    {
        [Inject]
        IModalService Modal { get; set; }

        [Inject]
        ICompanyDbAccess companyDbAccess { get; set; }

        [Inject]
        IUserSessionState sessionState { get; set; }

        private List<VmCompanyDetails> lstCompanyDetails;

        public AppCompanyDetails editCompany = new AppCompanyDetails();

        protected override async Task OnInitializedAsync()
        {
            bool gotLock = sessionState.Lock;
            while (gotLock)
            {
                await Task.Yield();
                gotLock = sessionState.Lock;
            }

            var lstAppCompanyDetails = await companyDbAccess.GetCompanies();
            lstCompanyDetails = lstAppCompanyDetails
                                            .Select(A => new VmCompanyDetails { Company = A, OnHover = false })
                                            .ToList();

        }

        private async void DataChanged()
        {
            var lstAppCompanyDetails = await companyDbAccess.GetCompanies();
            lstCompanyDetails = lstAppCompanyDetails
                                            .Select(A => new VmCompanyDetails { Company = A })
                                            .ToList();

            StateHasChanged();
        }

        protected void PrepareForEdit(AppCompanyDetails seletedRole)
        {
            editCompany = seletedRole;

            ShowEditModal();
        }

        protected void PrepareForDelete(AppCompanyDetails seletedRole)
        {
            editCompany = seletedRole;

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
            editCompany = new AppCompanyDetails();

            ShowEditModal();
        }

        protected void ShowEditModal()
        {
            Action Action = DataChanged;

            var parameters = new ModalParameters();
            parameters.Add("TaskObject", editCompany);
            parameters.Add("DataChanged", Action);

            Modal.Show<CompanyEdit>("Edit Company", parameters);
        }

        private async void HandleValidDelete()
        {
            await companyDbAccess.DeleteCompany(editCompany);

            DataChanged();
        }
    }
}
