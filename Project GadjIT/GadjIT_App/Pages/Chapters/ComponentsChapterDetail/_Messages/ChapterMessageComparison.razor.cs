using Blazored.Modal;
using GadjIT_ClientContext.P4W;
using GadjIT_ClientContext.P4W.Custom;
using GadjIT_App.Data.Admin;
using GadjIT_App.Services;
using GadjIT_App.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GadjIT_App.Pages.Chapters.ComponentsChapterDetail._Messages
{
    public partial class ChapterMessageComparison
    {
        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Inject]
        IChapterManagementService ChapterManagementService { get; set; }

        [Inject]
        public IUserSessionState UserSession { get; set; }

        [Parameter]
        public Action _ComparisonRefresh { get; set; }

        [Parameter]
        public VmTickerMessage _Object { get; set; }

        [Parameter]
        public UsrOrsfSmartflows _AltChapterRow { get; set; }

        [Parameter]
        public VmChapter _AltChapter { get; set; }

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
            
            var taskObject = _AltChapter.TickerMessages.Where(C => C.Message == _Object.Message.Message).SingleOrDefault();

            _AltChapter.TickerMessages.Remove(taskObject);
            _AltChapter.TickerMessages.Add(_Object.Message);

            bool gotLock = UserSession.Lock;
            while (gotLock)
            {
                await Task.Yield();
                gotLock = UserSession.Lock;
            }

            await UserSession.SwitchSelectedSystem();
            _AltChapterRow.SmartflowData = JsonConvert.SerializeObject(_AltChapter);
            await ChapterManagementService.Update(_AltChapterRow);
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
            _AltChapter.TickerMessages.Add(_Object.Message);

            bool gotLock = UserSession.Lock;
            while (gotLock)
            {
                await Task.Yield();
                gotLock = UserSession.Lock;
            }

            await UserSession.SwitchSelectedSystem();
            _AltChapterRow.SmartflowData = JsonConvert.SerializeObject(_AltChapter);
            await ChapterManagementService.Update(_AltChapterRow);
            await UserSession.ResetSelectedSystem();
            

            _ComparisonRefresh?.Invoke();
            await ModalInstance.CloseAsync();
        }

        
    }
}
