using Blazored.Modal;
using Blazored.Modal.Services;
using GadjIT_AppContext.GadjIT_App;
using GadjIT_AppContext.GadjIT_App.Custom;
using GadjIT_App.Data.Admin;
using GadjIT_App.FileManagement.FileProcessing.Interface;
using GadjIT_App.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.Extensions.Logging;
using GadjIT_App.Pages._Shared.Modals;

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
        public IModalService Modal { get; set; }

        [Inject]
        public IUserSessionState SessionState { get; set; }
        
        [Inject]
        public JsConsole JsConsole { get; set; }

        [Inject]
        public IFileHelper FileHelper { get; set; }

        [Inject]
        public IPDFHelper PDFHelper { get; set; }

        [Inject]
        public ICompanyDbAccess CompanyDbAccess { get; set; }

        [Inject]
        public IExcelHelper ExcelHelper { get; set; }

        [Inject]
        private ILogger<CompanyAccountsMain> Logger { get; set; }

        public string NavDisplay = "Companies";

        public bool DisplaySpinner = true;

        public List<AppCompanyDetails> AppCompanyDetails { get; set; }

        public List<AppCompanyAccountsSmartflow> AppCompanyAccounts { get; set; }

        public List<AppCompanyAccountsSmartflowDetails> AppCompanyAccountsDetails { get; set; }

        public List<CompanyAccountObject> CompanyAccountObjects { get; set; } = new List<CompanyAccountObject>();

        public List<App_SmartflowRecord> AllRecords { get; set; } = new List<App_SmartflowRecord>();

        public AppCompanyAccountsSmartflow SelectedCompanyAccount { get; set; }

        public bool Draft { get; set; }

        public bool SortByStartDate { get; set; }

        public bool SortByStartDateDesc { get; set; }

        public bool SortByEndDate { get; set; }

        public bool SortByEndDateDesc { get; set; }

        public bool SortByStatus { get; set; }

        public bool SortByStatusDesc { get; set; }

        public string SelectedSystem { get; set; } = "Live";
        
        public string SelectedStatus { get; set; } = "Active";

        /// <summary>
        /// TODO: Not sure why but calling the RefreshCompanyObjects procedure causes a crash
        /// Fixed by copying the code from the RefreshCompanyObjects but could do with figuring out why this happens
        /// </summary>
        /// <returns></returns>
        
        protected override async Task OnInitializedAsync()
        {
            await RefreshCompanyAccounts();
        }

        protected async Task RefreshPage()
        {
            try
            {
                DisplaySpinner = true;

                bool gotLock = SessionState.Lock;
                while (gotLock)
                {
                    await Task.Yield();
                    gotLock = SessionState.Lock;
                }

                AppCompanyDetails = await CompanyDbAccess.GetCompanies();

                AppCompanyAccounts = await CompanyDbAccess.GetCompanyAccounts();

                AllRecords = await CompanyDbAccess.GetAllAppSmartflowRecordsForAllCompanies();

                SelectedCompanyAccount = AppCompanyAccounts.Where(A => A.CompanyId == SessionState.Company.Id).FirstOrDefault();


                var companyAccountObject = new CompanyAccountObject();

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

                await InvokeAsync(() =>
                {
                    StateHasChanged();
                });

                DisplaySpinner = false;

                StateHasChanged();
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
            await InvokeAsync(() =>
            {
                StateHasChanged();
            });

        }



        protected void ShowNav(string displayChange)
        {
            NavDisplay = displayChange;
        }

        protected async Task RefreshCompanyAccounts()
        {
            bool gotLock = SessionState.Lock;
            while (gotLock)
            {
                await Task.Yield();
                gotLock = SessionState.Lock;
            }

            await CompanyDbAccess.RefreshAccounts();

            await RefreshPage();
        }

        protected void SelectCompany(AppCompanyAccountsSmartflow selectedCompany)
        {
            SelectedCompanyAccount = selectedCompany;
            
            NavDisplay = "Accounts";

            StateHasChanged();
        }

        protected void ShowCompanyAccountsDetailModal(AppCompanyAccountsSmartflowDetails taskObject)
        {
            Action dataChanged = RefreshCompanyObjects;
            AppCompanyAccountsSmartflowDetails copyObject = new AppCompanyAccountsSmartflowDetails
            {
                SmartflowAccountId = taskObject.SmartflowAccountId,
                ClientRowId = taskObject.ClientRowId,
                CompanyId = taskObject.CompanyId,
                SmartflowName = taskObject.SmartflowName,
                CaseType = taskObject.CaseType,
                CaseTypeGroup = taskObject.CaseTypeGroup,
                StartDate = taskObject.StartDate,
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
            parameters.Add("DataChanged", dataChanged);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-smartflow-fees"
            };


            Modal.Show<CompanyAccountsDetail>("Account Detail", parameters, options);
        }

        public async Task ExportToPdf()
        {
            //TODO: Restore this method but with iText7 or alternative
            // var data = PDFHelper.GenerateReport(CompanyAccountObjects);
            // await FileHelper.DownloadFile("Accounts.pdf", data);
        }

        public async void Bill()
        {
            var selectedAccountObject = CompanyAccountObjects
                                                        .Where(A => A.AccountObject.Id == SelectedCompanyAccount.Id)
                                                        .FirstOrDefault();

            var billingItems = await CompanyDbAccess.BillCompany(selectedAccountObject
                                                                    , AllRecords
                                                                    , Draft);

            await ExcelHelper.WriteSmartflowDataToExcel(selectedAccountObject, billingItems);

            RefreshCompanyObjects();
        }

        public async Task SaveBillablleChangesToSmartflowAccount(AppCompanyAccountsSmartflowDetails smartflowDetails)
        {
            smartflowDetails.Billable = !smartflowDetails.Billable;

            await CompanyDbAccess.UpdateSmartflowAccountDetails(smartflowDetails).ConfigureAwait(false);
        }

        public void BillNotDraft()
        {
            Draft = false; 
            ExecuteConfirm();
        }


        public void BillDraft()
        {
            Draft = true;

            ExecuteConfirm();
        }


        /// <summary>
        /// Brings up the confirm model
        /// </summary>
        protected void ExecuteConfirm()
        {
            string infoText = "Do you wish to proceed with billing?";

            Action SelectedAction = Bill;
            var parameters = new ModalParameters();
            parameters.Add("InfoHeader", "Confirm Action");
            parameters.Add("InfoText", infoText);
            parameters.Add("ConfirmAction", SelectedAction);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-confirm"
            };

            Modal.Show<ModalConfirm>("Confirm", parameters, options);
        }

        public void SwitchStartDateSort()
        {
            if (SortByStartDate)
            {
                SortByStartDate = false;
                SortByStartDateDesc = true;
            }
            else if (SortByStartDateDesc)
            {
                SortByStartDateDesc = false;
            }
            else
            {
                SortByStartDate = true;
            }

            SortByEndDate = false;
            SortByEndDateDesc = false;
            SortByStatus = false;
            SortByStatusDesc = false;

            StateHasChanged();
        }


        public void SwitchEndDateSort()
        {
            if (SortByEndDate)
            {
                SortByEndDate = false;
                SortByEndDateDesc = true;
            }
            else if (SortByEndDateDesc)
            {
                SortByEndDateDesc = false;
            }
            else
            {
                SortByEndDate = true;
            }

            SortByStartDate = false;
            SortByStartDateDesc = false;
            SortByStatus = false;
            SortByStatusDesc = false;

            StateHasChanged();
        }


        public void SwitchStatusSort()
        {
            if (SortByStatus)
            {
                SortByStatus = false;
                SortByStatusDesc = true;
            }
            else if (SortByStatusDesc)
            {
                SortByStatusDesc = false;
            }
            else
            {
                SortByStatus = true;
            }

            SortByEndDate = false;
            SortByEndDateDesc = false;
            SortByStartDate = false;
            SortByStartDateDesc = false;

            StateHasChanged();
        }

    }
}
