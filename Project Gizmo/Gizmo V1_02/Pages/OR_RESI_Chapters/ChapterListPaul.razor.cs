using Gizmo.Context.OR_RESI;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gizmo_V1_02.Services;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net;
using System.Web;
using Gizmo_V1_02.Services.SessionState;

namespace Gizmo_V1_02.Pages.OR_RESI_Chapters
{
    public partial class ChapterListPaul
    {

        private class StatusObject
        {
            public UsrOrDefChapterManagement management { get; set; }
            public bool hoveredOver { get; set; }
        }

        [Inject]
        private IChapterManagementService chapterManagementService { get; set; }

        [Inject]
        private IPageAuthorisationState pageAuthorisationState { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState> authenticationStateTask { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public IUserSessionState sessionState { get; set; }

        private List<UsrOrDefChapterManagement> lstAll;
        private List<UsrOrDefChapterManagement> lstChapters;
        private List<UsrOrDefChapterManagement> lstAgendas;
        private List<UsrOrDefChapterManagement> lstFees;
        private List<UsrOrDefChapterManagement> lstDocs;
        private List<UsrOrDefChapterManagement> lstStatus;

        private List<StatusObject> statusObjects;


        public List<DmDocuments> dropDownDocumentList;

        int parentId;

        UsrOrDefChapterManagement editObject = new UsrOrDefChapterManagement();

        string customHeader = string.Empty;
        string selectedList = string.Empty;

        string displaySection { get; set; } = "";
        string selectedCaseType { get; set; } = "";
        string selectedCaseTypeGroup { get; set; } = "";
        string selectedChapter { get; set; } = "";
        private int selectedChapterId { get; set; } = -1;

        protected List<string> CaseTypeList;

        private string dragState = "";

        protected override async Task OnInitializedAsync()
        {
            var authenticationState = await authenticationStateTask;

            if (!pageAuthorisationState.ChapterListAuthorisation(authenticationState))
            {
                string returnUrl = HttpUtility.UrlEncode($"/chapterlist");
                NavigationManager.NavigateTo($"Identity/Account/Login?returnUrl={returnUrl}", true);
            }

            bool gotLock = sessionState.Lock;
            while (gotLock)
            {
                await Task.Yield();
                gotLock = sessionState.Lock;
            }


            if (!(authenticationState.User.Identity.Name is null))
            {
                try
                {
                    CaseTypeList = await chapterManagementService.GetCaseTypes();
                }
                catch (Exception)
                {
                    NavigationManager.NavigateTo($"/", true);
                }
            }
        }

        public void DirectToLogin()
        {
            string returnUrl = HttpUtility.UrlEncode($"/");
            NavigationManager.NavigateTo($"Identity/Account/Login?returnUrl={returnUrl}", true);
        }

        void SelectHome()
        {
            selectedCaseType = "";
            selectedChapter = "";
        }

        async Task SelectCaseType(string caseType)
        {
            lstChapters = await chapterManagementService.GetChapterListByCaseType(caseType);

            selectedCaseType = caseType;
            selectedChapter = "";
        }

        private void ToggleStatusHover(StatusObject statusObject)
        {
            statusObject.hoveredOver = !statusObject.hoveredOver;
        }

        async Task SelectDocList(String chapter, int chapterID)
        {
            lstAll = await chapterManagementService.GetItemListByChapter(chapterID);

            lstAgendas = lstAll.Where(A => A.Type == "Agenda").ToList();
            lstFees = lstAll.Where(A => A.Type == "Fee").ToList();
            lstStatus = lstAll.Where(A => A.Type == "Status").ToList();

            statusObjects = lstAll.Where(A => A.Type == "Status")
                .Select(A => new StatusObject
                {
                    management = A,
                    hoveredOver = false
                })
                .ToList();

            lstDocs = lstAll
                .Where(A => A.Type != "Agenda")
                .Where(A => A.Type != "Fee")
                .Where(A => A.Type != "Status")
                .ToList();

            /*
            lstAgendas = await chapterManagementService.GetDocListByChapterAndDocType(selectedCaseType, chapter, "Agenda");
            lstFees = await chapterManagementService.GetDocListByChapterAndDocType(selectedCaseType, chapter, "Fee");
            lstDocs = await chapterManagementService.GetDocListByChapter(selectedCaseType, selectedChapter);
            lstStatus = await chapterManagementService.GetDocListByChapterAndDocType(selectedCaseType, chapter, "Status");
            */

            selectedChapterId = chapterID;
            selectedChapter = chapter;
        }

        private async void DataChanged()
        {
            lstAll = await chapterManagementService.GetItemListByChapter(selectedChapterId);

            lstAgendas = lstAll.Where(A => A.Type == "Agenda").ToList();
            lstFees = lstAll.Where(A => A.Type == "Fee").ToList();
            lstStatus = lstAll.Where(A => A.Type == "Status").ToList();

            statusObjects = lstAll.Where(A => A.Type == "Status")
            .Select(A => new StatusObject
            {
                management = A,
                hoveredOver = false
            })
            .ToList();

            lstDocs = lstAll
                .Where(A => A.Type != "Agenda")
                .Where(A => A.Type != "Fee")
                .Where(A => A.Type != "Status")
                .ToList();

            StateHasChanged();
        }

        void ShowAgenda(String option, String type)
        {
            if (option == "Show")
            {
                displaySection = type;
            }
            else
            {
                displaySection = "";
            }
        }

        private void PrepareForInsert(String header)
        {
            PrepDocumentList();
            parentId = selectedChapterId;

            int? maxSeq = lstAll.Max(A => A.SeqNo);


            editObject = new UsrOrDefChapterManagement
            {
                CaseType = "",
                CaseTypeGroup = "",
                ParentId = parentId,
                SeqNo = maxSeq
            };

            if (header == "Agenda")
            {
                editObject.Type = "Agenda";
            }
            else if (header == "Fee")
            {
                editObject.Type = "Fee";
            }
            else if (header == "Status")
            {
                editObject.Type = "Status";
            }

            customHeader = "Add New " + header;
            selectedList = header;
        }

        private void PrepareForEdit(UsrOrDefChapterManagement item, String header)
        {
            PrepDocumentList();
            customHeader = header;
            selectedList = header;
            editObject = item;
        }

        private void PrepareForDelete(UsrOrDefChapterManagement item)
        {
            editObject = item;
        }

        private async void PrepDocumentList()
        {
            dropDownDocumentList = await chapterManagementService.GetDocumentList(selectedCaseType);

            StateHasChanged();
        }

        private void OnDragDrop()
        {
            var docsOrder = "";

            foreach (var item in lstDocs)
            {
                docsOrder = docsOrder + " > " + item.Name;
            }

            dragState = docsOrder;
        }
    }
}
