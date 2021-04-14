using Blazored.Modal;
using GadjIT.ClientContext.P4W;
using GadjIT.ClientContext.P4W.Custom;
using Gizmo_V1_02.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Pages.Chapters
{
    public partial class TickerMessageDetail
    {

        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Inject]
        IChapterManagementService chapterManagementService { get; set; }

        public string filterText { get; set; } = "";

        [Parameter]
        public string Option { get; set; }

        [Parameter]
        public UsrOrDefChapterManagement SelectedChapterObject { get; set; }

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
                return CopyObject.FromDate is null ? DateTime.Now : DateTime.ParseExact(CopyObject.ToDate, "yyyyMMdd", CultureInfo.InvariantCulture);
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

        private async void HandleValidSubmit()
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

            SelectedChapterObject.ChapterData = JsonConvert.SerializeObject(SelectedChapter);
            await chapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);

            CopyObject = new TickerMessages();
            filterText = "";

            DataChanged?.Invoke();
            Close();

        }


    }
}
