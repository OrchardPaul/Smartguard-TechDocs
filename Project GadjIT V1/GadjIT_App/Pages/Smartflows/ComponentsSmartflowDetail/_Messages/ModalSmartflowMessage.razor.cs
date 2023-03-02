using Blazored.Modal;
using GadjIT_App.Services;
using GadjIT_App.Services.AppState;
using GadjIT_App.Services.SessionState;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Globalization;
using GadjIT_ClientContext.Models.Smartflow;
using GadjIT_ClientContext.Models.Smartflow.Client;

namespace GadjIT_App.Pages.Smartflows.ComponentsSmartflowDetail._Messages
{
    public partial class ModalSmartflowMessage
    {

        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }


        [Inject]
        IAppSmartflowsState AppSmartflowsState { get; set; }

        [Inject]
        IUserSessionState UserSession { get; set; }

        [Inject]
        IClientApiManagementService ClientApiManagementService { get; set; }


        [Parameter]
        public string _Option { get; set; }

        [Parameter]
        public Client_SmartflowRecord _Selected_ClientSmartflowRecord { get; set; }

        [Parameter]
        public Smartflow _SelectedSmartflow { get; set; }

        [Parameter]
        public SmartflowMessage _TaskObject { get; set; }

        [Parameter]
        public SmartflowMessage _CopyObject { get; set; }

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
                if (_SelectedSmartflow.TickerMessages is null)
                {
                    _SelectedSmartflow.TickerMessages = new List<SmartflowMessage>();
                }

                _SelectedSmartflow.TickerMessages.Add(_TaskObject);
            }


            _CopyObject = new SmartflowMessage();
            FilterText = "";

            _DataChanged?.Invoke();
            Close();

        }


    }
}
