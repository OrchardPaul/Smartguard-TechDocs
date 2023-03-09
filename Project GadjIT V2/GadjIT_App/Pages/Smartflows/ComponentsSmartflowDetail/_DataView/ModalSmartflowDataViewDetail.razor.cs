using Blazored.Modal;
using GadjIT_App.Services;
using GadjIT_App.Services.AppState;
using GadjIT_App.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GadjIT_ClientContext.Models.P4W;
using GadjIT_ClientContext.Models.Smartflow;
using GadjIT_ClientContext.Models.Smartflow.Client;

namespace GadjIT_App.Pages.Smartflows.ComponentsSmartflowDetail._DataView
{
    public partial class ModalSmartflowDataViewDetail
    {

        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Parameter]
        public string _Option { get; set; }

        [Parameter]
        public List<P4W_MpSysViews> _ListP4WViews { get; set; }

        [Parameter]
        public Client_SmartflowRecord _Selected_ClientSmartflowRecord { get; set; }

        [Parameter]
        public SmartflowV2 _SelectedSmartflow { get; set; }

        [Parameter]
        public SmartflowDataView _TaskObject { get; set; }

        [Parameter]
        public SmartflowDataView _CopyObject { get; set; }

        [Parameter]
        public Action _DataChanged { get; set; }

        

        [Inject]
        IClientApiManagementService ClientApiManagementService { get; set; }

        [Inject]
        public IPartnerAccessService PartnerAccessService { get; set; }

        [Inject]
        IAppSmartflowsState AppSmartflowsState { get; set; }

        [Inject]
        IUserSessionState UserSession { get; set; }


        public string FilterText { get; set; } = "";

        private async Task Close()
        {
            _CopyObject = new SmartflowDataView();
            await ModalInstance.CloseAsync();
        }

        private void HandleValidSubmit()
        {
            _TaskObject.SeqNo = _CopyObject.SeqNo;
            _TaskObject.DisplayName = _CopyObject.DisplayName;
            _TaskObject.ViewName = _CopyObject.ViewName;

            if (_Option == "Insert")
            {
                if(_SelectedSmartflow.DataViews is null)
                {
                    _SelectedSmartflow.DataViews = new List<SmartflowDataView>();
                }

                _SelectedSmartflow.DataViews.Add(_TaskObject);
            }

            _Selected_ClientSmartflowRecord.SmartflowData = JsonConvert.SerializeObject(_SelectedSmartflow);
            var returnChapterObject =  ClientApiManagementService.Update(_Selected_ClientSmartflowRecord);

            _CopyObject = new SmartflowDataView();
            FilterText = "";

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
