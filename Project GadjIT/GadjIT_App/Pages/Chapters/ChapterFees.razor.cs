using Blazored.Modal;
using GadjIT.ClientContext.P4W;
using GadjIT.ClientContext.P4W.Custom;
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

namespace GadjIT_App.Pages.Chapters
{
    public partial class ChapterFees
    {
        private class PostingType
        {
            public string Code { get; set; }
            public string Description { get; set; }
        }

        public bool VATable
        {
            get { return CopyObject.VATable == "Y" ? true : false; }
            set 
            {
                if (value)
                {
                    CopyObject.VATable = "Y";
                }
                else
                {
                    CopyObject.VATable = "N";
                }
            }
        }


        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Inject]
        IChapterManagementService chapterManagementService { get; set; }

        [Parameter]
        public string Option { get; set; }

        [Parameter]
        public UsrOrsfSmartflows SelectedChapterObject { get; set; }

        [Parameter]
        public VmChapter SelectedChapter { get; set; }

        [Parameter]
        public Fee TaskObject { get; set; }

        [Parameter]
        public Fee CopyObject { get; set; }

        [Parameter]
        public Action DataChanged { get; set; }

        [Inject]
        IAppChapterState appChapterState { get; set; }

        [Inject]
        IUserSessionState UserSession { get; set; }

        private int selectedCaseTypeGroup { get; set; } = -1;

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
            TaskObject = new Fee();
            await ModalInstance.CloseAsync();


        }

        private async Task HandleValidSubmit()
        {
            TaskObject.FeeName = CopyObject.FeeName;
            TaskObject.FeeCategory = CopyObject.FeeCategory;
            TaskObject.SeqNo = CopyObject.SeqNo;
            TaskObject.Amount = CopyObject.Amount;
            TaskObject.VATable = CopyObject.VATable;
            TaskObject.PostingType = CopyObject.PostingType;
            
            if (Option == "Insert")
            {
                SelectedChapter.Fees.Add(TaskObject);
            }

            SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(SelectedChapter);
            await chapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);

            TaskObject = new Fee();

            //keep track of time last updated ready for comparison by other sessions checking for updates
            appChapterState.SetLastUpdated(UserSession, SelectedChapter);

            DataChanged?.Invoke();
            Close();

        }

    }
}
