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

namespace Gizmo_V1_02.Pages.OR_RESI_Chapters
{
    public partial class ChapterList
    {
        [Inject]
        IChapterManagementService chapterManagementService { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState> authenticationStateTask { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }


        List<UsrOrDefChapterManagement> lstChapters;
        List<UsrOrDefChapterManagement> lstAgendas;
        List<UsrOrDefChapterManagement> lstFees;
        List<UsrOrDefChapterManagement> lstDocs;
        List<UsrOrDefChapterManagement> lstStatus;

        List<String> dropDownDocumentList;

        int parentId;

        UsrOrDefChapterManagement editObject = new UsrOrDefChapterManagement();

        string customHeader = string.Empty;
        string selectedList = string.Empty;

        string displaySection { get; set; } = "";
        string selectedCaseType { get; set; } = "";
        string selectedCaseTypeGroup { get; set; } = "";
        string selectedChapter { get; set; } = "";

        protected List<string> CaseTypeList;

        protected override async Task OnInitializedAsync()
        {
            var authenticationState = await authenticationStateTask;

            if (!authenticationState.User.IsInRole("Admin"))
            {
                string returnUrl = HttpUtility.UrlEncode($"/chapterlist");
                NavigationManager.NavigateTo($"/Identity/Account/Login?returnUrl={returnUrl}", true);
            }


            lstChapters = await service.GetChapterListByCaseType("Chapter", "");
            CaseTypeList = await chapterManagementService.GetCaseTypes();
        }

        void SelectHome()
        {
            selectedCaseType = "";
            selectedChapter = "";
        }

        async Task SelectCaseType(string caseType)
        {
            lstChapters = await service.GetChapterListByCaseType("Chapter", caseType);

            selectedCaseType = caseType;
            selectedChapter = "";
        }

        async Task selectDocList(String chapter)
        {
            lstAgendas = await service.GetDocListByChapterAndDocType(selectedCaseType, chapter, "Agenda");
            lstFees = await service.GetDocListByChapterAndDocType(selectedCaseType, chapter, "Fee");
            lstDocs = await service.GetDocListByChapter(selectedCaseType, chapter);
            lstStatus = await service.GetDocListByChapterAndDocType(selectedCaseType, chapter, "Status");


            selectedChapter = chapter;
        }

        private async void DataChanged()
        {
            lstAgendas = await service.GetDocListByChapterAndDocType(selectedCaseType, selectedChapter, "Agenda");
            lstFees = await service.GetDocListByChapterAndDocType(selectedCaseType, selectedChapter, "Fee");
            lstDocs = await service.GetDocListByChapter(selectedCaseType, selectedChapter);
            lstStatus = await service.GetDocListByChapterAndDocType(selectedCaseType, selectedChapter, "Status");

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
            parentId = service.GetParentId(selectedCaseType, selectedChapter);

            editObject = new UsrOrDefChapterManagement
            {
                CaseType = "",
                CaseTypeGroup = "",
                ParentId = parentId,
                SeqNo = service.GetMaxSeqNum(parentId)
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

        private async Task Delete()
        {
            await service.Delete(editObject.Id);
            await jsRuntime.InvokeAsync<object>("CloseModal", "confirmDeleteModal");
            DataChanged();
            editObject = new UsrOrDefChapterManagement();
        }

        private void PrepDocumentList()
        {
            dropDownDocumentList = service.GetDocumentList(selectedCaseType);
        }
    }
}
