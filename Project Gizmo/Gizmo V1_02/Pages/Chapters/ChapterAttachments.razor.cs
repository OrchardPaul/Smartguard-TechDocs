using Blazored.Modal;
using GadjIT.ClientContext.P4W;
using GadjIT.ClientContext.P4W.Custom;
using Gizmo_V1_02.Services;
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

        public int? RescheduleDays
        {
            get { return Attachment.ScheduleDays; }
            set
            {
                if (value < 0)
                {
                    Attachment.ScheduleDays = 0;
                }
                else
                {
                    Attachment.ScheduleDays = value;
                }
            }
        }


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

        private int selectedCaseTypeGroup { get; set; } = -1;

        List<string> DocTypeList = new List<string>() { "Letter", "Doc", "Email", "Form", "Step" };

        List<string> ActionList = new List<string>() { "Take", "Insert" };

        public List<string> documentList;

        private async void Close()
        {
            TaskObject = new UsrOrDefChapterManagement();
            await ModalInstance.CloseAsync();


        }

        private async void HandleValidSubmit()
        {

            CopyObject.FollowUpDocs = new List<FollowUpDoc> { Attachment };

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
            TaskObject.UserNotes = CopyObject.UserNotes;
            TaskObject.NextStatus = CopyObject.NextStatus;

            TaskObject.FollowUpDocs = CopyObject.FollowUpDocs;

            SelectedChapterObject.ChapterData = JsonConvert.SerializeObject(SelectedChapter);
            await chapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);

            TaskObject = new UsrOrDefChapterManagement();
            filterText = "";

            DataChanged?.Invoke();
            Close();

        }

        private async void RemoveAttachment()
        {

            CopyObject.FollowUpDocs = null; 

            TaskObject.FollowUpDocs = CopyObject.FollowUpDocs;

            SelectedChapterObject.ChapterData = JsonConvert.SerializeObject(SelectedChapter);
            await chapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);

            TaskObject = new UsrOrDefChapterManagement();
            filterText = "";

            DataChanged?.Invoke();
            Close();

        }
    }
}
