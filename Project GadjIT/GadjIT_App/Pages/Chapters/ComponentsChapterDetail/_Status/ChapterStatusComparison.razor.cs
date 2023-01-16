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

namespace GadjIT_App.Pages.Chapters.ComponentsChapterDetail._Status
{
    public partial class ChapterStatusComparison
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
        public VmGenSmartflowItem _Object { get; set; }

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
            
            var taskObject = _AltChapter.Items
                                        .Where(C => C.Type == _Object.AltObject.Type)
                                        .Where(C => C.Name == _Object.AltObject.Name)
                                        .FirstOrDefault();


            _AltChapter.Items.Remove(taskObject);
            _AltChapter.Items.Add(_Object.ChapterObject);


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
            _AltChapter.Items.Add(_Object.ChapterObject);

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
