using GadjIT.ClientContext.OR_RESI;
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
using GadjIT.ClientContext.OR_RESI.Custom;
using GadjIT.ClientContext.OR_RESI.Functions;

namespace Gizmo_V1_02.Pages.OR_RESI_Chapters
{
    public partial class ChapterList
    {
        [Inject]
        IModalService Modal { get; set; }

        [Inject]
        private IChapterManagementService chapterManagementService { get; set; }


        [Inject]
        private IPartnerAccessService partnerAccessService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public IUserSessionState sessionState { get; set; }

        //private List<UsrOrDefChapterManagement> lstChapters;
        private List<VmUsrOrDefChapterManagement> lstChapters { get; set; } = new List<VmUsrOrDefChapterManagement>();

        private List<VmUsrOrDefChapterManagement> lstAll { get; set; } = new List<VmUsrOrDefChapterManagement>();
        private List<VmUsrOrDefChapterManagement> lstAltSystemChapterItems { get; set; } = new List<VmUsrOrDefChapterManagement>();

        private List<VmUsrOrDefChapterManagement> lstAgendas { get; set; } = new List<VmUsrOrDefChapterManagement>();
        private List<VmUsrOrDefChapterManagement> lstFees { get; set; } = new List<VmUsrOrDefChapterManagement>();
        private List<VmChapterFee> lstVmFeeModalItems { get; set; } = new List<VmChapterFee>();
        private List<VmUsrOrDefChapterManagement> lstDocs { get; set; } = new List<VmUsrOrDefChapterManagement>();
        private List<VmUsrOrDefChapterManagement> lstStatus { get; set; } = new List<VmUsrOrDefChapterManagement>();


        public List<DmDocuments> dropDownChapterList;
        public List<CaseTypeGroups> partnerCaseTypeGroups;
        public List<fnORCHAGetFeeDefinitions> feeDefinitions;

        public string editCaseType { get; set; } = "";

        public UsrOrDefChapterManagement editChapter { get; set; }
        public string isCaseTypeOrGroup { get; set; } = "";
        
        public VmUsrOrDefChapterManagement editObject = new VmUsrOrDefChapterManagement { ChapterObject = new UsrOrDefChapterManagement() };
        public VmUsrOrDefChapterManagement editChapterObject = new VmUsrOrDefChapterManagement { ChapterObject = new UsrOrDefChapterManagement() };

        string customHeader = string.Empty;
        string selectedList = string.Empty;

        string displaySection { get; set; } = "";

        [Parameter]
        public string selectedCaseType { get; set; } = "";
        
        [Parameter]
        public string selectedCaseTypeGroup { get; set; } = "";
        
        [Parameter]
        public string selectedChapter { get; set; } = "";

        int rowChanged { get; set; } = 0;

        private int selectedChapterId { get; set; } = -1;

        private int? altSysSelectedChapterId { get; set; }

        public string ModalInfoHeader { get; set; }
        public string ModalInfoText { get; set; }
        public string ModalHeight { get; set; }
        public string ModalWidth { get; set; }

        public string navDisplay = "Docs";

        public bool compareSystems = false;

        private string RowChangedClass { get; set; } = "row-changed-nav3";
       

        public bool displaySpinner = true;

        public bool ListChapterLoaded = false;

        
        public List<string> lstDocTypes { get; set; } = new List<string> { "Doc", "Letter", "Form", "Email", "Step" };


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

            try
            {
                RefreshChapters();
                partnerCaseTypeGroups = await partnerAccessService.GetPartnerCaseTypeGroups();
            }
            catch (Exception)
            {
                NavigationManager.NavigateTo($"/", true);
            }


        }



        public void DirectToLogin()
        {
            string returnUrl = HttpUtility.UrlEncode($"/");
            NavigationManager.NavigateTo($"Identity/Account/Login?returnUrl={returnUrl}", true);
        }

        void SelectHome()
        {
            selectedChapter = "";
            rowChanged = 0;
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

        private async void SelectChapter(string chapter, int chapterID)
        {
            displaySpinner = true;

            lstAll = new List<VmUsrOrDefChapterManagement>();

            selectedChapterId = chapterID;
            selectedChapter = chapter;
            compareSystems = false;
            rowChanged = 0;

            await RefreshChapterItems("All");

            StateHasChanged();

        }

        private async void RefreshChapters()
        {
            ListChapterLoaded = false;

            var lstC = await chapterManagementService.GetAllChapters();
            lstChapters = lstC.Select(A => new VmUsrOrDefChapterManagement { ChapterObject = A } ).ToList();

            if(!(selectedChapter is null) & selectedChapter != "")
            {
                SelectChapter(selectedChapter,lstChapters
                                                    .Where(C => C.ChapterObject.CaseTypeGroup == selectedCaseTypeGroup)
                                                    .Where(C => C.ChapterObject.CaseType == selectedCaseType)
                                                    .Where(C => C.ChapterObject.Name == selectedChapter)
                                                    .Select(C => C.ChapterObject.Id).SingleOrDefault());
            }

            ListChapterLoaded = true;

            StateHasChanged();
        }

        private async Task<bool> RefreshChapterItems(string listType)
        {

            if (listType == "Chapters")
            {

                var lstC = await chapterManagementService.GetAllChapters();
                lstChapters = lstC.Select(A => new VmUsrOrDefChapterManagement { ChapterObject = A }).ToList();

            }
            else
            {
                var lst = await chapterManagementService.GetItemListByChapter(selectedChapterId);

                lstAll = lst.Select(A => new VmUsrOrDefChapterManagement { ChapterObject = A }).ToList();
                feeDefinitions = await chapterManagementService.GetFeeDefs(selectedCaseTypeGroup, selectedCaseType);

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

                    lstVmFeeModalItems = feeDefinitions
                                            .Select(FD => new VmChapterFee
                                            {
                                                FeeItem = lstFees
                                                                .Where(F => FD.FeeDesc == F.ChapterObject.Name)
                                                                .SingleOrDefault() is null
                                                                ? new UsrOrDefChapterManagement
                                                                {
                                                                    ParentId = selectedChapterId,
                                                                    Name = FD.FeeDesc,
                                                                    SeqNo = 1000,
                                                                    Type = "Fee",
                                                                    CaseType = "",
                                                                    CaseTypeGroup = "",
                                                                    CompleteName = ""
                                                                }
                                                                : lstFees
                                                                    .Where(F => FD.FeeDesc == F.ChapterObject.Name)
                                                                    .Select(F => F.ChapterObject)
                                                                    .SingleOrDefault(),
                                                feeDefinition = FD,
                                                selected = lstFees
                                                                .Where(F => FD.FeeDesc == F.ChapterObject.Name)
                                                                .SingleOrDefault() is null
                                                                ? false : true
                                            })
                                            .ToList();

                    CondenseSeq("Fees");
                }
                if (listType == "Status" | listType == "All")
                {
                    lstStatus = lstAll
                                        .OrderBy(A => A.ChapterObject.SeqNo)
                                        .Where(A => A.ChapterObject.Type == "Status")
                                        .ToList();

                }
            }

            displaySpinner = false;

            rowChanged = 0;
            return true;
        }

        private async void ToggleComparison()
        {
            compareSystems = !compareSystems;
            if (compareSystems)
            {
                await GetAltSytemChapterItems();
            }
        }

        private async Task<bool> GetAltSytemChapterItems()
        {
            if (compareSystems)
            {
                var test = await RefreshChapterItems(navDisplay);

                await sessionState.SwitchSelectedSystem();
                var temp = await chapterManagementService.GetItemListByChapter(selectedChapterId);
                if(temp.Count > 0)
                {
                    lstAltSystemChapterItems = temp.Select(T => new VmUsrOrDefChapterManagement { ChapterObject = T }).ToList();
                    altSysSelectedChapterId = lstAltSystemChapterItems
                                                .Select(A => A.ChapterObject.ParentId)
                                                .FirstOrDefault();

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
                if (chapterItem.IsChapterItemMatch(altObject))
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

            return chapterItem;
        }

        public async void CompareChapterItemsToAltSytemAction()
        {
            
            await GetAltSytemChapterItems();
        }

        public async void RefreshSelectedList()
        {
            await RefreshChapterItems(navDisplay);
        }

        private void PrepareForEdit(VmUsrOrDefChapterManagement item, string header)
        {
            customHeader = header;
            selectedList = header;
            editObject = item;

            ShowChapterDetailModal();
        }

        private void PrepareFeesForEdit()
        {
            ShowChapterFeesModal();
        }

        private void PrepareForInsert(string header, string type)
        {
            selectedList = type;

            editObject = new VmUsrOrDefChapterManagement { ChapterObject = new UsrOrDefChapterManagement() };
            editObject.ChapterObject.CaseType = "";
            editObject.ChapterObject.Type = (type == "Steps and Documents") ? "Doc" : type;
            editObject.ChapterObject.CaseTypeGroup = "";
            editObject.ChapterObject.SeqNo = lstAll
                                                .OrderByDescending(A => A.ChapterObject.SeqNo)
                                                .Select(A => A.ChapterObject.SeqNo)
                                                .FirstOrDefault() + 1;
            editObject.ChapterObject.ParentId = selectedChapterId;

            ShowChapterDetailModal();
        }

        private void PrepareChapterForInsert()
        {
            editChapterObject = new VmUsrOrDefChapterManagement { ChapterObject = new UsrOrDefChapterManagement() };

            if(!(selectedCaseTypeGroup == ""))
            {
                editChapterObject.ChapterObject.CaseTypeGroup = selectedCaseTypeGroup;
            }
            else
            {
                editChapterObject.ChapterObject.CaseTypeGroup = "";
            }

            if(!(selectedCaseType == ""))
            {
                editChapterObject.ChapterObject.CaseType = selectedCaseType;
            }
            else
            {
                editChapterObject.ChapterObject.CaseType = "";
            }

            editChapterObject.ChapterObject.Type = "Chapter";
            editChapterObject.ChapterObject.ParentId = 0;
            editChapterObject.ChapterObject.SeqNo = 0;
            editChapterObject.ChapterObject.SuppressStep = "";
            editChapterObject.ChapterObject.EntityType = "";

            ShowChapterAddOrEditModel();
        }

        private void PrepareCaseTypeForEdit(string caseType, string option)
        {
            editCaseType = caseType;
            isCaseTypeOrGroup = option;

            ShowCaseTypeEditModal();
        }

        private void PrepareChapterForEdit(UsrOrDefChapterManagement chapter, string option)
        {
            editChapter = chapter;
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


        protected void ShowNav(string displayChange)
        {
            compareSystems = false;
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
                case "Chapters":
                    lstItems = lstChapters
                                        .Where(A => A.ChapterObject.CaseTypeGroup == selectedCaseTypeGroup)
                                        .Where(A => A.ChapterObject.CaseType == selectedCaseType)
                                        .OrderBy(A => A.ChapterObject.SeqNo)
                                        .ToList();
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
            await RefreshChapterItems(listType);
        }

        private List<VmUsrOrDefChapterManagement> GetRelevantChapterList(string listType)
        {
            var listItems = new List<VmUsrOrDefChapterManagement>();

            switch (listType)
            {
                case "Docs":
                    listItems = lstDocs;
                    break;
                case "Fees":
                    listItems = lstFees;
                    break;
                case "Chapters":
                    listItems = lstChapters
                                        .Where(A => A.ChapterObject.CaseTypeGroup == selectedCaseTypeGroup)
                                        .Where(A => A.ChapterObject.CaseType == selectedCaseType)
                                        .OrderBy(A => A.ChapterObject.SeqNo)
                                        .ToList();
                    break;
            }

            return listItems;
        }

        protected async void CondenseSeq(string ListType )
        {

            var ListItems = GetRelevantChapterList(ListType);

            int seqNo = 0;

            foreach (VmUsrOrDefChapterManagement item in ListItems.OrderBy(A => A.ChapterObject.SeqNo))
            {
                seqNo += 1;
                item.ChapterObject.SeqNo = seqNo;

                await chapterManagementService.Update(item.ChapterObject);
            }
            StateHasChanged();
        }

        protected void CondenseFeeSeq()
        {
            RefreshSelectedList();
            
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
            parameters.Add("TaskObject", (isCaseTypeOrGroup == "Chapter") ? editChapter.Name : editCaseType);
            parameters.Add("originalName", (isCaseTypeOrGroup == "Chapter") ? editChapter.Name : editCaseType);
            if(isCaseTypeOrGroup == "Chapter")
            {
                parameters.Add("Chapter", editChapter);
            }
            parameters.Add("DataChanged", Action);
            parameters.Add("isCaseTypeOrGroup", isCaseTypeOrGroup);
            parameters.Add("caseTypeGroupName", selectedCaseTypeGroup);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-chapter-casetype"
            };

            Modal.Show<ChapterCaseTypeEdit>("Chapter", parameters, options);
        }


        protected void ShowChapterDetailModal()
        {
            Action Action = RefreshSelectedList;

            var parameters = new ModalParameters();
            parameters.Add("TaskObject", editObject.ChapterObject);
            parameters.Add("DataChanged", Action);
            parameters.Add("selectedCaseType", selectedCaseType);
            parameters.Add("selectedList", selectedList);
            parameters.Add("dropDownChapterList", dropDownChapterList);
            parameters.Add("CaseTypeGroups", partnerCaseTypeGroups);

            string className = "modal-chapter-item";

            if(selectedList == "Steps and Documents")
            {
                className = "modal-chapter-doc";
            }
            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal " + className
            };

            Modal.Show<ChapterDetail>(selectedList, parameters, options);
        }

        protected void ShowChapterFeesModal()
        {
            Action Action = RefreshSelectedList;
            Action RefreshFeeOrder = CondenseFeeSeq;

            var parameters = new ModalParameters();
            parameters.Add("DataChanged", Action);
            parameters.Add("RefreshFeeOrder", RefreshFeeOrder);
            parameters.Add("feeItems", lstVmFeeModalItems);
            parameters.Add("SeletedChapterId", selectedChapterId);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-chapter-fees"
            };


            Modal.Show<ChapterFees>("Fees", parameters, options);
        }


        protected void PrepareChapterDetailDelete(VmUsrOrDefChapterManagement selectedChapterItem)
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

        private void PrepareForComparison(VmUsrOrDefChapterManagement selectedItem)
        {
            editObject = selectedItem;

            ShowChapterComparisonModal();
        }

        protected void ShowChapterComparisonModal()
        {
            Action Compare = CompareChapterItemsToAltSytemAction;

            var parameters = new ModalParameters();
            parameters.Add("Object", editObject);
            parameters.Add("ComparisonRefresh", Compare);
            parameters.Add("sessionState", sessionState);
            parameters.Add("CurrentSysParentId", selectedChapterId);
            parameters.Add("AlternateSysParentId", altSysSelectedChapterId);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-chapter-comparison"
            };

            Modal.Show<ChapterItemComparison>("Synchronise Chapter Item", parameters, options);
        }


        private async void HandleChapterDetailDelete()
        {
            await chapterManagementService.Delete(editObject.ChapterObject.Id);

            await RefreshChapterItems(navDisplay);

        }

        /// <summary>
        /// swaps the CSS class for indicating that a row has changed.  
        /// This ensures that CSS recognises a new change even if the change occurs on the same row 
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns>string: row-changed or row-changedx</returns>
        private string ToggleRowChangedClass()
        {
            switch (RowChangedClass)
            {
                case "row-changed-nav3":
                    RowChangedClass = "row-changed-nav3x";
                    break;
                case "row-changed-nav3x":
                    RowChangedClass = "row-changed-nav3xx";
                    break;
                default:
                    RowChangedClass = "row-changed-nav3";
                    break;
            }

            return RowChangedClass;

        }

        private bool SequenceIsValid(string listType)
        {
            List<VmUsrOrDefChapterManagement> listItems = GetRelevantChapterList(listType);

            bool isValid = true;

            for (int i = 0; i < listItems.Count; i++)
            {
                if (listItems[i].ChapterObject.SeqNo != i+1)
                {
                    isValid = false;
                }
                
            }

            return isValid;

        }
    }
}
