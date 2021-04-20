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
    public partial class ChapterDetail : ComponentBase
    {
        public bool suppressStep
        {
            get { return (CopyObject.SuppressStep == "Y" ? true : false); }
            set
            {
                if (value)
                {
                    CopyObject.SuppressStep = "Y";
                }
                else
                {
                    CopyObject.SuppressStep = "N";
                }
            }
        }

        public int? RescheduleDays
        {
            get { return CopyObject.RescheduleDays; }
            set
            {
                if (value < 0)
                {
                    CopyObject.RescheduleDays = 0;
                }
                else
                {
                    CopyObject.RescheduleDays = value;
                }
            }
        }


        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Inject]
        IChapterManagementService chapterManagementService { get; set; }

        [Parameter]
        public RenderFragment CustomHeader { get; set; }

        [Parameter]
        public string selectedList { get; set; }

        public string filterText { get; set; } = "";

        [Parameter]
        public string Option { get; set; }

        [Parameter]
        public UsrOrDefChapterManagement SelectedChapterObject { get; set; }

        [Parameter]
        public VmChapter SelectedChapter { get; set; }

        [Parameter]
        public UsrOrDefChapterManagement TaskObject { get; set; }

        [Parameter]
        public UsrOrDefChapterManagement CopyObject { get; set; }


        [Parameter]
        public Action DataChanged { get; set; }

        [Parameter]
        public List<DmDocuments> dropDownChapterList { get; set; }

        [Parameter]
        public List<CaseTypeGroups> CaseTypeGroups { get; set; }

        [Parameter]
        public List<VmUsrOrDefChapterManagement> ListOfStatus { get; set; }

        [Parameter]
        public ICompanyDbAccess CompanyDbAccess { get; set; }

        [Parameter]
        public IUserSessionState sessionState { get; set; }

        private int selectedCaseTypeGroup { get; set; } = -1;

        List<string> Actions = new List<string>() { "TAKE", "INSERT" };

        public List<string> documentList;

        protected override void OnInitialized()
        {
            if (!(string.IsNullOrEmpty(SelectedChapter.P4WCaseTypeGroup)) && (SelectedChapter.P4WCaseTypeGroup != "Select"))
            {
                selectedCaseTypeGroup = CaseTypeGroups.Where(CT => CT.Name == SelectedChapter.P4WCaseTypeGroup).Select(CT => CT.Id).FirstOrDefault();
            }
        }

        private async void Close()
        {
            TaskObject = new UsrOrDefChapterManagement();
            await ModalInstance.CloseAsync();


        }

        private async void HandleValidSubmit()
        {
            if(!(new string[] { "Agenda", "Status" }.Any(s => TaskObject.Type.ToString().Contains(s))))
            {
                //clears lagacy value of "Letter" and revert it back to "Doc"
                TaskObject.Type = "Doc";
            }
            else
            {
                
                TaskObject.Type = CopyObject.Type;
            }

            if (TaskObject.Type == "Agenda")
            {
                TaskObject.Name = CopyObject.Name;
            }
            else
            {
                TaskObject.Name = Regex.Replace(CopyObject.Name, "[^0-9a-zA-Z-_]+", "");
            }

            
            TaskObject.EntityType = CopyObject.EntityType;
            TaskObject.SeqNo = CopyObject.SeqNo;
            TaskObject.SuppressStep = CopyObject.SuppressStep;
            TaskObject.CompleteName = CopyObject.CompleteName;
            TaskObject.AsName = Regex.Replace(CopyObject.AsName, "[^0-9a-zA-Z-_]+", "");
            TaskObject.RescheduleDays = CopyObject.RescheduleDays;
            TaskObject.AltDisplayName = Regex.Replace(CopyObject.AltDisplayName, "[^0-9a-zA-Z-_]+", "");
            TaskObject.UserMessage = CopyObject.UserMessage;
            TaskObject.PopupAlert = CopyObject.PopupAlert;
            TaskObject.NextStatus = CopyObject.NextStatus;
            TaskObject.Action = CopyObject.Action;

            if (Option == "Insert")
            {
                SelectedChapter.ChapterItems.Add(TaskObject);
            }
            
            SelectedChapterObject.ChapterData = JsonConvert.SerializeObject(SelectedChapter);
            await chapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);

            await CompanyDbAccess.SaveSmartFlowRecord(SelectedChapterObject, sessionState);

            TaskObject = new UsrOrDefChapterManagement();
            filterText = "";

            DataChanged?.Invoke();
            Close();

        }


    }
}
