using Blazored.Modal;
using GadjIT.ClientContext.P4W;
using GadjIT.ClientContext.P4W.Custom;
using GadjIT_V1_02.Data.Admin;
using GadjIT_V1_02.Services;
using GadjIT_V1_02.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GadjIT_V1_02.Pages.Chapters
{
    public partial class ChapterAddOrEdit
    {
        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Inject]
        IChapterManagementService chapterManagementService { get; set; }

        [Parameter]
        public UsrOrsfSmartflows TaskObject { get; set; }

        [Parameter]
        public Action DataChanged { get; set; }

        [Parameter]
        public List<VmUsrOrsfSmartflows> AllObjects { get; set; }

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
            if (TaskObject.Id == 0)
            {
                var name = Regex.Replace(TaskObject.SmartflowName, "[^0-9a-zA-Z-_ ]+", "");
                var caseType = Regex.Replace(TaskObject.CaseType, "[^0-9a-zA-Z-_ ]+", "");
                var caseTypeGroup = Regex.Replace(TaskObject.CaseTypeGroup, "[^0-9a-zA-Z-_ ]+", "");

                TaskObject.SmartflowName = name;
                TaskObject.CaseType = caseType;
                TaskObject.CaseTypeGroup = caseTypeGroup;
                TaskObject.SmartflowData = JsonConvert.SerializeObject(new VmChapter
                                                                                        {
                                                                                            CaseTypeGroup = caseTypeGroup,
                                                                                            CaseType = caseType,
                                                                                            Name = name,
                                                                                            SeqNo = TaskObject.SeqNo.GetValueOrDefault(),
                                                                                            StepName = "",
                                                                                            ShowPartnerNotes = "N",
                                                                                            Items = new List<GenSmartflowItem>(),
                                                                                            Fees = new List<Fee>(),
                                                                                            DataViews = new List<DataViews>(),
                                                                                            TickerMessages = new List<TickerMessages>()
                                                                                        });


                var returnObject = await chapterManagementService.Add(TaskObject);
                TaskObject.Id = returnObject.Id;
                await CompanyDbAccess.SaveSmartFlowRecord(TaskObject, sessionState);
            }
            else
            {
                await chapterManagementService.UpdateMainItem(TaskObject);
            }

            DataChanged?.Invoke();
            Close();
        }

        private async void HandleValidDelete()
        {
            await chapterManagementService.DeleteChapter(TaskObject.Id);

            DataChanged?.Invoke();
            Close();
        }
    }
}
