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
using System.Threading.Tasks;

namespace Gizmo_V1_02.Pages.Chapters
{
    public partial class ChapterAttachments
    {
        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Inject]
        IChapterManagementService chapterManagementService { get; set; }

        [Parameter]
        public FollowUpDoc Attachment { get; set; }

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

        public bool useCustomItem { get; set; } = false;


        List<string> ActionList = new List<string>() { "TAKE", "INSERT" };

        public List<string> documentList;


        protected override void OnInitialized()
        {
            if (!(string.IsNullOrEmpty(SelectedChapter.P4WCaseTypeGroup)) && (SelectedChapter.P4WCaseTypeGroup != "Select"))
            {
                selectedCaseTypeGroup = CaseTypeGroups.Where(CT => CT.Name == SelectedChapter.P4WCaseTypeGroup).Select(CT => CT.Id).FirstOrDefault();

                if (SelectedChapter.P4WCaseTypeGroup == "Entity Documents")
                {
                    selectedCaseTypeGroup = -1;
                }
            }
        }

        private async void Close()
        {
            TaskObject = new GenSmartflowItem();
            await ModalInstance.CloseAsync();


        }

        private async void HandleValidSubmit()
        {
            if(CopyObject.FollowUpDocs is null)
            {
                CopyObject.FollowUpDocs = new List<FollowUpDoc> { Attachment };
            }
            else
            {
                if (CopyObject.FollowUpDocs.Select(F => F.DocName).ToList().Contains(Attachment.DocName))
                {
                    var updateItem = CopyObject.FollowUpDocs.Where(F => F.DocName == Attachment.DocName).FirstOrDefault();

                    updateItem.DocAsName = Attachment.DocAsName;
                    updateItem.Action = Attachment.Action;
                }
                else
                {
                    CopyObject.FollowUpDocs.Add(Attachment);
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

            TaskObject.FollowUpDocs = CopyObject.FollowUpDocs;

            SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(SelectedChapter);
            await chapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);

            TaskObject = new GenSmartflowItem();
            filterText = "";

            DataChanged?.Invoke();
            Close();

        }

        private async void RemoveAttachment()
        {
            if (!(CopyObject.FollowUpDocs is null))
            {
                if (CopyObject.FollowUpDocs.Select(F => F.DocName).ToList().Contains(Attachment.DocName))
                {
                    var updateItem = CopyObject.FollowUpDocs.Where(F => F.DocName == Attachment.DocName).FirstOrDefault();

                    CopyObject.FollowUpDocs.Remove(updateItem);
                }
            }


            TaskObject.FollowUpDocs = CopyObject.FollowUpDocs;

            SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(SelectedChapter);
            await chapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);

            await CompanyDbAccess.SaveSmartFlowRecord(SelectedChapterObject, sessionState);

            TaskObject = new GenSmartflowItem();
            filterText = "";

            DataChanged?.Invoke();
            Close();

        }
    }
}
