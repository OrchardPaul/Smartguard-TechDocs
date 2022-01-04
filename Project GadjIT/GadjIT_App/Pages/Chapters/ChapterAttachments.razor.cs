using Blazored.Modal;
using GadjIT.ClientContext.P4W;
using GadjIT.ClientContext.P4W.Custom;
using GadjIT_App.Data.Admin;
using GadjIT_App.Services;
using GadjIT_App.Services.AppState;
using GadjIT_App.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GadjIT_App.Pages.Chapters
{
    public partial class ChapterAttachments
    {
        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Inject]
        IChapterManagementService chapterManagementService { get; set; }

        [Parameter]
        public LinkedItems Attachment { get; set; }

        [Parameter]
        public RenderFragment CustomHeader { get; set; }

        [Parameter]
        public string selectedList { get; set; }

        [Parameter]
        public List<TableDate> TableDates { get; set; }

        [Inject]
        IAppChapterState appChapterState { get; set; }

        public string filterText { get; set; } = "";
        
        public string filterTextDataItem { get; set; } = "";

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
        public List<DmDocuments> dropDownChapterList { get; set; }

        [Parameter]
        public List<CaseTypeGroups> CaseTypeGroups { get; set; }

        [Parameter]
        public List<VmUsrOrDefChapterManagement> ListOfStatus { get; set; }

        [Parameter]
        public Action RefreshDocList { get; set; }


        [Parameter]
        public ICompanyDbAccess CompanyDbAccess { get; set; }

        [Parameter]
        public IUserSessionState UserSession { get; set; }

        private int selectedCaseTypeGroup { get; set; } = -1;

        public bool useCustomItem { get; set; } = false;


        List<string> ActionList = new List<string>() { "TAKE", "INSERT", "SCHEDULE" };
        
        List<string> TrackMethodList = new List<string>() { "N/A", "Send Only", "Response Required" };

        public List<string> documentList;

        public bool _useCustomReschedule { get; set; }
        public bool useCustomReschedule
        {
            get { return _useCustomReschedule; }
            set
            {
                Attachment.ScheduleDataItem = !value ? "" : Attachment.ScheduleDataItem;
                _useCustomReschedule = value;
            }
        }



        protected override void OnInitialized()
        {
            if (!(string.IsNullOrEmpty(SelectedChapter.P4WCaseTypeGroup)) && (SelectedChapter.P4WCaseTypeGroup != "Select"))
            {
                selectedCaseTypeGroup = CaseTypeGroups.Where(CT => CT.Name == SelectedChapter.P4WCaseTypeGroup).Select(CT => CT.Id).FirstOrDefault();


                if (SelectedChapter.P4WCaseTypeGroup == "Entity Documents")
                {
                    selectedCaseTypeGroup = -1;
                }

                if (SelectedChapter.P4WCaseTypeGroup == "Global Documents")
                {
                    selectedCaseTypeGroup = 0;
                }
            }

            if (!(dropDownChapterList.ToList() is null)
                    && Attachment.DocName != ""
                    && !(Attachment.DocName is null)
                    && !dropDownChapterList.ToList().Select(D => D.Name).Contains(Attachment.DocName))
            {
                useCustomItem = true;
            }
            else
            {
                useCustomItem = false;
            }

            if (!string.IsNullOrEmpty(Attachment.ScheduleDataItem)
                && TableDates.ToList().Select(D => D.TableField).Contains(Attachment.ScheduleDataItem))
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
            Dictionary<int?, string> docTypes = new Dictionary<int?, string> { { 1, "Doc" }, { 4, "Form" }, { 6, "Step" }, { 8, "Date" }, { 9, "Email" }, { 11, "Doc" }, { 12, "Email" } };

            Attachment.DocType = dropDownChapterList.Where(D => D.Name.ToUpper() == Attachment.DocName.ToUpper())
                                                                                        .Select(D => string.IsNullOrEmpty(docTypes[D.DocumentType]) ? "Doc" : docTypes[D.DocumentType])
                                                                                        .FirstOrDefault();

            if (CopyObject.LinkedItems is null)
            {
                CopyObject.LinkedItems = new List<LinkedItems> { Attachment };
            }
            else
            {
                if (selectedList != "Edit Attachement")
                {
                    CopyObject.LinkedItems.Add(Attachment);
                }
            }


            

            TaskObject.Type = CopyObject.Type;
            TaskObject.Name = CopyObject.Name;
            TaskObject.EntityType = CopyObject.EntityType;
            TaskObject.SeqNo = CopyObject.SeqNo;
            TaskObject.SuppressStep = CopyObject.SuppressStep;
            TaskObject.CompleteName = CopyObject.CompleteName;
            TaskObject.AsName = CopyObject.AsName;
            TaskObject.RescheduleDays = CopyObject.RescheduleDays;
            TaskObject.AltDisplayName = CopyObject.AltDisplayName;
            TaskObject.UserMessage = CopyObject.UserMessage;
            TaskObject.PopupAlert = CopyObject.PopupAlert;
            TaskObject.NextStatus = CopyObject.NextStatus;

            TaskObject.LinkedItems = CopyObject.LinkedItems;

            SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(SelectedChapter);
            await chapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);

            TaskObject = new GenSmartflowItem();
            filterText = "";


            //keep track of time last updated ready for comparison by other sessions checking for updates
            appChapterState.SetLastUpdated(UserSession, SelectedChapter);

            DataChanged?.Invoke();
            Close();

        }

        private async void RemoveAttachment()
        {
            if (!(CopyObject.LinkedItems is null))
            {
                if (CopyObject.LinkedItems.Select(F => F.DocName).ToList().Contains(Attachment.DocName))
                {
                    var updateItem = CopyObject.LinkedItems.Where(F => F.DocName == Attachment.DocName).FirstOrDefault();

                    CopyObject.LinkedItems.Remove(updateItem);
                }
            }


            TaskObject.LinkedItems = CopyObject.LinkedItems;

            SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(SelectedChapter);
            await chapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);

            await CompanyDbAccess.SaveSmartFlowRecord(SelectedChapterObject, UserSession);

            TaskObject = new GenSmartflowItem();
            filterText = "";

            //keep track of time last updated ready for comparison by other sessions checking for updates
            appChapterState.SetLastUpdated(UserSession, SelectedChapter);

            DataChanged?.Invoke();
            Close();

        }
    }
}
