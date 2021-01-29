using Blazored.Modal;
using GadjIT.ClientContext.OR_RESI;
using Gizmo_V1_02.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Pages.OR_RESI_Chapters
{
    public partial class ChapterDetail : ComponentBase
    {
        public bool suppressStep
        {
            get { return (TaskObject.SuppressStep == "Y" ? true : false); }
            set
            {
                if (value)
                {
                    TaskObject.SuppressStep = "Y";
                }
                else
                {
                    TaskObject.SuppressStep = "N";
                }
            }
        }

        public int? RescheduleDays
        {
            get { return TaskObject.RescheduleDays; }
            set
            {
                if (value < 0)
                {
                    TaskObject.RescheduleDays = 0;
                }
                else
                {
                    TaskObject.RescheduleDays = value;
                }
            }
        }


        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Inject]
        IChapterManagementService chapterManagementService { get; set; }

        [Parameter]
        public UsrOrDefChapterManagement TaskObject { get; set; }

        [Parameter]
        public RenderFragment CustomHeader { get; set; }

        [Parameter]
        public string selectedList { get; set; }

        [Parameter]
        public Action DataChanged { get; set; }

        [Parameter]
        public string selectedCaseType { get; set; }

        [Parameter]
        public List<DmDocuments> dropDownChapterList { get; set; }

        [Parameter]
        public List<CaseTypeGroups> CaseTypeGroups { get; set; }

        private int selectedCaseTypeGroup { get; set; } = -1;

        List<string> DocTypeList = new List<string>() { "Letter", "Doc", "Email", "Form", "Step" };

        public List<string> documentList;

        private async void Close()
        {
            TaskObject = new UsrOrDefChapterManagement();
            await ModalInstance.CloseAsync();
        }

        private async void HandleValidSubmit()
        {
            if (TaskObject.Id == 0)
            {
                await chapterManagementService.Add(TaskObject);
            }
            else
            {
                await chapterManagementService.Update(TaskObject);
            }

            TaskObject = new UsrOrDefChapterManagement();

            DataChanged?.Invoke();
            Close();

        }

        private void SuppressChange(object value)
        {
            if ((bool)value)
            {
                TaskObject.SuppressStep = "Y";
            }
            else
            {
                TaskObject.SuppressStep = "N";
            }
        }

    }
}
