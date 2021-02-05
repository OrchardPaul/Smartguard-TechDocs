using Blazored.Modal;
using GadjIT.ClientContext.P4W;
using GadjIT.ClientContext.P4W.Custom;
using Gizmo_V1_02.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Pages.Chapters
{
    public partial class ChapterCaseTypeEdit
    {
        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Inject]
        IChapterManagementService chapterManagementService { get; set; }

        [Parameter]
        public List<VmUsrOrDefChapterManagement> ListChapters { get; set; }

        [Parameter]
        public string TaskObject { get; set; }

        [Parameter]
        public UsrOrDefChapterManagement Chapter { get; set; }

        [Parameter]
        public string originalName { get; set; }

        [Parameter]
        public string caseTypeGroupName { get; set; }

        [Parameter]
        public string isCaseTypeOrGroup { get; set; }

        [Parameter]
        public Action DataChanged { get; set; }

        private async void Close()
        {
            await ModalInstance.CloseAsync();
        }

        private async void HandleValidSubmit()
        {
            if (isCaseTypeOrGroup == "CaseType")
            {
                var chapters = ListChapters.Where(C => C.ChapterObject.CaseType == originalName).ToList();

                foreach (var chapter in chapters)
                {
                    var updateJson = JsonConvert.DeserializeObject<VmChapter>(chapter.ChapterObject.ChapterData);
                    updateJson.CaseType = TaskObject;
                    chapter.ChapterObject.CaseType = TaskObject;
                    chapter.ChapterObject.ChapterData = JsonConvert.SerializeObject(updateJson);
                    await chapterManagementService.Update(chapter.ChapterObject).ConfigureAwait(false);
                }
            }
            else if (isCaseTypeOrGroup == "CaseTypeGroup")
            {
                var chapters = ListChapters.Where(C => C.ChapterObject.CaseTypeGroup == originalName).ToList();

                foreach (var chapter in chapters)
                {
                    var updateJson = JsonConvert.DeserializeObject<VmChapter>(chapter.ChapterObject.ChapterData);
                    updateJson.CaseTypeGroup = TaskObject;
                    chapter.ChapterObject.CaseTypeGroup = TaskObject;
                    chapter.ChapterObject.ChapterData = JsonConvert.SerializeObject(updateJson);
                    await chapterManagementService.Update(chapter.ChapterObject).ConfigureAwait(false);
                }
            }
            else
            {
                var updateJson = JsonConvert.DeserializeObject<VmChapter>(Chapter.ChapterData);
                updateJson.Name = Chapter.Name;
                Chapter.ChapterData = JsonConvert.SerializeObject(updateJson);
                await chapterManagementService.Update(Chapter).ConfigureAwait(false);
            }

            DataChanged?.Invoke();
            Close();
        }

  

    }
}
