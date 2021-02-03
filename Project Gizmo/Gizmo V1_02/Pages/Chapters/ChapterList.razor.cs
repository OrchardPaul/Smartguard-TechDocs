using GadjIT.ClientContext.P4W;
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
using GadjIT.ClientContext.P4W.Custom;
using GadjIT.ClientContext.P4W.Functions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Gizmo_V1_02.Pages.Chapters
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

        public string navDisplay = "Agenda";

        private bool seqMoving = false;

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
            navDisplay = "Agenda";

            await RefreshChapterItems("All");

            StateHasChanged();
        }


        private async void RefreshChapters()
        {
            ListChapterLoaded = false;

            var lstC = await chapterManagementService.GetAllChapters();
            lstChapters = lstC.Select(A => new VmUsrOrDefChapterManagement { ChapterObject = A }).ToList();

            if (!(selectedChapter is null) & selectedChapter != "")
            {
                SelectChapter(selectedChapter, lstChapters
                                                    .Where(C => C.ChapterObject.CaseTypeGroup == selectedCaseTypeGroup)
                                                    .Where(C => C.ChapterObject.CaseType == selectedCaseType)
                                                    .Where(C => C.ChapterObject.Name == selectedChapter)
                                                    .Select(C => C.ChapterObject.Id).SingleOrDefault());
            }

            ListChapterLoaded = true;
            StateHasChanged();
        }

        private class Chapter
        {
            public string CaseTypeGroup { get; set; }
            public string CaseType { get; set; }
            public string Name { get; set; }
            public int SeqNo { get; set; }
            public List<UsrOrDefChapterManagement> ChapterItems { get; set; }

        }
        

        private void GetItemListByChapter(int chapterID)
        {

            string chapterData = lstChapters
                                .Where(A => A.ChapterObject.Id == chapterID)
                                .Select(A => A.ChapterObject.ChapterData)
                                .SingleOrDefault();

            chapterData = "{ 'CaseTypeGroup': 'OR Debt','CaseType': 'Debt - Commercial','Name':'Winding Up Proceedings','SeqNo': 1,'ChapterItems': [{ 'Type':'Agenda','Name':'WUP','SeqNo':0,'AltDisplayName':'','SuppressStep':'','EntityType':'','AsName':'','CompleteName':'','RescheduleDays':0,'NextStatus':''},{ 'Type':'Agenda','Name':'Insolvency','SeqNo':0,'AltDisplayName':'','SuppressStep':'','EntityType':'','AsName':'','CompleteName':'','RescheduleDays':0,'NextStatus':''},{ 'Type':'Status','Name':'In Progress','SeqNo':1,'AltDisplayName':'','SuppressStep':'','EntityType':'','AsName':'','CompleteName':'','RescheduleDays':0,'NextStatus':''},{ 'Type':'Status','Name':'Withdrawn','SeqNo':2,'AltDisplayName':'','SuppressStep':'Y','EntityType':'','AsName':'','CompleteName':'','RescheduleDays':0,'NextStatus':''},{ 'Type':'Status','Name':'Concluded','SeqNo':3,'AltDisplayName':'','SuppressStep':'Y','EntityType':'','AsName':'','CompleteName':'','RescheduleDays':0,'NextStatus':''},{ 'Type':'Letter','Name':'Blank LTR to Agent','SeqNo':1,'AltDisplayName':'Blank Letter to the Agent','SuppressStep':'','EntityType':'Agent','AsName':'','CompleteName':'','RescheduleDays':0,'NextStatus':''},{ 'Type':'Letter','Name':'Blank LTR to Debtor','SeqNo':2,'AltDisplayName':'Blank Letter to the Debtor','SuppressStep':'','EntityType':'Debtor','AsName':'','CompleteName':'','RescheduleDays':0,'NextStatus':''},{ 'Type':'Letter','Name':'Blank LTR to Offical Receiver','SeqNo':3,'AltDisplayName':'Blank Letter to the Official Receiver','SuppressStep':'','EntityType':'Offical Receiver','AsName':'','CompleteName':'','RescheduleDays':0,'NextStatus':''},{ 'Type':'Letter','Name':'Blank LTR to Client','SeqNo':4,'AltDisplayName':'Blank Letter to the Client','SuppressStep':'','EntityType':'Client','AsName':'','CompleteName':'','RescheduleDays':0,'NextStatus':''},{ 'Type':'Letter','Name':'Blank LTR to Court','SeqNo':5,'AltDisplayName':'Blank Letter to the Court','SuppressStep':'','EntityType':'Court','AsName':'','CompleteName':'','RescheduleDays':0,'NextStatus':''},{ 'Type':'Letter','Name':'LTR to Debtor - WUP Short Form Demand','SeqNo':6,'AltDisplayName':'Letter to Debtor - Winding Up Proceedings Short Form Demand','SuppressStep':'','EntityType':'Debtor','AsName':'FS Awaiting Response from Debtor to Short Form Demand','CompleteName':'FS SHORT FORM DEMAND SENT TO DEBTOR','RescheduleDays':14,'NextStatus':'In Progress'},{ 'Type':'Form','Name':'FRM Statutory Demand','SeqNo':7,'AltDisplayName':'','SuppressStep':'','EntityType':'','AsName':'','CompleteName':'','RescheduleDays':0,'NextStatus':''},{ 'Type':'Letter','Name':'LTR to Agent - Enc Statutory Demand for service','SeqNo':8,'AltDisplayName':'','SuppressStep':'','EntityType':'Agent','AsName':'FS Awaiting Certificate of Service of Stat Demand','CompleteName':'FS STATUTORY DEMAND SENT FOR SERVICE','RescheduleDays':14,'NextStatus':''},{ 'Type':'Letter','Name':'LTR to Process Server - Chasing Certificate of Service\r\n','SeqNo':9,'AltDisplayName':'','SuppressStep':'','EntityType':'Process Server','AsName':'FS Awaiting Certificate of Service of Stat Demand','CompleteName':'FS CHASED PROCESS SERVER FOR CERTIFICTAE OF SERVICE','RescheduleDays':7,'NextStatus':''},{ 'Type':'Doc','Name':'DOC WUP Petition','SeqNo':10,'AltDisplayName':'','SuppressStep':'','EntityType':'','AsName':'','CompleteName':'','RescheduleDays':0,'NextStatus':''},{ 'Type':'Doc','Name':'DOC WUP Witness Statement','SeqNo':11,'AltDisplayName':'','SuppressStep':'','EntityType':'','AsName':'','CompleteName':'','RescheduleDays':0,'NextStatus':''},{ 'Type':'Letter','Name':'LTR to Court - Enclosing Winding Up Petition','SeqNo':12,'AltDisplayName':'','SuppressStep':'','EntityType':'Court','AsName':'FS Awaiting Sealed WuP from Court','CompleteName':'FS WINDING UP PETITION SENT TO COURT','RescheduleDays':14,'NextStatus':''},{ 'Type':'Letter','Name':'LTR to Court - Chasing Sealed WUP Petiton','SeqNo':13,'AltDisplayName':'','SuppressStep':'','EntityType':'Court','AsName':'FS Awaiting Sealed WuP from Court','CompleteName':'FS CHASED COURT FOR SEALED WINDING UP PRETITION','RescheduleDays':7,'NextStatus':''},{ 'Type':'Letter','Name':'LTR to Client - Informing of WUP Hearing Date','SeqNo':14,'AltDisplayName':'','SuppressStep':'','EntityType':'Client','AsName':'','CompleteName':'','RescheduleDays':0,'NextStatus':''},{ 'Type':'Letter','Name':'LTR to Agent - Enclosing WUP for Service','SeqNo':15,'AltDisplayName':'','SuppressStep':'','EntityType':'Agent','AsName':'FS Awaiting Affidavit of Service of WuP','CompleteName':'FS WINDING UP PRETITON SENT FOR SERVICE','RescheduleDays':14,'NextStatus':''},{ 'Type':'Letter','Name':'LTR to Process Server - Chasing Affidavit of Service','SeqNo':16,'AltDisplayName':'','SuppressStep':'','EntityType':'Process Server','AsName':'FS Awaiting Affidavit of Service of WuP','CompleteName':'FS CHASED PROCESS SERVER FOR AFFIDAVIT OF SERVICE','RescheduleDays':7,'NextStatus':''},{ 'Type':'Letter','Name':'LTR to London Gazette - Enc Advertisment WUP','SeqNo':17,'AltDisplayName':'','SuppressStep':'','EntityType':'London Gazette','AsName':'','CompleteName':'FS GAZETTE ADVERTISEMENT LODGED','RescheduleDays':0,'NextStatus':''},{ 'Type':'Doc','Name':'DOC London Gazette Advertisment WUP','SeqNo':18,'AltDisplayName':'','SuppressStep':'','EntityType':'','AsName':'','CompleteName':'','RescheduleDays':0,'NextStatus':''},{ 'Type':'Letter','Name':'LTR to Court - Enc Certificate of Service','SeqNo':19,'AltDisplayName':'','SuppressStep':'','EntityType':'Court','AsName':'','CompleteName':'FS CERTIFICATE OF SERVICE SENT TO COURT','RescheduleDays':0,'NextStatus':''},{ 'Type':'Letter','Name':'LTR to Agent - To Attend WUP Hearing','SeqNo':20,'AltDisplayName':'','SuppressStep':'','EntityType':'Agent','AsName':'FS Awaiting Hearing Attendance Confirmation from Agenr','CompleteName':'FS AGENT INSTRUCTED TO ATTEND WUP HEARING','RescheduleDays':7,'NextStatus':''},{ 'Type':'Doc','Name':'DOC List of Creditors Intending to Appear (WUP)','SeqNo':21,'AltDisplayName':'','SuppressStep':'','EntityType':'','AsName':'','CompleteName':'','RescheduleDays':0,'NextStatus':''},{ 'Type':'Letter','Name':'LTR to Court - Enc Certificate of Compliance','SeqNo':22,'AltDisplayName':'','SuppressStep':'','EntityType':'Court','AsName':'','CompleteName':'FS CERTIFICATE OF COMPLIANCE SENT TO COURT','RescheduleDays':0,'NextStatus':''},{ 'Type':'Doc','Name':'DOC Certificate of Compliance (WUP)','SeqNo':23,'AltDisplayName':'','SuppressStep':'','EntityType':'','AsName':'','CompleteName':'','RescheduleDays':0,'NextStatus':''},{ 'Type':'Letter','Name':'LTR to Agent - Chasing WUP Hearing Result','SeqNo':24,'AltDisplayName':'','SuppressStep':'','EntityType':'Agent','AsName':'FS Awaiting WUP Hearing Result from Agent','CompleteName':'FS CHASED AGENT FOR WUP HEARING RESULT','RescheduleDays':7,'NextStatus':''},{ 'Type':'Letter','Name':'LTR to Client - WUP Order Granted','SeqNo':25,'AltDisplayName':'','SuppressStep':'Y','EntityType':'Client','AsName':'','CompleteName':'FS CLIENT INFORMED WUP ORDER GRANTED','RescheduleDays':0,'NextStatus':'Concluded'},{ 'Type':'Fee','Name':'Cost for Statutory Demand','SeqNo':1,'AltDisplayName':'','SuppressStep':'','EntityType':'','AsName':'','CompleteName':'','RescheduleDays':0,'NextStatus':''},{ 'Type':'Fee','Name':'Cost for Short Form Demand','SeqNo':2,'AltDisplayName':'','SuppressStep':'','EntityType':'','AsName':'','CompleteName':'','RescheduleDays':0,'NextStatus':''},{ 'Type':'Fee','Name':'Agents Fees Process Server','SeqNo':3,'AltDisplayName':'','SuppressStep':'','EntityType':'','AsName':'','CompleteName':'','RescheduleDays':0,'NextStatus':''},{ 'Type':'Fee','Name':'Court Issue Fee for Winding up Petition','SeqNo':4,'AltDisplayName':'','SuppressStep':'','EntityType':'','AsName':'','CompleteName':'','RescheduleDays':0,'NextStatus':''},{ 'Type':'Fee','Name':'Winding Up Petition Deposit','SeqNo':5,'AltDisplayName':'','SuppressStep':'','EntityType':'','AsName':'','CompleteName':'','RescheduleDays':0,'NextStatus':''},{ 'Type':'Fee','Name':'Cost for Winding Up Petition','SeqNo':6,'AltDisplayName':'','SuppressStep':'','EntityType':'','AsName':'','CompleteName':'','RescheduleDays':0,'NextStatus':''},{ 'Type':'Fee','Name':'London Gazette Advertisement Fee','SeqNo':7,'AltDisplayName':'','SuppressStep':'','EntityType':'','AsName':'','CompleteName':'','RescheduleDays':0,'NextStatus':''},{ 'Type':'Fee','Name':'Agents Fees to Attend Hearing','SeqNo':8,'AltDisplayName':'','SuppressStep':'','EntityType':'','AsName':'','CompleteName':'','RescheduleDays':0,'NextStatus':''}]}";

            Chapter chapter = JsonConvert.DeserializeObject<Chapter>(chapterData);

            string ctg = chapter.CaseTypeGroup;

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
                GetItemListByChapter(selectedChapterId);

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

                var chapter = await chapterManagementService.GetItemListByChapterName(selectedCaseTypeGroup, selectedCaseType, selectedChapter);
                altSysSelectedChapterId = chapter
                                                .Select(A => A.Id)
                                                .FirstOrDefault();

                if (altSysSelectedChapterId == 0)
                {
                    return false;
                }

                var temp = await chapterManagementService.GetItemListByChapter(altSysSelectedChapterId.Value);

                if (!(altSysSelectedChapterId is null))
                {
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


            }

            return true;
        }

        private VmUsrOrDefChapterManagement CompareChapterItemsToAltSytem(VmUsrOrDefChapterManagement chapterItem)
        {
            var altObject = lstAltSystemChapterItems
                                .Where(A => A.ChapterObject.Name == chapterItem.ChapterObject.Name)
                                .SingleOrDefault();

            if (altObject is null)
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

            if (type == "Steps and Documents")
            {
                editObject.ChapterObject.SeqNo = lstDocs
                                                    .OrderByDescending(A => A.ChapterObject.SeqNo)
                                                    .Select(A => A.ChapterObject.SeqNo)
                                                    .FirstOrDefault() + 1;
            }
            else if (type == "Status")
            {
                editObject.ChapterObject.SeqNo = lstStatus
                                                    .OrderByDescending(A => A.ChapterObject.SeqNo)
                                                    .Select(A => A.ChapterObject.SeqNo)
                                                    .FirstOrDefault() + 1;
            }
            else
            {
                editObject.ChapterObject.SeqNo = lstFees
                                    .OrderByDescending(A => A.ChapterObject.SeqNo)
                                    .Select(A => A.ChapterObject.SeqNo)
                                    .FirstOrDefault() + 1;
            }

            editChapterObject.ChapterObject.SeqNo = editChapterObject.ChapterObject.SeqNo is null
                                                        ? 0
                                                        : editChapterObject.ChapterObject.SeqNo;

            editObject.ChapterObject.ParentId = selectedChapterId;

            ShowChapterDetailModal();
        }

        private void PrepareChapterForInsert()
        {
            editChapterObject = new VmUsrOrDefChapterManagement { ChapterObject = new UsrOrDefChapterManagement() };

            if (!(selectedCaseTypeGroup == ""))
            {
                editChapterObject.ChapterObject.CaseTypeGroup = selectedCaseTypeGroup;
            }
            else
            {
                editChapterObject.ChapterObject.CaseTypeGroup = "";
            }

            if (!(selectedCaseType == ""))
            {
                editChapterObject.ChapterObject.CaseType = selectedCaseType;
            }
            else
            {
                editChapterObject.ChapterObject.CaseType = "";
            }

            if (!string.IsNullOrWhiteSpace(selectedCaseTypeGroup) & !string.IsNullOrWhiteSpace(selectedCaseType))
            {
                editChapterObject.ChapterObject.SeqNo = lstChapters
                                                            .Where(C => C.ChapterObject.ParentId == 0)
                                                            .Where(C => C.ChapterObject.CaseType == selectedCaseType)
                                                            .Where(C => C.ChapterObject.CaseTypeGroup == selectedCaseTypeGroup)
                                                            .OrderByDescending(C => C.ChapterObject.SeqNo)
                                                            .Select(C => C.ChapterObject.SeqNo)
                                                            .FirstOrDefault() + 1;
            }
            else
            {
                editChapterObject.ChapterObject.SeqNo = 1;
            }

            editChapterObject.ChapterObject.Type = "Chapter";
            editChapterObject.ChapterObject.ParentId = 0;

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
            if (!(selectedCaseType == ""))
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
            seqMoving = true; //prevents changes to the form whilst process of changing seq is carried out

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
                case "Status":
                    lstItems = lstStatus;
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
            StateHasChanged();

            seqMoving = false;

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
                case "Status":
                    listItems = lstStatus;
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


        public void RefreshSelectedList()
        {
            CondenseSeq(navDisplay);
        }

        protected async void CondenseSeq(string ListType)
        {
            await RefreshChapterItems(ListType);

            var ListItems = GetRelevantChapterList(ListType);

            int seqNo = 0;

            foreach (VmUsrOrDefChapterManagement item in ListItems.OrderBy(A => A.ChapterObject.SeqNo))
            {
                seqNo += 1;
                item.ChapterObject.SeqNo = seqNo;

                await chapterManagementService.Update(item.ChapterObject);
            }

            await RefreshChapterItems(ListType);
            StateHasChanged();
        }

        protected void CondenseFeeSeq()
        {
            CondenseSeq("Fees");
        }


        protected void ShowChapterAddOrEditModel()
        {
            Action Action = RefreshChapters;

            var parameters = new ModalParameters();
            parameters.Add("TaskObject", editChapterObject);
            parameters.Add("DataChanged", Action);
            parameters.Add("AllObjects", lstChapters);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-chapter-chapter"
            };

            Modal.Show<ChapterAddOrEdit>("Chapter", parameters, options);
        }

        protected void ShowCaseTypeEditModal()
        {
            Action Action = RefreshChapters;

            var parameters = new ModalParameters();
            parameters.Add("TaskObject", (isCaseTypeOrGroup == "Chapter") ? editChapter.Name : editCaseType);
            parameters.Add("originalName", (isCaseTypeOrGroup == "Chapter") ? editChapter.Name : editCaseType);
            if (isCaseTypeOrGroup == "Chapter")
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
            Action action = RefreshSelectedList;

            var parameters = new ModalParameters();
            parameters.Add("TaskObject", editObject.ChapterObject);
            parameters.Add("DataChanged", action);
            parameters.Add("selectedCaseType", selectedCaseType);
            parameters.Add("selectedList", selectedList);
            parameters.Add("dropDownChapterList", dropDownChapterList);
            parameters.Add("CaseTypeGroups", partnerCaseTypeGroups);
            parameters.Add("ListOfStatus", lstStatus);

            string className = "modal-chapter-item";

            if (selectedList == "Steps and Documents")
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
            Action RefreshFeeOrder = RefreshSelectedList;

            var parameters = new ModalParameters();
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

        protected void PrepareChapterDelete(VmUsrOrDefChapterManagement selectedChapterItem)
        {
            editObject = selectedChapterItem;

            Action SelectedDeleteAction = HandleChapterDelete;
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
            StateHasChanged();
        }

        private async void HandleChapterDelete()
        {
            await chapterManagementService.Delete(editObject.ChapterObject.Id);

            RefreshChapters();
            StateHasChanged();
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
            if (seqMoving == false | compareSystems == true)
            {
                List<VmUsrOrDefChapterManagement> listItems = GetRelevantChapterList(listType);

                bool isValid = true;

                for (int i = 0; i < listItems.Count; i++)
                {
                    if (listItems[i].ChapterObject.SeqNo != i + 1)
                    {
                        isValid = false;
                    }

                }

                return isValid;
            }
            else
            {
                return true;
            }

        }
    }
}
