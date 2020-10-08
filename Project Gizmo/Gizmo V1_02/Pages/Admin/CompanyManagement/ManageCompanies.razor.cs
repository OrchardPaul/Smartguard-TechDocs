using Gizmo.Context.Gizmo_Authentification;
using Gizmo_V1_02.Data.Admin;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Pages.Admin.CompanyManagement
{
    public partial class ManageCompanies
    {
        [Inject]
        ICompanyDbAccess companyDbAccess { get; set; }

        private List<AppCompanyDetails> lstCompanyDetails;

        public AppCompanyDetails editCompany = new AppCompanyDetails();

        protected override async Task OnInitializedAsync()
        {
            lstCompanyDetails = await companyDbAccess.GetCompanies();
        }

        private async void DataChanged()
        {
            lstCompanyDetails = await companyDbAccess.GetCompanies();

            StateHasChanged();
        }

        protected void PrepareForEdit(AppCompanyDetails seletedRole)
        {
            editCompany = seletedRole;
        }

        protected void PrepareForDelete(AppCompanyDetails seletedRole)
        {
            editCompany = seletedRole;
        }

        protected void PrepareForInsert()
        {
            editCompany = new AppCompanyDetails();
        }
    }
}
