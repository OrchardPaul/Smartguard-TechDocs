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
using Blazored.Modal.Services;
using Blazored.Modal;
using Gizmo_V1_02.Pages.Shared.Modals;

namespace Gizmo_V1_02.Pages.OR_RESI_Chapters
{
    public partial class ChapterList
    {
        [Inject]
        IModalService Modal { get; set; }

        [Inject]
        private IChapterManagementService chapterManagementService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public IUserSessionState sessionState { get; set; }

        private List<UsrOrDefChapterManagement> lstChapters;

        private List<UsrOrDefChapterManagement> lstAll { get; set; } = new List<UsrOrDefChapterManagement>();
        private List<UsrOrDefChapterManagement> lstAgendas { get; set; } = new List<UsrOrDefChapterManagement>();
        private List<UsrOrDefChapterManagement> lstFees { get; set; } = new List<UsrOrDefChapterManagement>();
        private List<UsrOrDefChapterManagement> lstDocs { get; set; } = new List<UsrOrDefChapterManagement>();
        private List<UsrOrDefChapterManagement> lstStatus { get; set; } = new List<UsrOrDefChapterManagement>();


        public List<DmDocuments> dropDownDocumentList;


        public string editCaseType { get; set; } = "";
        public string isCaseTypeOrGroup { get; set; } = "";
        
        public UsrOrDefChapterManagement editObject = new UsrOrDefChapterManagement();
        public UsrOrDefChapterManagement editChapterObject = new UsrOrDefChapterManagement();

        string customHeader = string.Empty;
        string selectedList = string.Empty;

        string displaySection { get; set; } = "";
        string selectedCaseType { get; set; } = "";
        string selectedCaseTypeGroup { get; set; } = "";
        string selectedChapter { get; set; } = "";
        private int selectedChapterId { get; set; } = -1;


        private bool editChapterDetail { get; set; } = false;

        public string ModalInfoHeader { get; set; }
        public string ModalInfoText { get; set; }
        public string ModalHeight { get; set; }
        public string ModalWidth { get; set; }

        public string navDisplay = "Agenda";

        public List<string> lstDocTypes { get; set; } = new List<string> { "Document", "Letter", "Form", "Email", "Step" };

        protected override async Task OnInitializedAsync()
        {
            //var authenticationState = await pageAuthorisationState.ChapterListAuthorisation();

            //if (!authenticationState)
            //{
            //    string returnUrl = HttpUtility.UrlEncode($"/chapterlist");
            //    NavigationManager.NavigateTo($"Identity/Account/Login?returnUrl={returnUrl}", true);
            //}

            bool gotLock = sessionState.Lock;
            while (gotLock)
            {
                await Task.Yield();
                gotLock = sessionState.Lock;
            }


            //if (authenticationState)
            //{
                try
                {
                    RefreshChapters();
                }
                catch(Exception)
                {
                    NavigationManager.NavigateTo($"/", true);
                }
            //}

        }

        public void DirectToLogin()
        {
            string returnUrl = HttpUtility.UrlEncode($"/");
            NavigationManager.NavigateTo($"Identity/Account/Login?returnUrl={returnUrl}", true);
        }

        void SelectHome()
        {
            selectedChapter = "";
        }

        void SelectCaseTypeGroup(string caseTypeGroup)
        {
            selectedCaseTypeGroup = (selectedCaseTypeGroup == caseTypeGroup) ? "" : caseTypeGroup;
            selectedCaseType = "";
            selectedChapter = "";
        }

        void SelectCaseType(string caseType)
        {
            selectedCaseType = (selectedCaseType == caseType) ? "" : caseType;
            selectedChapter = "";
            PrepDocumentList();
        }

        private void SelectChapter(string chapter, int chapterID)
        {
            lstAll = new List<UsrOrDefChapterManagement>();

            selectedChapterId = chapterID;
            selectedChapter = chapter;

            RefreshChapterItems("All");

        }

        private async void RefreshChapters()
        {
            lstChapters = await chapterManagementService.GetAllChapters();

            StateHasChanged();
        }

        private async void RefreshChapterItems(string listType)
        {
            lstAll = await chapterManagementService.GetItemListByChapter(selectedChapterId);

            if (listType == "Agenda" | listType == "All")
            {
                lstAgendas = lstAll
                                    .OrderBy(A => A.SeqNo)
                                    .Where(A => A.Type == "Agenda")
                                    .ToList();

            }
            if (listType == "Docs" | listType == "All")
            {
                lstDocs = lstAll
                                    .OrderBy(A => A.SeqNo)
                                    .Where(A => lstDocTypes.Contains(A.Type))
                                    .ToList();

            }
            if (listType == "Fees" | listType == "All")
            {
                lstFees = lstAll
                                    .OrderBy(A => A.SeqNo)
                                    .Where(A => A.Type == "Fee")
                                    .ToList();
                
            }
            if (listType == "Status" | listType == "All")
            {
                lstStatus = lstAll
                                    .OrderBy(A => A.SeqNo)
                                    .Where(A => A.Type == "Status")
                                    .ToList();

            }

            StateHasChanged();
        }


        public void RefreshSelectedList()
        {
            RefreshChapterItems(displaySection);
        }

        private void PrepareForEdit(UsrOrDefChapterManagement item, string header)
        {
            customHeader = header;
            selectedList = header;
            editObject = item;

            ShowChapterDetailModal();
        }

        private void PrepareForInsert(string header)
        {
            selectedList = header;
            editObject = new UsrOrDefChapterManagement();

            ShowChapterDetailModal();
        }

        private void PrepareChapterForInsert()
        {
            editChapterObject = new UsrOrDefChapterManagement();

            if(!(selectedCaseTypeGroup == ""))
            {
                editChapterObject.CaseTypeGroup = selectedCaseTypeGroup;
            }
            else
            {
                editChapterObject.CaseTypeGroup = "";
            }

            if(!(selectedCaseType == ""))
            {
                editChapterObject.CaseType = selectedCaseType;
            }
            else
            {
                editChapterObject.CaseType = "";
            }

            editChapterObject.Type = "Chapter";
            editChapterObject.ParentId = 0;
            editChapterObject.SeqNo = 0;
            editChapterObject.SuppressStep = "";
            editChapterObject.EntityType = "";

            ShowChapterAddOrEditModel();
        }

        private void PrepareCaseTypeForEdit(string caseType, string option)
        {
            editCaseType = caseType;
            isCaseTypeOrGroup = option;

            ShowCaseTypeEditModal();
        }

        private async void PrepDocumentList()
        {
            dropDownDocumentList = await chapterManagementService.GetDocumentList(selectedCaseType);

            StateHasChanged();
        }

        private void ToggleChapterDetailEdit(string selectedDetail)
        {
            editChapterDetail = !editChapterDetail;
            displaySection = selectedDetail;
        }

        private void PrepareModalInfoDisplay(string modalHeader
                                                , string modalText
                                                , string modalHeight
                                                , string modalWidth)
        {
            ModalInfoHeader = modalHeader;
            ModalInfoText = modalText;
            ModalHeight = modalHeight;
            ModalWidth = modalWidth;
        }

        protected void ShowNav(string displayChange)
        {
            navDisplay = displayChange;
        }

       

        /// <summary>
        /// Moves a sequecnce item up or down a list of type [UsrOrDefChapterManagement]
        /// </summary>
        /// <remarks>
        /// <para>Up: swaps the item with the preceding item in the lest by reducing sequence number by 1 </para>
        /// <para>Up: swaps the item with the following item in the lest by increasing sequence number by 1 </para>
        /// </remarks>
        /// <param name="selectobject">: current list item</param>
        /// <param name="listType">: Doc or Fee</param>
        /// <param name="direction">: Up or Down</param>
        /// <returns>No return</returns>
        protected async void MoveSeq(UsrOrDefChapterManagement selectobject, string listType, string direction)
        {
            var lstItems = new List<UsrOrDefChapterManagement>();
            int incrementBy;

            incrementBy = (direction.ToLower() == "up" ? -1 : 1);

            switch (listType)
            {
                case "Docs":
                    lstItems = lstDocs;
                    break;
                case "Fees":
                    lstItems = lstFees;
                    break;
            }

            var swapItem = lstItems.Where(D => D.SeqNo == (selectobject.SeqNo + incrementBy)).SingleOrDefault();
            if (!(swapItem is null))
            {
                selectobject.SeqNo += incrementBy;
                swapItem.SeqNo = swapItem.SeqNo + (incrementBy * -1);

                await chapterManagementService.Update(selectobject);
                await chapterManagementService.Update(swapItem);
            }
            else
            {
                CondenseSeq(lstItems);
            }

            RefreshChapterItems(listType);
        }

        private async void CondenseSeq(List<UsrOrDefChapterManagement> lstItems)
        {
            int seqNo = 0;

            foreach (UsrOrDefChapterManagement item in lstItems.OrderBy(A => A.SeqNo))
            {
                seqNo += 1;
                item.SeqNo = seqNo;

                await chapterManagementService.Update(item);
            }

        }

        protected void ShowChapterAddOrEditModel()
        {
            Action Action = RefreshChapters;

            var parameters = new ModalParameters();
            parameters.Add("TaskObject", editChapterObject);
            parameters.Add("DataChanged", Action);
            parameters.Add("AllObjects", lstChapters);

            Modal.Show<ChapterAddOrEdit>("Chapter", parameters);
        }

        protected void ShowCaseTypeEditModal()
        {
            Action Action = RefreshChapters;

            var parameters = new ModalParameters();
            parameters.Add("TaskObject", editCaseType);
            parameters.Add("DataChanged", Action);
            parameters.Add("isCaseTypeOrGroup", isCaseTypeOrGroup);
            parameters.Add("originalName", editCaseType);
            parameters.Add("caseTypeGroupName", selectedCaseTypeGroup);

            Modal.Show<ChapterCaseTypeEdit>("Chapter", parameters);
        }


        protected void ShowChapterDetailModal()
        {
            Action Action = RefreshSelectedList;

            var parameters = new ModalParameters();
            parameters.Add("TaskObject", editObject);
            parameters.Add("DataChanged", Action);
            parameters.Add("selectedCaseType", selectedCaseType);
            parameters.Add("selectedList", selectedList);
            parameters.Add("dropDownDocumentList", dropDownDocumentList);

            Modal.Show<ChapterDetail>(selectedList, parameters);
        }


        protected void PrepareChapterDetailDelete(UsrOrDefChapterManagement selectedChapterItem)
        {
            editObject = selectedChapterItem;

            Action SelectedDeleteAction = HandleChapterDetailDelete;
            var parameters = new ModalParameters();
            parameters.Add("InfoHeader", "Delete?");
            parameters.Add("ModalHeight", "300px");
            parameters.Add("ModalWidth", "500px");
            parameters.Add("DeleteAction", SelectedDeleteAction);

            Modal.Show<ModalDelete>("Delete?", parameters);
        }

        private async void HandleChapterDetailDelete()
        {
            await chapterManagementService.Delete(editObject.Id);

            RefreshChapterItems(selectedList);

        }
    }
}
