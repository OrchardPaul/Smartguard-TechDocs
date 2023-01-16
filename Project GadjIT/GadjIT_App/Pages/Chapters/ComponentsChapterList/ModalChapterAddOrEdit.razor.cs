using Blazored.Modal;
using GadjIT_ClientContext.P4W;
using GadjIT_ClientContext.P4W.Custom;
using GadjIT_App.Data.Admin;
using GadjIT_App.Services;
using GadjIT_App.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GadjIT_App.Pages.Chapters.ComponentsChapterList
{
    public partial class ModalChapterAddOrEdit
    {
        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Inject]
        IChapterManagementService ChapterManagementService { get; set; }

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
        public IUserSessionState UserSession { get; set; }

        [Parameter]
        public ICompanyDbAccess CompanyDbAccess { get; set; }


        public int Error { get; set; } = 0;


        private async void Close()
        {
            await ModalInstance.CloseAsync();


        }

        private async void CreateSmartFlow()
        {
            if (TaskObject.Id == 0)
            {

                var name = Regex.Replace(TaskObject.SmartflowName, "[^0-9a-zA-Z-_ (){}!£$%^&*,.#?@<>`: ]+", "");
                var caseType = Regex.Replace(TaskObject.CaseType, "[^0-9a-zA-Z-_ (){}!£$%^&*,.#?@<>`: ]+", "");
                var caseTypeGroup = Regex.Replace(TaskObject.CaseTypeGroup, "[^0-9a-zA-Z-_ (){}!£$%^&*,.#?@<>`: ]+", "");

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
                    DataViews = new List<DataView>(),
                    TickerMessages = new List<TickerMessage>()
                });


                var returnObject = await ChapterManagementService.Add(TaskObject);
                TaskObject.Id = returnObject.Id;
                await CompanyDbAccess.SaveSmartFlowRecord(TaskObject, UserSession);
            }
            else
            {
                await ChapterManagementService.UpdateMainItem(TaskObject);
            }

            DataChanged?.Invoke();
            Close();
        }


        private void HandleValidSubmit()
        {
            if (AllObjects
                .Where(A => A.SmartflowObject.CaseTypeGroup == TaskObject.CaseTypeGroup)
                .Select(A => A.SmartflowObject.SmartflowName)
                .Contains(TaskObject.SmartflowName))
            {
                Error = 1; //Smartflow with same name already exists
                StateHasChanged();
            }
            else
            {

                CreateSmartFlow();
            }

        }

        private async void HandleValidDelete()
        {
            await ChapterManagementService.DeleteChapter(TaskObject.Id);

            DataChanged?.Invoke();
            Close();
        }

        private void ResetError()
        {
            Error = 0;
            StateHasChanged();
        }
    }
}
