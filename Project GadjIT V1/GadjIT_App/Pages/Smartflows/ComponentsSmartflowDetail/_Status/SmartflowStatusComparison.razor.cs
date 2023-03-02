using Blazored.Modal;
using GadjIT_App.Services;
using GadjIT_App.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GadjIT_ClientContext.Models.Smartflow;
using GadjIT_ClientContext.Models.Smartflow.Client;

namespace GadjIT_App.Pages.Smartflows.ComponentsSmartflowDetail._Status
{
    public partial class SmartflowStatusComparison
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
        public VmGenSmartflowItem _Object { get; set; }

        [Parameter]
        public Client_SmartflowRecord _Alt_ClientSmartflowRecord { get; set; }

        [Parameter]
        public Smartflow _Alt_Smartflow { get; set; }
       

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
            
            var taskObject = _Alt_Smartflow.Items
                                        .Where(C => C.Type == _Object.AltObject.Type)
                                        .Where(C => C.Name == _Object.AltObject.Name)
                                        .FirstOrDefault();


            _Alt_Smartflow.Items.Remove(taskObject);
            _Alt_Smartflow.Items.Add(_Object.ChapterObject);


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
            _Alt_Smartflow.Items.Add(_Object.ChapterObject);

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
