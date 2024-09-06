using Blazored.Modal;
using GadjIT_App.Services;
using GadjIT_App.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using GadjIT_ClientContext.Models.Smartflow;
using GadjIT_ClientContext.Models.Smartflow.Client;

namespace GadjIT_App.Pages.Smartflows.ComponentsSmartflowDetail._DataView
{
    public partial class SmartflowDataViewComparison
    {
        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Inject]
        IClientApiManagementService ClientApiManagementService { get; set; }

        [Inject]
        public IUserSessionState UserSession { get; set; }


        [Parameter]
        public Action _ComparisonRefresh { get; set; }

        [Parameter]
        public VmSmartflowDataView _Object { get; set; }

        [Parameter]
        public Client_SmartflowRecord _Alt_ClientSmartflowRecord { get; set; }

        [Parameter]
        public SmartflowV2 _Alt_Smartflow { get; set; }

        private async void Close()
        {
            await ModalInstance.CloseAsync();
        }

        private async void HandleSyncItem()
        {
            await SyncItem();
        }

        private async Task SyncItem()
        {
            
            var taskObject = _Alt_Smartflow.DataViews.Where(C => C.DisplayNameOrViewName == _Object.DataView.DisplayNameOrViewName).SingleOrDefault();

            _Alt_Smartflow.DataViews.Remove(taskObject);
            _Alt_Smartflow.DataViews.Add(_Object.DataView);

            bool gotLock = UserSession.Lock;
            while (gotLock)
            {
                await Task.Yield();
                gotLock = UserSession.Lock;
            }

            await UserSession.SwitchSelectedSystem();
            _Alt_ClientSmartflowRecord.SmartflowData = JsonConvert.SerializeObject(_Alt_Smartflow);
            await ClientApiManagementService.Update(_Alt_ClientSmartflowRecord);
            await UserSession.ResetSelectedSystem();
            

            _ComparisonRefresh?.Invoke();
            await ModalInstance.CloseAsync();

        }


        private async void HandleAddItem()
        {
            await AddItem();
        }


        private async Task AddItem()
        {
            _Alt_Smartflow.DataViews.Add(_Object.DataView);

            bool gotLock = UserSession.Lock;
            while (gotLock)
            {
                await Task.Yield();
                gotLock = UserSession.Lock;
            }

            await UserSession.SwitchSelectedSystem();
            _Alt_ClientSmartflowRecord.SmartflowData = JsonConvert.SerializeObject(_Alt_Smartflow);
            await ClientApiManagementService.Update(_Alt_ClientSmartflowRecord);
            await UserSession.ResetSelectedSystem();
            

            _ComparisonRefresh?.Invoke();
            await ModalInstance.CloseAsync();
        }

        
    }
}
