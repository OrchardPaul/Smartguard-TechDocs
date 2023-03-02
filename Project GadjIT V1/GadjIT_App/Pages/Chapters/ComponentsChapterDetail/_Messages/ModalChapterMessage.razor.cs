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
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace GadjIT_App.Pages.Chapters.ComponentsChapterDetail._Messages
{
    public partial class ModalChapterMessage
    {

        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }


        [Inject]
        IAppChapterState AppChapterState { get; set; }

        [Inject]
        IUserSessionState UserSession { get; set; }

        [Inject]
        IChapterManagementService ChapterManagementService { get; set; }


        [Parameter]
        public string _Option { get; set; }

        [Parameter]
        public UsrOrsfSmartflows _SelectedChapterObject { get; set; }

        [Parameter]
        public VmSmartflow _SelectedChapter { get; set; }

        [Parameter]
        public TickerMessage _TaskObject { get; set; }

        [Parameter]
        public TickerMessage _CopyObject { get; set; }

        [Parameter]
        public Action _DataChanged { get; set; }

        public string FilterText { get; set; } = "";

        public DateTime FromDate { 
            get 
            {
                return _CopyObject.FromDate is null ? DateTime.Now : DateTime.ParseExact(_CopyObject.FromDate, "yyyyMMdd", CultureInfo.InvariantCulture);
            } 
            set
            {
                _CopyObject.FromDate = value.ToString("yyyyMMdd");
            }
            }

        public DateTime ToDate
        {
            get
            {
                return _CopyObject.ToDate is null ? DateTime.Now : DateTime.ParseExact(_CopyObject.ToDate, "yyyyMMdd", CultureInfo.InvariantCulture);
            }
            set
            {
                _CopyObject.ToDate = value.ToString("yyyyMMdd");
            }
        }

        public List<string> documentList;

        private async void Close()
        {
            await ModalInstance.CloseAsync();
        }

        private void HandleValidSubmit()
        {
            _TaskObject.SeqNo = _CopyObject.SeqNo;
            _TaskObject.Message = _CopyObject.Message;
            _TaskObject.FromDate = _CopyObject.FromDate;
            _TaskObject.ToDate = _CopyObject.ToDate;

            if (_Option == "Insert")
            {
                if (_SelectedChapter.TickerMessages is null)
                {
                    _SelectedChapter.TickerMessages = new List<TickerMessage>();
                }

                _SelectedChapter.TickerMessages.Add(_TaskObject);
            }


            _CopyObject = new TickerMessage();
            FilterText = "";

            _DataChanged?.Invoke();
            Close();

        }


    }
}
