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
using Gizmo.Context.OR_RESI.Custom;

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

        private List<VmUsrOrDefChapterManagement> lstAll { get; set; } = new List<VmUsrOrDefChapterManagement>();
        private List<VmUsrOrDefChapterManagement> lstAltSystemChapterItems { get; set; } = new List<VmUsrOrDefChapterManagement>();

        private List<VmUsrOrDefChapterManagement> lstAgendas { get; set; } = new List<VmUsrOrDefChapterManagement>();
        private List<VmUsrOrDefChapterManagement> lstFees { get; set; } = new List<VmUsrOrDefChapterManagement>();
        private List<VmUsrOrDefChapterManagement> lstDocs { get; set; } = new List<VmUsrOrDefChapterManagement>();
        private List<VmUsrOrDefChapterManagement> lstStatus { get; set; } = new List<VmUsrOrDefChapterManagement>();


        public List<DmDocuments> dropDownChapterList;


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

        int rowChanged { get; set; } = 0;

        private int selectedChapterId { get; set; } = -1;


        private bool editChapterDetail { get; set; } = false;

        public string ModalInfoHeader { get; set; }
        public string ModalInfoText { get; set; }
        public string ModalHeight { get; set; }
        public string ModalWidth { get; set; }

        public string navDisplay = "Agenda";

        public bool compareSystems = false;


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
            PrepChapterList();
        }

        private void SelectChapter(string chapter, int chapterID)
        {
            lstAll = new List<VmUsrOrDefChapterManagement>();

            selectedChapterId = chapterID;
            selectedChapter = chapter;
            compareSystems = false;

            RefreshChapterItems("All");

        }

        private async void RefreshChapters()
        {
            lstChapters = await chapterManagementService.GetAllChapters();

            StateHasChanged();
        }

        private async void RefreshChapterItems(string listType)
        {
            var lst = await chapterManagementService.GetItemListByChapter(selectedChapterId);

            lstAll = lst.Select(A => new VmUsrOrDefChapterManagement { ChapterObject = A }).ToList();

            if (listType == "Agenda" | listType == "All")
            {
                lstAgendas = lstAll
                                    .OrderBy(A => A.ChapterObject.SeqNo)
                                    .Where(A => A.ChapterObject.Type == "Agenda")
                                    .ToList();

            }
            if (listType == "Docs" | listType == "All")
            {
                lstDocs = lstAll
                                    .OrderBy(A => A.ChapterObject.SeqNo)
                                    .Where(A => lstDocTypes.Contains(A.ChapterObject.Type))
                                    .ToList();

            }
            if (listType == "Fees" | listType == "All")
            {
                lstFees = lstAll
                                    .OrderBy(A => A.ChapterObject.SeqNo)
                                    .Where(A => A.ChapterObject.Type == "Fee")
                                    .ToList();
                
            }
            if (listType == "Status" | listType == "All")
            {
                lstStatus = lstAll
                                    .OrderBy(A => A.ChapterObject.SeqNo)
                                    .Where(A => A.ChapterObject.Type == "Status")
                                    .ToList();

            }


            StateHasChanged();
        }

        private async Task<bool> GetAltSytemChapterItems()
        {
            compareSystems = !compareSystems;

            if (compareSystems)
            {


                lstDocs = lstDocs.Select(D => { D.ComparisonIcon = null; return D; }).ToList();

                await sessionState.SwitchSelectedSystem();
                var temp = await chapterManagementService.GetItemListByChapter(selectedChapterId);
                lstAltSystemChapterItems = temp.Select(T => new VmUsrOrDefChapterManagement { ChapterObject = T }).ToList();
                await sessionState.ResetSelectedSystem();

                foreach (var item in lstDocs)
                {
                    CompareChapterItemsToAltSytem(item);
                }

                foreach (var item in lstAgendas)
                {
                    CompareChapterItemsToAltSytem(item);
                }

                foreach (var item in lstFees)
                {
                    CompareChapterItemsToAltSytem(item);
                }

                foreach (var item in lstStatus)
                {
                    CompareChapterItemsToAltSytem(item);
                }

                StateHasChanged();
            }

            return true;
        }

        private VmUsrOrDefChapterManagement CompareChapterItemsToAltSytem(VmUsrOrDefChapterManagement chapterItem)
        {
            

            var altObject = lstAltSystemChapterItems
                                .Where(A => A.ChapterObject.Name == chapterItem.ChapterObject.Name)
                                .SingleOrDefault();

            if(altObject is null)
            {
                chapterItem.ComparisonResult = "No match";
                chapterItem.ComparisonIcon = "times";
            }
            else
            {
                if (chapterItem.IsChapterItemsSame(altObject))
                {
                    chapterItem.ComparisonResult = "Exact match";
                    chapterItem.ComparisonIcon = "check";

                }
                else
                {
                    chapterItem.ComparisonResult = "Partial match";
                    chapterItem.ComparisonIcon = "exclamation";

                }

            }

            if (chapterItem.ChapterObject.SeqNo < 3)
            {
                chapterItem.ComparisonIcon = "times";
            }
            else
            {
                if (chapterItem.ChapterObject.SeqNo < 5)
                {
                    chapterItem.ComparisonIcon = "check";
                }
                else
                {
                    chapterItem.ComparisonIcon = "exclamation";

                }
            }
            return chapterItem;
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

        private async void PrepChapterList()
        {
            if(!(selectedCaseType == ""))
            {
                dropDownChapterList = await chapterManagementService.GetDocumentList(selectedCaseType);
                StateHasChanged();
            }
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
            rowChanged = 0;
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
        /// <param name="listType">: Docs or Fees</param>
        /// <param name="direction">: Up or Down</param>
        /// <returns>No return</returns>
        protected async void MoveSeq(UsrOrDefChapterManagement selectobject, string listType, string direction)
        {
            var lstItems = new List<VmUsrOrDefChapterManagement>();
            int incrementBy;

            incrementBy = (direction.ToLower() == "up" ? -1 : 1);

            rowChanged = (int)(selectobject.SeqNo + incrementBy);
            
            switch (listType)
            {
                case "Docs":
                    lstItems = lstDocs;
                    break;
                case "Fees":
                    lstItems = lstFees;
                    break;
            }

            
            var swapItem = lstItems.Where(D => D.ChapterObject.SeqNo == (selectobject.SeqNo + incrementBy)).SingleOrDefault();
            if (!(swapItem is null))
            {
                selectobject.SeqNo += incrementBy;
                swapItem.ChapterObject.SeqNo = swapItem.ChapterObject.SeqNo + (incrementBy * -1);

                await chapterManagementService.Update(selectobject);
                await chapterManagementService.Update(swapItem.ChapterObject);


            }
            else
            {
                CondenseSeq(lstItems);
            }

            RefreshChapterItems(listType);
        }

        private async void CondenseSeq(List<VmUsrOrDefChapterManagement> lstItems)
        {
            int seqNo = 0;

            foreach (VmUsrOrDefChapterManagement item in lstItems.OrderBy(A => A.ChapterObject.SeqNo))
            {
                seqNo += 1;
                item.ChapterObject.SeqNo = seqNo;

                await chapterManagementService.Update(item.ChapterObject);
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
            parameters.Add("dropDownChapterList", dropDownChapterList);

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
