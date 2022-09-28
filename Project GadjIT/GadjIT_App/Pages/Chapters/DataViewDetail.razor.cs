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

namespace GadjIT_App.Pages.Chapters
{
    public partial class DataViewDetail
    {

        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Inject]
        IChapterManagementService chapterManagementService { get; set; }

        [Inject]
        IAppChapterState appChapterState { get; set; }

        [Inject]
        IUserSessionState UserSession { get; set; }

        public string filterText { get; set; } = "";

        [Parameter]
        public string Option { get; set; }

        [Parameter]
        public List<MpSysViews> ListPartnerViews { get; set; }

        [Parameter]
        public UsrOrsfSmartflows SelectedChapterObject { get; set; }

        [Parameter]
        public VmChapter SelectedChapter { get; set; }

        [Parameter]
        public DataViews TaskObject { get; set; }

        [Parameter]
        public DataViews CopyObject { get; set; }


        [Parameter]
        public Action DataChanged { get; set; }

        [Parameter]
        public IPartnerAccessService PartnerAccessService { get; set; }

        [Parameter]
        public Action RefreshViewList { get; set; }

        public string Level { get; set; } = "Matter";

        public List<string> documentList;

        private async void Close()
        {
            CopyObject = new DataViews();
            await ModalInstance.CloseAsync();
        }

        private async Task HandleValidSubmit()
        {
            TaskObject.BlockNo = CopyObject.BlockNo;
            TaskObject.DisplayName = CopyObject.DisplayName;
            TaskObject.ViewName = CopyObject.ViewName;

            if (Option == "Insert")
            {
                if(SelectedChapter.DataViews is null)
                {
                    SelectedChapter.DataViews = new List<DataViews>();
                }

                SelectedChapter.DataViews.Add(TaskObject);
            }

            SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(SelectedChapter);
            await chapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);

            CopyObject = new DataViews();
            filterText = "";


            //keep track of time last updated ready for comparison by other sessions checking for updates
            appChapterState.SetLastUpdated(UserSession, SelectedChapter);

            DataChanged?.Invoke();
            Close();

        }

        private async Task RefreshViewListOnModel()
        {
            ListPartnerViews = await PartnerAccessService.GetPartnerViews();
            StateHasChanged();
            RefreshViewList?.Invoke();
        }



    }
}
