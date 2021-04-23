using Blazored.Modal;
using GadjIT.ClientContext.P4W;
using GadjIT.ClientContext.P4W.Custom;
using Gizmo_V1_02.Data.Admin;
using Gizmo_V1_02.Services;
using Gizmo_V1_02.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

        [Parameter]
        public IUserSessionState sessionState { get; set; }

        [Parameter]
        public ICompanyDbAccess CompanyDbAccess { get; set; }


        private async void Close()
        {
            await ModalInstance.CloseAsync();


        }

        private async void HandleValidSubmit()
        {
            if (TaskObject.ChapterObject.Id == 0)
            {
                var name = Regex.Replace(TaskObject.ChapterObject.Name, "[^0-9a-zA-Z-_ ]+", "");
                var caseType = Regex.Replace(TaskObject.ChapterObject.CaseType, "[^0-9a-zA-Z-_ ]+", "");
                var caseTypeGroup = Regex.Replace(TaskObject.ChapterObject.CaseTypeGroup, "[^0-9a-zA-Z-_ ]+", "");

                TaskObject.ChapterObject.Name = name;
                TaskObject.ChapterObject.CaseType = caseType;
                TaskObject.ChapterObject.CaseTypeGroup = caseTypeGroup;
                TaskObject.ChapterObject.ChapterData = JsonConvert.SerializeObject(new VmChapter
                                                                                        {
                                                                                            CaseTypeGroup = caseTypeGroup,
                                                                                            CaseType = caseType,
                                                                                            Name = name,
                                                                                            SeqNo = TaskObject.ChapterObject.SeqNo.GetValueOrDefault(),
                                                                                            StepName = "",
                                                                                            ShowPartnerNotes = "N",
                                                                                            ChapterItems = new List<UsrOrDefChapterManagement>(),
                                                                                            Fees = new List<Fee>(),
                                                                                            DataViews = new List<DataViews>(),
                                                                                            TickerMessages = new List<TickerMessages>()
                                                                                        });


                var returnObject = await chapterManagementService.Add(TaskObject.ChapterObject);
                TaskObject.ChapterObject.Id = returnObject.Id;
                await CompanyDbAccess.SaveSmartFlowRecord(TaskObject.ChapterObject, sessionState);
            }
            else
            {
                await chapterManagementService.UpdateMainItem(TaskObject.ChapterObject);
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
