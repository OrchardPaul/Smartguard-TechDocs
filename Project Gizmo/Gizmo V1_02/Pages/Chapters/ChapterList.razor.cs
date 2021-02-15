using GadjIT.ClientContext.P4W;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gizmo_V1_02.Services;
using System.Web;
using Gizmo_V1_02.Services.SessionState;
using Blazored.Modal.Services;
using Blazored.Modal;
using Gizmo_V1_02.Pages.Shared.Modals;
using GadjIT.ClientContext.P4W.Custom;
using GadjIT.ClientContext.P4W.Functions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Schema.Generation;
using Newtonsoft.Json.Schema;
using Microsoft.AspNetCore.Hosting;
using BlazorInputFile;

namespace Gizmo_V1_02.Pages.Chapters
{
    public partial class ChapterList
    {
        private class ChapterColour
        {
            public string ColourName { get; set; }
            public string ColourCode { get; set; }
        }

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

        [Inject]
        public IWebHostEnvironment env { get; set; }

        [Inject]
        private IChapterFileUpload ChapterFileUpload { get; set; }

        private List<VmUsrOrDefChapterManagement> lstChapters { get; set; } = new List<VmUsrOrDefChapterManagement>();

        private List<VmUsrOrDefChapterManagement> lstAltSystemChapters { get; set; } = new List<VmUsrOrDefChapterManagement>();

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
        public string updateJSON { get; set; } = "";

        public UsrOrDefChapterManagement editChapter { get; set; }
        public string isCaseTypeOrGroup { get; set; } = "";

        public VmUsrOrDefChapterManagement editObject = new VmUsrOrDefChapterManagement { ChapterObject = new UsrOrDefChapterManagement() };
        public VmUsrOrDefChapterManagement editChapterObject = new VmUsrOrDefChapterManagement { ChapterObject = new UsrOrDefChapterManagement() };


        string selectedList = string.Empty;

        string displaySection { get; set; } = "";

        [Parameter]
        public string UrlCaseTypeGroup { set { selectedChapter.CaseTypeGroup = value; } }

        [Parameter]
        public string UrlCaseType { set { selectedChapter.CaseType = value; } }

        [Parameter]
        public string UrlChapter { set { selectedChapter.Name = value; } }

        [Parameter]
        public VmChapter selectedChapter { get; set; } = new VmChapter { ChapterItems = new List<UsrOrDefChapterManagement>() };

        public VmChapter altChapter { get; set; } = new VmChapter();

        public UsrOrDefChapterManagement SelectedChapterObject { get; set; } = new UsrOrDefChapterManagement();

        public UsrOrDefChapterManagement AltChapterObject { get; set; } = new UsrOrDefChapterManagement();

        int rowChanged { get; set; } = 0;

        private int selectedChapterId { get; set; } = -1;

        private int? altSysSelectedChapterId { get; set; }


        public string ModalInfoHeader { get; set; }
        public string ModalInfoText { get; set; }
        public string ModalHeight { get; set; }
        public string ModalWidth { get; set; }

        public string navDisplay = "Chapter";

        private bool seqMoving = false;

        public bool compareSystems = false;

        private string RowChangedClass { get; set; } = "row-changed-nav3";


        public bool displaySpinner = true;

        public bool ListChapterLoaded = false;

        public string alertMsgJSOM { get; set; }
        
        public bool showJSON = false;


        public List<string> lstDocTypes { get; set; } = new List<string> { "Doc", "Letter", "Form", "Email", "Step" };



        private List<ChapterColour> ListChapterColours { get; set; } = new List<ChapterColour>
                                                            { 
                                                                new ChapterColour { ColourName = "Grey", ColourCode = "#3F000000"},
                                                                new ChapterColour { ColourName = "Blue", ColourCode = "#3F0074FF"},
                                                                new ChapterColour { ColourName = "Pink", ColourCode = "#3FFD64EF"},
                                                                new ChapterColour { ColourName = "Peach", ColourCode = "#3FEA9C66"},
                                                                new ChapterColour { ColourName = "Yellow", ColourCode = "#3FFFFF00"},
                                                                new ChapterColour { ColourName = "Beige", ColourCode = "#3F957625"},
                                                                new ChapterColour { ColourName = "Lilac", ColourCode = "#3F6E6FDB"},
                                                                new ChapterColour { ColourName = "Green", ColourCode = "#3F32EC29"},
                                                                new ChapterColour { ColourName = "Aqua", ColourCode = "#3F5BDCD0"}
                                                            };

            

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
            selectedChapter.Name = "";
            rowChanged = 0;
        }

        void SelectCaseTypeGroup(string caseTypeGroup)
        {
            selectedChapter.CaseTypeGroup = (selectedChapter.CaseTypeGroup == caseTypeGroup) ? "" : caseTypeGroup;
            selectedChapter.CaseType = "";
            selectedChapter.Name = "";
        }

        void SelectCaseType(string caseType)
        {
            selectedChapter.CaseType = (selectedChapter.CaseType == caseType) ? "" : caseType;
            selectedChapter.Name = "";
            PrepChapterList();
        }

        private async void SelectChapter(UsrOrDefChapterManagement chapter)
        {
            displaySpinner = true;

            lstAll = new List<VmUsrOrDefChapterManagement>();

            SelectedChapterObject = chapter;

            if (!(chapter.ChapterData is null))
            {
                selectedChapter = JsonConvert.DeserializeObject<VmChapter>(chapter.ChapterData);
            }
            else
            {
                //Initialise the VmChapter in case of null Json
                selectedChapter = new VmChapter { ChapterItems = new List<UsrOrDefChapterManagement>() };
                selectedChapter.CaseTypeGroup = chapter.CaseTypeGroup;
                selectedChapter.CaseType = chapter.CaseType;
                selectedChapter.Name = chapter.Name;
            }

            selectedChapterId = chapter.Id;
            compareSystems = false;
            rowChanged = 0;
            navDisplay = "Chapter";

            feeDefinitions = await chapterManagementService.GetFeeDefs(selectedChapter.CaseTypeGroup, selectedChapter.CaseType);

            await RefreshChapterItems("All");

            StateHasChanged();
        }

        private async void SaveChapterDetails()
        {
            SelectedChapterObject.Name = selectedChapter.Name;

            SelectedChapterObject.ChapterData = JsonConvert.SerializeObject(selectedChapter);
            await chapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);

            await RefreshChapterItems("All");
            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }

        


        private async void RefreshChapters()
        {
            ListChapterLoaded = false;

            var lstC = await chapterManagementService.GetAllChapters();
            lstChapters = lstC.Select(A => new VmUsrOrDefChapterManagement { ChapterObject = A }).ToList();

            if (!(selectedChapter.Name is null) & selectedChapter.Name != "")
            {
                SelectChapter(lstChapters
                                    .Where(C => C.ChapterObject.CaseTypeGroup == selectedChapter.CaseTypeGroup)
                                    .Where(C => C.ChapterObject.CaseType == selectedChapter.CaseType)
                                    .Where(C => C.ChapterObject.Name == selectedChapter.Name)
                                    .Select(C => C.ChapterObject)
                                    .SingleOrDefault());
            }

            ListChapterLoaded = true;
            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
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
                var lst = selectedChapter.ChapterItems;

                lstAll = lst.Select(L => new VmUsrOrDefChapterManagement { ChapterObject = L })
                                .Select(L => { L.ChapterObject.CaseTypeGroup = selectedChapter.CaseTypeGroup;
                                                L.ChapterObject.CaseType = selectedChapter.CaseType;
                                                L.ChapterObject.ChapterName = selectedChapter.Name;
                                                return L; })
                                .ToList();

                /*
                 * listType = All when chapter is selected, 
                 * listType = nav selected e.g. Agenda when and object in a specific list has been altered
                 * 
                 */
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
                await CompareSelectedChapterToAltSystem();
            }
        }

        private async Task<bool> RefreshAltSystemChaptersList()
        {
            try
            {
                await sessionState.SwitchSelectedSystem();

                var lstC = await chapterManagementService.GetAllChapters();
                lstAltSystemChapters = lstC.Select(A => new VmUsrOrDefChapterManagement { ChapterObject = A }).ToList();

                await sessionState.ResetSelectedSystem();

                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> CompareSelectedChapterToAltSystem()
        {

            if (compareSystems)
            {
                var test = await RefreshChapterItems(navDisplay);

                await RefreshAltSystemChaptersList();

                AltChapterObject = lstAltSystemChapters
                                        .Where(A => A.ChapterObject.Name == SelectedChapterObject.Name)
                                        .Where(A => A.ChapterObject.CaseType == SelectedChapterObject.CaseType)
                                        .Where(A => A.ChapterObject.CaseTypeGroup == SelectedChapterObject.CaseTypeGroup)
                                        .Select(C => C.ChapterObject)
                                        .SingleOrDefault();

                if(!(AltChapterObject is null))
                {
                    altChapter = JsonConvert.DeserializeObject<VmChapter>(AltChapterObject.ChapterData);

                    var temp = altChapter.ChapterItems;


                    lstAltSystemChapterItems = temp.Select(T => new VmUsrOrDefChapterManagement { ChapterObject = T }).ToList();

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

                    await InvokeAsync(() =>
                    {
                        StateHasChanged();
                    });
                }

                

            }

            return true;
        }

        private async void CompareAllChapters()
        {
            lstChapters = lstChapters.Select(C => { C.ComparisonIcon = null; C.ComparisonResult = null; return C; }).ToList();

            var test = new string(SelectedChapterObject.ChapterData);

            lstAltSystemChapters = new List<VmUsrOrDefChapterManagement>();
            AltChapterObject = new UsrOrDefChapterManagement();
            compareSystems = !compareSystems;
            await RefreshCompararisonAllChapters();


            await InvokeAsync(() =>
            {
                StateHasChanged();
            });

        }

        private async Task<bool> RefreshCompararisonAllChapters()
        {
            if (compareSystems)
            {
                await RefreshAltSystemChaptersList();

                /*
                 * for every chapter get list of chapter items from both current system and alt system
                 * if any result returns false
                 * 
                 * 
                 */
                foreach (var chapter in lstChapters)
                {
                    var chapterItems = JsonConvert.DeserializeObject<VmChapter>(chapter.ChapterObject.ChapterData);

                    var vmChapterItems = chapterItems.ChapterItems.Select(C => new VmUsrOrDefChapterManagement { ChapterObject = C }).ToList();

                    AltChapterObject = lstAltSystemChapters
                                        .Where(A => A.ChapterObject.Name == chapter.ChapterObject.Name)
                                        .Where(A => A.ChapterObject.CaseType == chapter.ChapterObject.CaseType)
                                        .Where(A => A.ChapterObject.CaseTypeGroup == chapter.ChapterObject.CaseTypeGroup)
                                        .Select(C => C.ChapterObject)
                                        .SingleOrDefault();

                    if(AltChapterObject is null)
                    {
                        chapter.ComparisonResult = "No match";
                        chapter.ComparisonIcon = "times";
                    }
                    else
                    {
                        altChapter = JsonConvert.DeserializeObject<VmChapter>(AltChapterObject.ChapterData);

                        lstAltSystemChapterItems = altChapter.ChapterItems.Select(T => new VmUsrOrDefChapterManagement { ChapterObject = T }).ToList();

                        foreach(var item in vmChapterItems)
                        {
                            CompareChapterItemsToAltSytem(item);
                        }

                        if(vmChapterItems.Where(C => C.ComparisonResult == "No match" | C.ComparisonResult == "Partial match").Count() > 0 | vmChapterItems.Count() != lstAltSystemChapterItems.Count())
                        {
                            chapter.ComparisonResult = "Partial match";
                            chapter.ComparisonIcon = "exclamation";
                        }
                        else
                        {
                            chapter.ComparisonResult = "Exact match";
                            chapter.ComparisonIcon = "check";
                        }
                    }
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
            await CompareSelectedChapterToAltSystem();
        }

        private void PrepareForExport(List<VmUsrOrDefChapterManagement> items, string header)
        {
            var parameters = new ModalParameters();
            parameters.Add("lstChapterItems", items);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-chapter-export"
            };

            Modal.Show<ChapterExport>("Chapter Export", parameters, options);
        }

        private void PrepareForEdit(VmUsrOrDefChapterManagement item, string header)
        {
            selectedList = header;
            editObject = item;

            ShowChapterDetailModal("Edit");
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

            editObject.ChapterObject.SeqNo = editObject.ChapterObject.SeqNo is null
                                                        ? 1
                                                        : editObject.ChapterObject.SeqNo;

            editObject.ChapterObject.ParentId = selectedChapterId;

            ShowChapterDetailModal("Insert");
        }

        private void PrepNewChapter()
        {
            editChapterObject = new VmUsrOrDefChapterManagement { ChapterObject = new UsrOrDefChapterManagement() };

            if (!(selectedChapter.CaseTypeGroup == ""))
            {
                editChapterObject.ChapterObject.CaseTypeGroup = selectedChapter.CaseTypeGroup;
            }
            else
            {
                editChapterObject.ChapterObject.CaseTypeGroup = "";
            }

            if (!(selectedChapter.CaseType == ""))
            {
                editChapterObject.ChapterObject.CaseType = selectedChapter.CaseType;
            }
            else
            {
                editChapterObject.ChapterObject.CaseType = "";
            }

            if (!string.IsNullOrWhiteSpace(selectedChapter.CaseTypeGroup) & !string.IsNullOrWhiteSpace(selectedChapter.CaseType))
            {
                editChapterObject.ChapterObject.SeqNo = lstChapters
                                                            .Where(C => C.ChapterObject.ParentId == 0)
                                                            .Where(C => C.ChapterObject.CaseType == selectedChapter.CaseType)
                                                            .Where(C => C.ChapterObject.CaseTypeGroup == selectedChapter.CaseTypeGroup)
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
        }

        private void PrepareChapterForInsert()
        {
            PrepNewChapter();
            ShowChapterAddOrEditModel();
        }

        private void PrepareChapterForCopy()
        {
            var test = Uri.EscapeDataString(SelectedChapterObject.ChapterData);

            PrepNewChapter();
            ShowChapterCopyModel();
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
            if (!(selectedChapter.CaseType == ""))
            {
                dropDownChapterList = await chapterManagementService.GetDocumentList(selectedChapter.CaseType);
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
                                        .Where(A => A.ChapterObject.CaseTypeGroup == selectedChapter.CaseTypeGroup)
                                        .Where(A => A.ChapterObject.CaseType == selectedChapter.CaseType)
                                        .OrderBy(A => A.ChapterObject.SeqNo)
                                        .ToList();
                    break;
            }

            var swapItem = lstItems.Where(D => D.ChapterObject.SeqNo == (selectobject.SeqNo + incrementBy)).SingleOrDefault();
            if (!(swapItem is null))
            {
                selectobject.SeqNo += incrementBy;
                swapItem.ChapterObject.SeqNo = swapItem.ChapterObject.SeqNo + (incrementBy * -1);

                if(listType == "Chapters")
                {
                    await chapterManagementService.Update(selectobject).ConfigureAwait(false);
                    await chapterManagementService.Update(swapItem.ChapterObject).ConfigureAwait(false);
                }
                else
                {
                    SelectedChapterObject.ChapterData = JsonConvert.SerializeObject(selectedChapter);
                    await chapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);
                }

                
            }

            await RefreshChapterItems(listType);
            await InvokeAsync(() =>
            {
                StateHasChanged();
            }); 
            

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
                                        .Where(A => A.ChapterObject.CaseTypeGroup == selectedChapter.CaseTypeGroup)
                                        .Where(A => A.ChapterObject.CaseType == selectedChapter.CaseType)
                                        .OrderBy(A => A.ChapterObject.SeqNo)
                                        .ToList();
                    break;
            }

            return listItems;
        }


        public async void RefreshSelectedList()
        {
            await RefreshChapterItems("All");
            //CondenseSeq(navDisplay);

            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
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
            }

            SelectedChapterObject.ChapterData = JsonConvert.SerializeObject(selectedChapter);
            await chapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);

            await RefreshChapterItems(ListType);

            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }

        protected async void CondenseChapterSeq()
        {
            var ListItems = GetRelevantChapterList("Chapters");

            int seqNo = 0;

            foreach (VmUsrOrDefChapterManagement item in ListItems.OrderBy(A => A.ChapterObject.SeqNo))
            {
                seqNo += 1;
                item.ChapterObject.SeqNo = seqNo;

                await chapterManagementService.Update(item.ChapterObject);
            }

            RefreshChapters();
        }


        protected void CondenseFeeSeq()
        {
            CondenseSeq("Fees");
        }

        protected void ShowChapterCopyModel()
        {
            Action Action = RefreshChapters;

            var parameters = new ModalParameters();
            parameters.Add("TaskObject", editChapterObject);
            parameters.Add("AllChapters", lstChapters);
            parameters.Add("currentChapter", selectedChapter);
            parameters.Add("DataChanged", Action);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-chapter-chapter"
            };

            Modal.Show<ChapterCopy>("Copy Chapter", parameters, options);
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

        

        private void ShowUpdateJSON()
        {
            updateJSON = SelectedChapterObject.ChapterData;

            alertMsgJSOM = "";

            showJSON = true;

            StateHasChanged();
        }

        private void CloseUpdateJSON()
        {
            showJSON = false;

            StateHasChanged();
        }


        private async void SaveJsonFromDirectInput()
        {
            alertMsgJSOM = "";
            var IsJsonValid = false;

            JSchemaGenerator generator = new JSchemaGenerator();
            try
            {
                JSchema schema = generator.Generate(typeof(VmChapter));
                JObject jObject = JObject.Parse(updateJSON);

                IsJsonValid = jObject.IsValid(schema);

            }
            catch
            {
                IsJsonValid = false;
            }

            if (IsJsonValid)
            {
                var chapterData = JsonConvert.DeserializeObject<VmChapter>(updateJSON);
                selectedChapter.ChapterItems = chapterData.ChapterItems;
                SelectedChapterObject.ChapterData = JsonConvert.SerializeObject(selectedChapter);

                await chapterManagementService.Update(SelectedChapterObject);

                SelectChapter(SelectedChapterObject);

                showJSON = false;

                await InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            }
            else
            {
                showJSON = false;
            }

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
            parameters.Add("caseTypeGroupName", selectedChapter.CaseTypeGroup);
            parameters.Add("ListChapters", lstChapters);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-chapter-casetype"
            };

            Modal.Show<ChapterCaseTypeEdit>("Chapter", parameters, options);
        }


        protected void ShowChapterDetailModal(string option)
        {
            Action action = RefreshSelectedList;

            var copyObject = new UsrOrDefChapterManagement
                                {
                                    Type = editObject.ChapterObject.Type,
                                    Name = editObject.ChapterObject.Name,
                                    EntityType = editObject.ChapterObject.EntityType,
                                    SeqNo = editObject.ChapterObject.SeqNo,
                                    SuppressStep = editObject.ChapterObject.SuppressStep,
                                    CompleteName = editObject.ChapterObject.CompleteName,
                                    AsName = editObject.ChapterObject.AsName,
                                    RescheduleDays = editObject.ChapterObject.RescheduleDays,
                                    AltDisplayName = editObject.ChapterObject.AltDisplayName,
                                    UserMessage = editObject.ChapterObject.UserMessage,
                                    UserNotes = editObject.ChapterObject.UserNotes,
                                    NextStatus = editObject.ChapterObject.NextStatus
                                };
         
            var parameters = new ModalParameters();
            parameters.Add("TaskObject", editObject.ChapterObject);
            parameters.Add("CopyObject", copyObject);
            parameters.Add("DataChanged", action);
            parameters.Add("selectedList", selectedList);
            parameters.Add("dropDownChapterList", dropDownChapterList);
            parameters.Add("CaseTypeGroups", partnerCaseTypeGroups);
            parameters.Add("ListOfStatus", lstStatus);
            parameters.Add("SelectedChapter", selectedChapter);
            parameters.Add("SelectedChapterObject", SelectedChapterObject);
            parameters.Add("Option", option);

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
            Action RefreshFeeOrder = CondenseFeeSeq;

            var parameters = new ModalParameters();
            parameters.Add("RefreshFeeOrder", RefreshFeeOrder);
            parameters.Add("feeItems", lstVmFeeModalItems);
            parameters.Add("SeletedChapterId", selectedChapterId);
            parameters.Add("SelectedChapter", selectedChapter);
            parameters.Add("SelectedChapterObject", SelectedChapterObject);

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
            parameters.Add("CurrentChapter", selectedChapter);
            parameters.Add("AltChapter", altChapter);
            parameters.Add("CurrentChapterRow", SelectedChapterObject);
            parameters.Add("AltChapterRow", AltChapterObject);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-chapter-comparison"
            };

            Modal.Show<ChapterItemComparison>("Synchronise Chapter Item", parameters, options);
        }

        

        private async void HandleChapterDetailDelete()
        {

            selectedChapter.ChapterItems.Remove(editObject.ChapterObject);
            SelectedChapterObject.ChapterData = JsonConvert.SerializeObject(selectedChapter);
            await chapterManagementService.Update(SelectedChapterObject);

            await RefreshChapterItems(navDisplay);
            StateHasChanged();
        }

        private async void HandleChapterDelete()
        {
            await chapterManagementService.Delete(editObject.ChapterObject.Id);

            RefreshChapters();
            StateHasChanged();
        }


        protected void PrepareChapterSync()
        {
            string infoText;

            switch (navDisplay)
            {
                case "Chapter":
                    infoText = $"Make the {(sessionState.selectedSystem == "Live" ? "Dev" : "Live")} system the same as {sessionState.selectedSystem} for all chapter items.";
                    break;
                default:
                    infoText = $"Make the {(sessionState.selectedSystem == "Live" ? "Dev" : "Live")} system the same as {sessionState.selectedSystem} for all {navDisplay}.";
                    break;
            }

            Action SelectedAction = HandleChapterSync;
            var parameters = new ModalParameters();
            parameters.Add("InfoHeader", "Confirm Action");
            parameters.Add("InfoText", infoText);
            parameters.Add("ConfirmAction", SelectedAction);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-confirm"
            };

            Modal.Show<ModalConfirm>("Confirm", parameters, options);
        }

        private async void CreateSelectedChapterOnAlt()
        {
            await sessionState.SwitchSelectedSystem();

            AltChapterObject = lstAltSystemChapters
                                        .Where(A => A.ChapterObject.Name == SelectedChapterObject.Name)
                                        .Where(A => A.ChapterObject.CaseType == SelectedChapterObject.CaseType)
                                        .Where(A => A.ChapterObject.CaseTypeGroup == SelectedChapterObject.CaseTypeGroup)
                                        .Select(C => C.ChapterObject)
                                        .SingleOrDefault();

            if(AltChapterObject is null)
            {
                var newAltChapterObject = new UsrOrDefChapterManagement 
                                                    { 
                                                        SeqNo = SelectedChapterObject.SeqNo,
                                                        CaseTypeGroup = SelectedChapterObject.CaseTypeGroup,
                                                        CaseType = SelectedChapterObject.CaseType,
                                                        Name = SelectedChapterObject.Name,
                                                        ChapterData = SelectedChapterObject.ChapterData,
                                                        ParentId = SelectedChapterObject.ParentId,
                                                        Type = SelectedChapterObject.Type
                                                    };

                await chapterManagementService.Add(newAltChapterObject);
            }
            else
            {
                AltChapterObject.ChapterData = SelectedChapterObject.ChapterData;

                await chapterManagementService.Update(AltChapterObject);
            }

            

            await sessionState.ResetSelectedSystem();

            await RefreshCompararisonAllChapters();

            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }


        protected void PrepareChapterCreateOnAlt(UsrOrDefChapterManagement selectedChapter)
        {
            SelectedChapterObject = selectedChapter;

            string infoText = $"Do you wish to sync this chapter to {(sessionState.selectedSystem == "Live" ? "Dev" : "Live")}.";

            Action SelectedAction = CreateSelectedChapterOnAlt;
            var parameters = new ModalParameters();
            parameters.Add("InfoHeader", "Confirm Action");
            parameters.Add("InfoText", infoText);
            parameters.Add("ConfirmAction", SelectedAction);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-confirm"
            };

            Modal.Show<ModalConfirm>("Confirm", parameters, options);
        }

        private void HandleChapterSync()
        {
            if(navDisplay.ToLower() == "chapter")
            {
                SyncAll();
            }
            else
            {
                SyncToAltSystem(navDisplay);
            }

        }

        private List<IFileListEntry> filesJSON = new List<IFileListEntry>();


        private void HandleJSONFileSelection(IFileListEntry[] entryFiles)
        {
            var files = new List<IFileListEntry>();
            foreach (var file in entryFiles)
            {
                if (file != null)
                {   
                    ChapterFileUpload.UploadChapterFiles(file);
                    files.Add(file);
                }

            }
            if(files != null && files.Count > 0)
            {
                filesJSON = files;
                StateHasChanged();
            }
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

        /// <summary>
        /// moves the AA (transparancy) element of an android hex color to the end of the string
        /// XAML Forms use Hex color but in format #aarrggbb
        /// HTML hex is in format #rrggbbaa
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns>string: row-changed or row-changedx</returns>
        public string getHTMLColourFromAndroid(string colAndroid)
        {
            string colHTML = "";

            if (!string.IsNullOrEmpty(colAndroid) && (Regex.IsMatch(colAndroid, "^#(?:[0-9a-fA-F]{8})$")))
            {
                colHTML = "#" + colAndroid.Substring(3, 6) + colAndroid.Substring(1, 2);
            }
            else
            {
                colHTML = "#FFFFFFFF";
            }

            return colHTML;
        }


        private async void SyncAll()
        {
            compareSystems = true;
            await CompareSelectedChapterToAltSystem();
            compareSystems = false;
            SyncToAltSystem("All");
        }


        private async void SyncToAltSystem(string option)
        {
            var selectedCopyItems = new VmChapter { ChapterItems = new List<UsrOrDefChapterManagement>() };           

            if (!(AltChapterObject.ChapterData is null))
            {
                selectedCopyItems = JsonConvert.DeserializeObject<VmChapter>(AltChapterObject.ChapterData);
            }


            if (option == "Agenda" | option == "All")
            {
                foreach (var item in selectedCopyItems.ChapterItems.Where(C => C.Type == "Agenda").ToList())
                {
                    selectedCopyItems.ChapterItems.Remove(item);
                }

                selectedCopyItems.ChapterItems.AddRange(selectedChapter.ChapterItems.Where(C => C.Type == "Agenda").ToList());
            }

            

            if (option == "Status" | option == "All")
            {
                foreach (var item in selectedCopyItems.ChapterItems.Where(C => C.Type == "Status").ToList())
                {
                    selectedCopyItems.ChapterItems.Remove(item);
                }

                selectedCopyItems.ChapterItems.AddRange(selectedChapter.ChapterItems.Where(C => C.Type == "Status").ToList());
            }

            

            if (option == "Docs" | option == "All")
            {
                foreach (var item in selectedCopyItems.ChapterItems.Where(C => lstDocTypes.Contains(C.Type)).ToList())
                {
                    selectedCopyItems.ChapterItems.Remove(item);
                }

                selectedCopyItems.ChapterItems.AddRange(selectedChapter.ChapterItems.Where(C => lstDocTypes.Contains(C.Type)).ToList());
            }


            if (option == "Fees" | option == "All")
            {
                foreach (var item in selectedCopyItems.ChapterItems.Where(C => C.Type == "Fee").ToList())
                {
                    selectedCopyItems.ChapterItems.Remove(item);
                }

                selectedCopyItems.ChapterItems.AddRange(selectedChapter.ChapterItems.Where(C => C.Type == "Fee").ToList());
            }

            AltChapterObject.ChapterData = JsonConvert.SerializeObject(new VmChapter
            {
                CaseTypeGroup = AltChapterObject.CaseTypeGroup,
                CaseType = AltChapterObject.CaseType,
                Name = AltChapterObject.Name,
                SeqNo = AltChapterObject.SeqNo.GetValueOrDefault(),
                ChapterItems = selectedCopyItems.ChapterItems
            });

            await sessionState.SwitchSelectedSystem();
            
            if (AltChapterObject.Id == 0)
            {
                await chapterManagementService.Add(AltChapterObject);
            }
            else
            {
                await chapterManagementService.Update(AltChapterObject);
            }

            await sessionState.ResetSelectedSystem();

            await CompareSelectedChapterToAltSystem();
            StateHasChanged();
        }
    }
}
