using Blazored.Modal;
using GadjIT.ClientContext.P4W;
using GadjIT.ClientContext.P4W.Custom;
using GadjIT_V1_02.Data.Admin;
using GadjIT_V1_02.Services;
using GadjIT_V1_02.Services.AppState;
using GadjIT_V1_02.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GadjIT_V1_02.Pages.Chapters
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
                
                 CopyObject.RescheduleDays = value;
            }
        }

        

        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Inject]
        IChapterManagementService chapterManagementService { get; set; }

        [Inject]
        IAppChapterState appChapterState { get; set; }

        [Inject]
        IUserSessionState sessionState { get; set; }

        [Parameter]
        public RenderFragment CustomHeader { get; set; }

        [Parameter]
        public string selectedList { get; set; }

        public string filterText { get; set; } = "";

        [Parameter]
        public string Option { get; set; }

        [Parameter]
        public UsrOrsfSmartflows SelectedChapterObject { get; set; }

        [Parameter]
        public VmChapter SelectedChapter { get; set; }

        [Parameter]
        public GenSmartflowItem TaskObject { get; set; }

        [Parameter]
        public GenSmartflowItem CopyObject { get; set; }


        [Parameter]
        public Action DataChanged { get; set; }

        [Parameter]
        public Action RefreshDocList { get; set; }

        [Parameter]
        public List<DmDocuments> dropDownChapterList { get; set; }

        [Parameter]
        public List<TableDate> TableDates { get; set; }

        [Parameter]
        public List<CaseTypeGroups> CaseTypeGroups { get; set; }

        [Parameter]
        public List<VmUsrOrDefChapterManagement> ListOfStatus { get; set; }

        private int selectedCaseTypeGroup { get; set; } = -2;

        List<string> Actions = new List<string>() { "TAKE", "INSERT" };
        
        List<string> TrackMethodList = new List<string>() { "N/A", "Send Only", "Response Required" };

        public List<string> documentList;

        
        public bool useCustomItem { get; set; } = false;

        public bool _useCustomReschedule { get; set; }
        public bool useCustomReschedule {
            get { return _useCustomReschedule; }
            set
            {
                CopyObject.RescheduleDataItem = !value ? "" : CopyObject.RescheduleDataItem;
                _useCustomReschedule = value;
            }
        }

        [Required]
        public string customItemName { get; set; } = "";

        protected override void OnInitialized()
        {
            if (!(string.IsNullOrEmpty(SelectedChapter.P4WCaseTypeGroup)) && (SelectedChapter.P4WCaseTypeGroup != "Select"))
            {
                selectedCaseTypeGroup = CaseTypeGroups.Where(CT => CT.Name == SelectedChapter.P4WCaseTypeGroup).Select(CT => CT.Id).FirstOrDefault();

                if (SelectedChapter.P4WCaseTypeGroup == "Global Documents")
                {
                    selectedCaseTypeGroup = 0;
                }

                if (SelectedChapter.P4WCaseTypeGroup == "Entity Documents")
                {
                    selectedCaseTypeGroup = -1;
                }
            }

            if (!(dropDownChapterList.ToList() is null)
                    && CopyObject.Name != ""
                    && !(CopyObject.Name is null)
                    && !dropDownChapterList.ToList().Select(D => D.Name).Contains(CopyObject.Name))
            {
                useCustomItem = true;
                customItemName = CopyObject.Name;
            }
            else
            {
                useCustomItem = false;
            }

            if (!string.IsNullOrEmpty(CopyObject.RescheduleDataItem)
                    && TableDates.ToList().Select(D => D.TableField).Contains(CopyObject.RescheduleDataItem))
            {
                useCustomReschedule = true;
            }
            else
            {
                useCustomReschedule = false;
            }

        }


        private async void RefreshDocListOnModel()
        {
            dropDownChapterList = await chapterManagementService.GetDocumentList(SelectedChapter.CaseType);
            StateHasChanged();
            RefreshDocList?.Invoke();
        }

        private async void Close()
        {
            TaskObject = new GenSmartflowItem();
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
                TaskObject.Name = Regex.Replace(CopyObject.Name, "[^0-9a-zA-Z-_ (){}!£$%^&*,]+", "");
            }

            
            TaskObject.EntityType = CopyObject.EntityType;
            TaskObject.SeqNo = CopyObject.SeqNo;
            TaskObject.SuppressStep = CopyObject.SuppressStep;
            TaskObject.CompleteName = CopyObject.CompleteName is null ? "" : Regex.Replace(CopyObject.CompleteName, "[^0-9a-zA-Z-_ (){}!£$%^&*,]+", "");
            TaskObject.AsName = CopyObject.AsName is null ? "" : Regex.Replace(CopyObject.AsName, "[^0-9a-zA-Z-_ (){}!£$%^&*,]+", "");
            TaskObject.RescheduleDays = CopyObject.RescheduleDays is null ? 0 : CopyObject.RescheduleDays;
            TaskObject.AltDisplayName = CopyObject.AltDisplayName is null ? "" : Regex.Replace(CopyObject.AltDisplayName, "[^0-9a-zA-Z-_ (){}!£$%^&*,]+", "");
            TaskObject.UserMessage = CopyObject.UserMessage;
            TaskObject.PopupAlert = CopyObject.PopupAlert;
            TaskObject.NextStatus = CopyObject.NextStatus;
            TaskObject.Action = CopyObject.Action;
            TaskObject.TrackingMethod = CopyObject.TrackingMethod;
            TaskObject.ChaserDesc = CopyObject.ChaserDesc;
            TaskObject.RescheduleDataItem = CopyObject.RescheduleDataItem;

            if (Option == "Insert")
            {
                SelectedChapter.Items.Add(TaskObject);
            }
            
            SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(SelectedChapter);
            await chapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);

            TaskObject = new GenSmartflowItem();
            filterText = "";

            //keep track of time last updated ready for comparison by other sessions checking for updates
            appChapterState.SetLastUpdated(sessionState, SelectedChapter);

            DataChanged?.Invoke();
            Close();

        }


    }
}
