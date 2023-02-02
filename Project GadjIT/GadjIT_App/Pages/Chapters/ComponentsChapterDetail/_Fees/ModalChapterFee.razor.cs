using Blazored.Modal;
using GadjIT_ClientContext.P4W;
using GadjIT_ClientContext.P4W.Custom;
using GadjIT_App.Services;
using GadjIT_App.Services.AppState;
using GadjIT_App.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace GadjIT_App.Pages.Chapters.ComponentsChapterDetail._Fees
{
    public partial class ModalChapterFee
    {
        


        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }


        [Parameter]
        public string _Option { get; set; }

        [Parameter]
        public UsrOrsfSmartflows _SelectedChapterObject { get; set; }

        [Parameter]
        public VmSmartflow _SelectedChapter { get; set; }

        [Parameter]
        public Fee _TaskObject { get; set; }

        [Parameter]
        public Fee _CopyObject { get; set; }

        [Parameter]
        public Action _DataChanged { get; set; }


        [Inject]
        ILogger<ModalChapterFee> Logger {get; set;}

        [Inject]
        INotificationManager NotificationManager {get; set;}


        [Inject]
        IChapterManagementService ChapterManagementService { get; set; }

        
        [Inject]
        IUserSessionState UserSession { get; set; }


        public int Error { get; set; } = 0;

        private class PostingType
        {
            public string Code { get; set; }
            public string Description { get; set; }
        }

        public bool VATable
        {
            get { return _CopyObject.VATable == "Y" ? true : false; }
            set 
            {
                if (value)
                {
                    _CopyObject.VATable = "Y";
                }
                else
                {
                    _CopyObject.VATable = "N";
                }
            }
        }

        private List<PostingType> PostingTypes { get; set; } = new List<PostingType> 
                                                                            {
                                                                                  new PostingType {Code = "DSO", Description = "DSO - Office Payout to Bank (Disbursement)" }
                                                                                , new PostingType {Code = "DSP", Description = "DSP - Office Payout to Petty Cash (Disbursement)"}
                                                                                , new PostingType {Code = "WOD", Description = "WOD - Office Write Off Debt (Write Off Debt)"}
                                                                                , new PostingType {Code = "O/N", Description = "O/N - Office Payout to Non-Bank (Office to Nominal Transfer)"}
                                                                                , new PostingType {Code = "OCR", Description = "OCR - Office Received to Bank (Office Credit)"}
                                                                                , new PostingType {Code = "OCP", Description = "OCP - Office Received to Petty Cash (Office Credit)"}
                                                                                , new PostingType {Code = "OTO", Description = "OTO - Office Transfer to Office (Office to Office Transfer)"}
                                                                                , new PostingType {Code = "OTC", Description = "OTC - Office Transfer to Client (Office to Client Transfer)"}
                                                                                , new PostingType {Code = "CDR", Description = "CDR - Client Payout (Client Debit)"}
                                                                                , new PostingType {Code = "CCR", Description = "CCR - Client Received (Client Credit)"}
                                                                                , new PostingType {Code = "CIN", Description = "CIN - Client Interest (Client Interest)"}
                                                                                , new PostingType {Code = "CTO", Description = "CTO - Client Transfer to Office (Client to Office Transfer)"}
                                                                                , new PostingType {Code = "CTC", Description = "CTC - Client Transfer to Client (Client to Client Transfer)"}
                                                                                , new PostingType {Code = "CTD", Description = "CTD - Client Transfer to Deposit (Client to Designated Deposit)"}
                                                                                , new PostingType {Code = "DFD", Description = "DFD - Deposit Payout (Direct from Designated Deposit)"}
                                                                                , new PostingType {Code = "DOD", Description = "DOD - Deposit Received (Direct on Deposit)" }
                                                                            };

        private List<string> FeeCategories { get; set; } = new List<string>
                                                                            {
                                                                                "Disbursement"
                                                                                ,"Additional Fee"
                                                                                ,"Our Fee"
                                                                                ,"Other"
                                                                            };

        private async void Close()
        {
            _TaskObject = new Fee();
            await ModalInstance.CloseAsync();


        }

        private void HandleValidSubmit()
        {
            try
            {
                _TaskObject.FeeName = _CopyObject.FeeName;
                _TaskObject.FeeCategory = _CopyObject.FeeCategory;
                _TaskObject.SeqNo = _CopyObject.SeqNo;
                _TaskObject.Amount = _CopyObject.Amount;
                _TaskObject.VATable = _CopyObject.VATable;
                _TaskObject.PostingType = _CopyObject.PostingType;
                
                if (_Option == "Insert")
                {
                    _SelectedChapter.Fees.Add(_TaskObject);
                }

                _TaskObject = new Fee();

                _DataChanged?.Invoke();
                Close();
            }
            catch(Exception e)
            {
                GenericErrorLog(true,e, "HandleValidSubmit", e.Message);
            }
            finally
            {
                Close();
            }
        }

        private void ResetError()
        {
            Error = 0;
            StateHasChanged();
        }

        /****************************************/
        /* ERROR HANDLING */
        /****************************************/
        private async void GenericErrorLog(bool showNotificationMsg, Exception e, string _method, string _message)
        {
            if(showNotificationMsg)
            {
                await NotificationManager.ShowNotification("Danger", $"Oops! Something went wrong.").ConfigureAwait(false);
            }

            using (LogContext.PushProperty("SourceSystem", UserSession.SelectedSystem))
            using (LogContext.PushProperty("SourceCompanyId", UserSession.Company.Id))
            using (LogContext.PushProperty("SourceUserId", UserSession.User.Id))
            using (LogContext.PushProperty("SourceContext", nameof(ModalChapterFee)))
            {
                Logger.LogError(e,"Error - Method: {0}, Message: {1}",_method, _message);
            }

        }

    }
}
