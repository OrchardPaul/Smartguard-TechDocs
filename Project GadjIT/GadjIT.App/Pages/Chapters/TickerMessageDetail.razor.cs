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
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace GadjIT_App.Pages.Chapters
{
    public partial class TickerMessageDetail
    {

        [Inject]
        IAppChapterState appChapterState { get; set; }

        [Inject]
        IUserSessionState sessionState { get; set; }


        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Inject]
        IChapterManagementService chapterManagementService { get; set; }

        public string filterText { get; set; } = "";

        [Parameter]
        public string Option { get; set; }

        [Parameter]
        public UsrOrsfSmartflows SelectedChapterObject { get; set; }

        [Parameter]
        public VmChapter SelectedChapter { get; set; }

        [Parameter]
        public TickerMessages TaskObject { get; set; }

        [Parameter]
        public TickerMessages CopyObject { get; set; }

        [Parameter]
        public Action DataChanged { get; set; }

        public DateTime FromDate { 
            get 
            {
                return CopyObject.FromDate is null ? DateTime.Now : DateTime.ParseExact(CopyObject.FromDate, "yyyyMMdd", CultureInfo.InvariantCulture);
            } 
            set
            {
                CopyObject.FromDate = value.ToString("yyyyMMdd");
            }
            }

        public DateTime ToDate
        {
            get
            {
                return CopyObject.ToDate is null ? DateTime.Now : DateTime.ParseExact(CopyObject.ToDate, "yyyyMMdd", CultureInfo.InvariantCulture);
            }
            set
            {
                CopyObject.ToDate = value.ToString("yyyyMMdd");
            }
        }

        public List<string> documentList;

        private async void Close()
        {
            await ModalInstance.CloseAsync();
        }

        private async Task HandleValidSubmit()
        {
            TaskObject.SeqNo = CopyObject.SeqNo;
            TaskObject.Message = CopyObject.Message;
            TaskObject.FromDate = CopyObject.FromDate;
            TaskObject.ToDate = CopyObject.ToDate;

            if (Option == "Insert")
            {
                if (SelectedChapter.TickerMessages is null)
                {
                    SelectedChapter.TickerMessages = new List<TickerMessages>();
                }

                SelectedChapter.TickerMessages.Add(TaskObject);
            }

            SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(SelectedChapter);
            await chapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);

            //keep track of time last updated ready for comparison by other sessions checking for updates
            appChapterState.SetLastUpdated(sessionState, SelectedChapter);

            CopyObject = new TickerMessages();
            filterText = "";

            DataChanged?.Invoke();
            Close();

        }


    }
}
