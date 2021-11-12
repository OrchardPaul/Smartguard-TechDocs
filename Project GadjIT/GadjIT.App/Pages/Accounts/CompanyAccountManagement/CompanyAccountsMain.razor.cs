using Blazored.Modal;
using Blazored.Modal.Services;
using GadjIT.AppContext.GadjIT_App;
using GadjIT.AppContext.GadjIT_App.Custom;
using GadjIT_App.Data.Admin;
using GadjIT_App.FileManagement.FileProcessing.Interface;
using GadjIT_App.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GadjIT_App.Pages.Accounts.CompanyAccountManagement
{

    public class JsConsole
    {
        private readonly IJSRuntime JsRuntime;
        public JsConsole(IJSRuntime jSRuntime)
        {
            this.JsRuntime = jSRuntime;
        }

        public async Task LogAsync(string message)
        {
            await this.JsRuntime.InvokeVoidAsync("console.log", message);
        }
    }

    public partial class CompanyAccountsMain
    {


        [Inject]
        IModalService Modal { get; set; }

        [Inject]
        public IUserSessionState SessionState { get; set; }
        
        [Inject]
        public JsConsole jsConsole { get; set; }

        [Inject]
        public IFileHelper FileHelper { get; set; }

        [Inject]
        public IPDFHelper PDFHelper { get; set; }

        [Inject]
        public ICompanyDbAccess CompanyDbAccess { get; set; }

        [Inject]
        public IExcelHelper ExcelHelper { get; set; }

        public string navDisplay = "Companies";

        public bool displaySpinner = true;

        public List<AppCompanyDetails> AppCompanyDetails { get; set; }

        public List<AppCompanyAccountsSmartflow> AppCompanyAccounts { get; set; }

        public List<AppCompanyAccountsSmartflowDetails> AppCompanyAccountsDetails { get; set; }

        public List<CompanyAccountObject> CompanyAccountObjects { get; set; } = new List<CompanyAccountObject>();

        public List<SmartflowRecords> AllRecords { get; set; } = new List<SmartflowRecords>();

        public AppCompanyAccountsSmartflow SelectedCompanyAccount { get; set; }

        public string SelectedSystem { get; set; } = "Live";

        protected override async Task OnInitializedAsync()
        {
            try
            {
                displaySpinner = true;

                AppCompanyDetails = await CompanyDbAccess.GetCompanies();

                AppCompanyAccounts = await CompanyDbAccess.GetCompanyAccounts();

                AllRecords = await CompanyDbAccess.GetAllSmartflowRecordsForAllCompanies();

                SelectedCompanyAccount = AppCompanyAccounts.Where(A => A.CompanyId == SessionState.Company.Id).FirstOrDefault();

                RefreshCompanyObjects();

                RefreshCompanyAccounts();

                displaySpinner = false;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        protected async void RefreshCompanyObjects()
        {
            var companyAccountObject = new CompanyAccountObject();

            CompanyAccountObjects.Clear();

            foreach (var account in AppCompanyAccounts)
            {
                AppCompanyAccountsDetails = await CompanyDbAccess.GetCompanyAccountDetailsByAccountId(account.Id);

                companyAccountObject = new CompanyAccountObject
                {
                    CompanyObject = AppCompanyDetails.Where(D => D.Id == account.CompanyId).FirstOrDefault(),
                    AccountObject = account,
                    SmartflowAccounts = AppCompanyAccountsDetails
                };

                CompanyAccountObjects.Add(companyAccountObject);
            }

            StateHasChanged();
        }



        protected void ShowNav(string displayChange)
        {
            navDisplay = displayChange;
        }

        protected async void RefreshCompanyAccounts()
        {
            bool gotLock = CompanyDbAccess.Lock;
            while (gotLock)
            {
                await Task.Yield();
                gotLock = CompanyDbAccess.Lock;
            }

            await CompanyDbAccess.RefreshAccounts();
        }

        protected void SelectCompany(AppCompanyAccountsSmartflow selectedCompany)
        {
            SelectedCompanyAccount = selectedCompany;

            navDisplay = "Accounts";

            StateHasChanged();
        }

        protected void ShowCompanyAccountsDetailModal(AppCompanyAccountsSmartflowDetails taskObject)
        {
            Action dataChanged = RefreshCompanyObjects;
            AppCompanyAccountsSmartflowDetails copyObject = new AppCompanyAccountsSmartflowDetails
            {
                SmartflowAccountId = taskObject.SmartflowAccountId,
                SmartflowRecordId = taskObject.SmartflowRecordId,
                StartDate = taskObject.StartDate,
                EndDate = taskObject.EndDate,
                Status = taskObject.Status,
                Billable = taskObject.Billable,
                BillingDescription = taskObject.BillingDescription,
                CreatedBy = taskObject.CreatedBy,
                System = taskObject.System,
                DeletedDate = taskObject.DeletedDate,
                MonthlyCharge = taskObject.MonthlyCharge,
                MonthsDuration = taskObject.MonthsDuration,
                MonthsRemaining = taskObject.MonthsRemaining,
                TotalBilled = taskObject.TotalBilled,
                Outstanding = taskObject.Outstanding
            };

            var parameters = new ModalParameters();
            parameters.Add("CopyObject", copyObject);
            parameters.Add("SelectedAccountDetailsObject", taskObject);
            parameters.Add("SelectedAccountSmartflowObject", AllRecords.Where(A => A.Id == copyObject.SmartflowRecordId).FirstOrDefault());
            parameters.Add("DataChanged", dataChanged);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-chapter-fees"
            };


            Modal.Show<CompanyAccountsDetail>("Account Detail", parameters, options);
        }

        public async Task ExportToPdf()
        {
            var data = PDFHelper.GenerateReport(CompanyAccountObjects);
            await FileHelper.DownloadFile("Accounts.pdf", data);
        }

        public async Task Bill()
        {
            var selectedAccountObject = CompanyAccountObjects
                                                        .Where(A => A.AccountObject.Id == SelectedCompanyAccount.Id)
                                                        .FirstOrDefault();

            var billingItems = await CompanyDbAccess.BillCompany(selectedAccountObject
                                                                    , AllRecords);

            await ExcelHelper.WriteChapterDataToExcel(selectedAccountObject, billingItems);

            RefreshCompanyObjects();
        }

    }
}
