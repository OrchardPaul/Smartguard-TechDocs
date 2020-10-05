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

        private List<CompanyDetails> lstCompanyDetails;

        public CompanyDetails editCompany = new CompanyDetails();

        protected override async Task OnInitializedAsync()
        {
            lstCompanyDetails = await companyDbAccess.GetCompanies();
        }

        private async void DataChanged()
        {
            lstCompanyDetails = await companyDbAccess.GetCompanies();

            StateHasChanged();
        }

        protected void PrepareForEdit(CompanyDetails seletedRole)
        {
            editCompany = seletedRole;
        }

        protected void PrepareForDelete(CompanyDetails seletedRole)
        {
            editCompany = seletedRole;
        }

        protected void PrepareForInsert()
        {
            editCompany = new CompanyDetails();
        }
    }
}
