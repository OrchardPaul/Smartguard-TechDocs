using Blazored.Modal;
using GadjIT_AppContext.GadjIT_App;
using GadjIT_App.Data.Admin;
using Microsoft.AspNetCore.Components;
using System;

namespace GadjIT_App.Pages.Accounts.CompanyAccountManagement
{
    public partial class CompanyAccountsDetail
    {

        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Inject]
        ICompanyDbAccess CompanyDbAccess { get; set; }

        [Parameter]
        public AppCompanyAccountsSmartflowDetails SelectedAccountDetailsObject { get; set; }

        [Parameter]
        public AppCompanyAccountsSmartflowDetails CopyObject { get; set; }


        [Parameter]
        public Action DataChanged { get; set; }

        private async void Close()
        {
            await ModalInstance.CloseAsync();
        }

        private async void HandleValidSubmit()
        {
            SelectedAccountDetailsObject.MonthsDuration = CopyObject.MonthsDuration;
            SelectedAccountDetailsObject.StartDate = CopyObject.StartDate;
            SelectedAccountDetailsObject.Status = CopyObject.Status;
            SelectedAccountDetailsObject.Billable = CopyObject.Billable;
            SelectedAccountDetailsObject.BillingDescription = CopyObject.BillingDescription;
            SelectedAccountDetailsObject.CreatedBy = CopyObject.CreatedBy;
            SelectedAccountDetailsObject.System = CopyObject.System;
            SelectedAccountDetailsObject.DeletedDate = CopyObject.DeletedDate;
            SelectedAccountDetailsObject.MonthlyCharge = CopyObject.MonthlyCharge;
            SelectedAccountDetailsObject.MonthsRemaining = CopyObject.MonthsRemaining;
            SelectedAccountDetailsObject.TotalBilled = CopyObject.TotalBilled;
            SelectedAccountDetailsObject.Outstanding = CopyObject.Outstanding;
            SelectedAccountDetailsObject.CompanyId = CopyObject.CompanyId;
            SelectedAccountDetailsObject.ClientRowId = CopyObject.ClientRowId;

            SelectedAccountDetailsObject = await CompanyDbAccess.UpdateSmartflowAccountDetails(SelectedAccountDetailsObject).ConfigureAwait(false);
           
            DataChanged?.Invoke();
            Close();

        }
    }
}
