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

namespace GadjIT_App.Pages.Chapters.ComponentsChapterDetail._DataView
{
    public partial class ModalChapterDataViewDetail
    {

        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Parameter]
        public string _Option { get; set; }

        [Parameter]
        public List<MpSysViews> _ListP4WViews { get; set; }

        [Parameter]
        public UsrOrsfSmartflows _SelectedChapterObject { get; set; }

        [Parameter]
        public VmChapter _SelectedChapter { get; set; }

        [Parameter]
        public DataView _TaskObject { get; set; }

        [Parameter]
        public DataView _CopyObject { get; set; }

        [Parameter]
        public Action _DataChanged { get; set; }

        

        [Inject]
        IChapterManagementService ChapterManagementService { get; set; }

        [Inject]
        public IPartnerAccessService PartnerAccessService { get; set; }

        [Inject]
        IAppChapterState AppChapterState { get; set; }

        [Inject]
        IUserSessionState UserSession { get; set; }


        public string FilterText { get; set; } = "";

        private async Task Close()
        {
            _CopyObject = new DataView();
            await ModalInstance.CloseAsync();
        }

        private void HandleValidSubmit()
        {
            _TaskObject.SeqNo = _CopyObject.SeqNo;
            _TaskObject.DisplayName = _CopyObject.DisplayName;
            _TaskObject.ViewName = _CopyObject.ViewName;

            if (_Option == "Insert")
            {
                if(_SelectedChapter.DataViews is null)
                {
                    _SelectedChapter.DataViews = new List<DataView>();
                }

                _SelectedChapter.DataViews.Add(_TaskObject);
            }

            _SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(_SelectedChapter);
            var returnChapterObject =  ChapterManagementService.Update(_SelectedChapterObject);

            _CopyObject = new DataView();
            FilterText = "";


            //keep track of time last updated ready for comparison by other sessions checking for updates
            AppChapterState.SetLastUpdated(UserSession, _SelectedChapter);

            _DataChanged?.Invoke();
            Close();

        }

        private async Task RefreshViewListOnModel()
        {
            _ListP4WViews = await PartnerAccessService.GetPartnerViews();
            StateHasChanged();
        }



    }
}
