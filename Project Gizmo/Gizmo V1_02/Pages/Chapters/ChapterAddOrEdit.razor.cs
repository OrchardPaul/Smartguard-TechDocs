using Blazored.Modal;
using GadjIT.ClientContext.P4W;
using GadjIT.ClientContext.P4W.Custom;
using Gizmo_V1_02.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Pages.Chapters
{
    public partial class ChapterAddOrEdit
    {
        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Inject]
        IChapterManagementService chapterManagementService { get; set; }

        [Parameter]
        public VmUsrOrDefChapterManagement TaskObject { get; set; }

        [Parameter]
        public Action DataChanged { get; set; }

        [Parameter]
        public List<VmUsrOrDefChapterManagement> AllObjects { get; set; }

        [Parameter]
        public bool addNewCaseTypeGroupOption { get; set; } = false;

        [Parameter]
        public bool addNewCaseTypeOption { get; set; } = false;

        private async void Close()
        {
            await ModalInstance.CloseAsync();


        }

        private void ToggleNewCaseTypeGroupOption()
        {
            addNewCaseTypeGroupOption = !addNewCaseTypeGroupOption;
            addNewCaseTypeOption = (addNewCaseTypeGroupOption) ? true : false;
        }

        private void ToggleNewCaseTypeOption()
        {
            addNewCaseTypeOption = !addNewCaseTypeOption;
        }

        private async void HandleValidSubmit()
        {
            if (TaskObject.ChapterObject.Id == 0)
            {
                await chapterManagementService.Add(TaskObject.ChapterObject);
            }
            else
            {
                await chapterManagementService.Update(TaskObject.ChapterObject);
            }
            DataChanged?.Invoke();
            Close();
        }

        private async void HandleValidDelete()
        {
            await chapterManagementService.DeleteChapter(TaskObject.ChapterObject.Id);

            DataChanged?.Invoke();
            Close();
        }
    }
}
