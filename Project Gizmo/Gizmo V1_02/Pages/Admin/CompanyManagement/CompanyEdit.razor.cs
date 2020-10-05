using Gizmo.Context.Gizmo_Authentification;
using Gizmo_V1_02.Data.Admin;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Pages.Admin.CompanyManagement
{
    public partial class CompanyEdit
    {
        [Parameter]
        public CompanyDetails TaskObject { get; set; }

        [Parameter]
        public Action DataChanged { get; set; }

        [Inject]
        private ICompanyDbAccess service { get; set; }

        private async Task ClosechapterModal()
        {
            await jsRuntime.InvokeAsync<Object>("CloseModal", "companyModal");
        }

        private async void HandleValidSubmit()
        {
            await service.SubmitChanges(TaskObject);

            await ClosechapterModal();
            DataChanged?.Invoke();
        }

        private async void HandleValidDelete()
        {
            await service.DeleteCompany(TaskObject);

            await ClosechapterModal();
            DataChanged?.Invoke();
        }
    }
}
