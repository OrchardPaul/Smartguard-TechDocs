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

namespace GadjIT_App.Pages.Chapters.ComponentsChapterList
{
    public partial class ModalChapterCaseTypeEdit
    {
        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Inject]
        IChapterManagementService chapterManagementService { get; set; }

        [Parameter]
        public List<VmUsrOrsfSmartflows> ListChapters { get; set; }

        [Parameter]
        public string TaskObject { get; set; }

        [Parameter]
        public UsrOrsfSmartflows Chapter { get; set; }

        [Parameter]
        public string originalName { get; set; }

        [Parameter]
        public string caseTypeGroupName { get; set; }

        [Parameter]
        public string isCaseTypeOrGroup { get; set; }

        [Parameter]
        public Action DataChanged { get; set; }

        [Parameter]
        public ICompanyDbAccess CompanyDbAccess { get; set; }

        [Parameter]
        public IUserSessionState UserSession { get; set; }


        private async void Close()
        {
            await ModalInstance.CloseAsync();
        }

        private async Task HandleValidSubmit()
        {
            if (isCaseTypeOrGroup == "CaseType")
            {
                var chapters = ListChapters.Where(C => C.SmartflowObject.CaseType == originalName).ToList();

                foreach (var chapter in chapters)
                {
                    var updateJson = JsonConvert.DeserializeObject<VmChapter>(chapter.SmartflowObject.SmartflowData);
                    updateJson.CaseType = TaskObject;
                    chapter.SmartflowObject.CaseType = TaskObject;
                    chapter.SmartflowObject.SmartflowData = JsonConvert.SerializeObject(updateJson);
                    await chapterManagementService.UpdateMainItem(chapter.SmartflowObject).ConfigureAwait(false);
                }
            }
            else if (isCaseTypeOrGroup == "CaseTypeGroup")
            {
                var chapters = ListChapters.Where(C => C.SmartflowObject.CaseTypeGroup == originalName).ToList();

                foreach (var chapter in chapters)
                {
                    var updateJson = JsonConvert.DeserializeObject<VmChapter>(chapter.SmartflowObject.SmartflowData);
                    updateJson.CaseTypeGroup = TaskObject;
                    chapter.SmartflowObject.CaseTypeGroup = TaskObject;
                    chapter.SmartflowObject.SmartflowData = JsonConvert.SerializeObject(updateJson);
                    await chapterManagementService.UpdateMainItem(chapter.SmartflowObject).ConfigureAwait(false);
                }
            }
            else
            {
                var updateJson = JsonConvert.DeserializeObject<VmChapter>(Chapter.SmartflowData);
                updateJson.Name = Chapter.SmartflowName;
                Chapter.SmartflowData = JsonConvert.SerializeObject(updateJson);
                await chapterManagementService.UpdateMainItem(Chapter).ConfigureAwait(false);
            }

            DataChanged?.Invoke();
            Close();
        }

  

    }
}
