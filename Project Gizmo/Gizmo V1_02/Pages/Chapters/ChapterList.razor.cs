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
using System.IO;
using Gizmo_V1_02.FileManagement.FileClassObjects.FileOptions;
using Gizmo_V1_02.FileManagement.FileClassObjects;
using System.Net;
using Microsoft.JSInterop;

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

        private ChapterFileOptions ChapterFileOption { get; set; }

        private List<FileDesc> ListFileDescriptions { get; set; }

        private FileDesc SelectedFileDescription { get; set; }

        private List<VmUsrOrDefChapterManagement> lstChapters { get; set; } = new List<VmUsrOrDefChapterManagement>();

        private List<VmUsrOrDefChapterManagement> lstAltSystemChapters { get; set; } = new List<VmUsrOrDefChapterManagement>();

        private List<VmUsrOrDefChapterManagement> lstAll { get; set; } = new List<VmUsrOrDefChapterManagement>();
        private List<VmUsrOrDefChapterManagement> lstAltSystemChapterItems { get; set; } = new List<VmUsrOrDefChapterManagement>();
        private List<VmFee> lstAltSystemFeeItems { get; set; } = new List<VmFee>();

        private List<VmUsrOrDefChapterManagement> lstAgendas { get; set; } = new List<VmUsrOrDefChapterManagement>();
        private List<VmFee> lstFees { get; set; } = new List<VmFee>();
        private List<VmChapterFee> lstVmFeeModalItems { get; set; } = new List<VmChapterFee>();
        private List<VmUsrOrDefChapterManagement> lstDocs { get; set; } = new List<VmUsrOrDefChapterManagement>();
        private List<VmUsrOrDefChapterManagement> lstStatus { get; set; } = new List<VmUsrOrDefChapterManagement>();

        private List<VmDataViews> ListVmDataViews { get; set; } = new List<VmDataViews>();

        public List<MpSysViews> ListP4WViews;
        public List<DmDocuments> dropDownChapterList;
        public List<CaseTypeGroups> partnerCaseTypeGroups;
        public List<fnORCHAGetFeeDefinitions> feeDefinitions;

        public string editCaseType { get; set; } = "";
        public string updateJSON { get; set; } = "";

        public string selectColour { 
            get 
            { 
                return selectedChapter.BackgroundColourName; 
            } 
            set 
            { 
                selectedChapter.BackgroundColour = ListChapterColours.Where(C => C.ColourName == value).Select(C => C.ColourCode).FirstOrDefault(); 
                selectedChapter.BackgroundColourName = value; 
            } 
        }

        public UsrOrDefChapterManagement editChapter { get; set; }
        public string isCaseTypeOrGroup { get; set; } = "";

        public VmDataViews EditDataViewObject = new VmDataViews { DataView = new DataViews() };
        public VmUsrOrDefChapterManagement editObject = new VmUsrOrDefChapterManagement { ChapterObject = new UsrOrDefChapterManagement() };
        public VmFee editFeeObject = new VmFee { FeeObject = new Fee() };
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

        public IList<string> JSONErrors { get; set; }

        public List<string> lstDocTypes { get; set; } = new List<string> { "Doc", "Letter", "Form", "Email", "Step" };

        public ChapterP4WStepSchema ChapterP4WStep { get; set; }

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


        public bool PartnerShowNotes
        {
            get { return (selectedChapter.ShowPartnerNotes == "Y" ? true : false); }
            set
            {
                if (value)
                {
                    selectedChapter.ShowPartnerNotes = "Y";
                }
                else
                {
                    selectedChapter.ShowPartnerNotes = "N";
                }
            }

        }

        public bool showNewStep { get; set; } = false;

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
                ListP4WViews = await partnerAccessService.GetPartnerViews();
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
                selectedChapter = new VmChapter { ChapterItems = new List<UsrOrDefChapterManagement>(), DataViews = new List<DataViews>() };
                selectedChapter.CaseTypeGroup = chapter.CaseTypeGroup;
                selectedChapter.CaseType = chapter.CaseType;
                selectedChapter.Name = chapter.Name;
            }

            selectedChapter.StepName = $"SF {chapter.Name} Smartflow";
            selectedChapter.SelectedStep = selectedChapter.SelectedStep is null || selectedChapter.SelectedStep == "Create New" ? "" : selectedChapter.SelectedStep;
            selectedChapter.Fees = selectedChapter.Fees is null ? new List<Fee>() : selectedChapter.Fees;
            selectedChapterId = chapter.Id;
            compareSystems = false;
            rowChanged = 0;
            navDisplay = "Chapter";
            showJSON = false;

            feeDefinitions = await chapterManagementService.GetFeeDefs(selectedChapter.CaseTypeGroup, selectedChapter.CaseType);

            ChapterFileOption = new ChapterFileOptions
            {
                Company = sessionState.Company.CompanyName,
                CaseTypeGroup = selectedChapter.CaseTypeGroup,
                CaseType = selectedChapter.CaseType,
                Chapter = selectedChapter.Name
            };

            ChapterFileUpload.SetChapterOptions(ChapterFileOption);

            await RefreshChapterItems("All");
            GetSeletedChapterFileList();

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
                Dictionary<int?, string> docTypes = new Dictionary<int?, string> { { 1, "Doc" },{ 4, "Form" }, {6, "Step" }, { 8, "Date" }, { 9, "Email" }, {11,"Doc" } , { 12, "Email" } };

                lstAll = lst.Select(L => new VmUsrOrDefChapterManagement { ChapterObject = L })
                                .Select(L => {
                                    L.ChapterObject.CaseTypeGroup = selectedChapter.CaseTypeGroup;
                                    L.ChapterObject.CaseType = selectedChapter.CaseType;
                                    L.ChapterObject.ChapterName = selectedChapter.Name;
                                    
                                    return L;
                                })
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
                                        .Select(A => {
                                            A.DocType = dropDownChapterList.Where(D => D.Name.ToUpper() == A.ChapterObject.Name.ToUpper())
                                                                                        .Select(D => string.IsNullOrEmpty(docTypes[D.DocumentType]) ? "Doc" : docTypes[D.DocumentType])
                                                                                        .FirstOrDefault();
                                                        return A;
                                        })
                                        .ToList();

                }
                if (listType == "Fees" | listType == "All")
                {
                    lstFees = selectedChapter.Fees.Select(F => new VmFee { FeeObject = F }).ToList();

                    lstVmFeeModalItems = feeDefinitions
                                            .Select(FD => new VmChapterFee
                                            {
                                                FeeItem = lstFees
                                                                .Where(F => FD.FeeDesc == F.FeeObject.FeeName)
                                                                .SingleOrDefault() is null
                                                                ? new Fee
                                                                {
                                                                    FeeName = FD.FeeDesc,
                                                                    SeqNo = 1000,
                                                                    FeeCategory = FD.Category,
                                                                    Amount = 0,
                                                                    VATable = "N",
                                                                    PostingType = ""
                                                                }
                                                                : lstFees
                                                                    .Where(F => FD.FeeDesc == F.FeeObject.FeeName)
                                                                    .Select(F => F.FeeObject)
                                                                    .SingleOrDefault(),
                                                feeDefinition = FD,
                                                selected = lstFees
                                                                .Where(F => FD.FeeDesc == F.FeeObject.FeeName)
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
                if(listType == "DataViews" | listType == "All")
                {
                    ListVmDataViews = (selectedChapter.DataViews is null) 
                                                    ? new List<VmDataViews>() 
                                                    : selectedChapter
                                                            .DataViews
                                                            .Select(D => new VmDataViews { DataView = D })
                                                            .OrderBy(D => D.DataView.BlockNo)
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

        private async void ToggleFeeComparison()
        {
            compareSystems = !compareSystems;

            if (compareSystems)
            {
                await CompareSelectedFeeToAltSystem();
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

                if (!(AltChapterObject is null))
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

                    //foreach (var item in lstFees)
                    //{
                    //    CompareChapterItemsToAltSytem(item);
                    //}

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

        private async Task<bool> CompareSelectedFeeToAltSystem()
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

                if (!(AltChapterObject is null))
                {
                    altChapter = JsonConvert.DeserializeObject<VmChapter>(AltChapterObject.ChapterData);

                    var temp = altChapter.Fees is null ? new List<Fee>() : altChapter.Fees;

                    lstAltSystemFeeItems = temp.Select(T => new VmFee { FeeObject = T }).ToList();

                    foreach (var item in lstFees)
                    {
                        CompareFeeItemsToAltSytem(item);
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

                    if (AltChapterObject is null)
                    {
                        chapter.ComparisonResult = "No match";
                        chapter.ComparisonIcon = "times";
                    }
                    else
                    {
                        altChapter = JsonConvert.DeserializeObject<VmChapter>(AltChapterObject.ChapterData);

                        lstAltSystemChapterItems = altChapter.ChapterItems.Select(T => new VmUsrOrDefChapterManagement { ChapterObject = T }).ToList();

                        foreach (var item in vmChapterItems)
                        {
                            CompareChapterItemsToAltSytem(item);
                        }

                        if (vmChapterItems.Where(C => C.ComparisonResult == "No match" | C.ComparisonResult == "Partial match").Count() > 0 | vmChapterItems.Count() != lstAltSystemChapterItems.Count())
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

        private VmFee CompareFeeItemsToAltSytem(VmFee chapterItem)
        {
            var altObject = lstAltSystemFeeItems
                                .Where(A => A.FeeObject.FeeName == chapterItem.FeeObject.FeeName)
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

        public async void CompareFeeItemsToAltSytemAction()
        {
            await CompareSelectedFeeToAltSystem();
        }

        private void PrepareForExport(List<VmUsrOrDefChapterManagement> items, string header)
        {
            var parameters = new ModalParameters();

            if (items is null)
            {
                items = new List<VmUsrOrDefChapterManagement> ();
            }

            parameters.Add("lstChapterItems", items);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-chapter-export"
            };

            Modal.Show<ChapterExport>("Smartflow Export", parameters, options);
        }

        private void PrepareForEdit(VmUsrOrDefChapterManagement item, string header)
        {
            selectedList = header;
            editObject = item;

            ShowChapterDetailModal("Edit");
        }

        private void PrepareDataViewForEdit(VmDataViews item, string header)
        {
            selectedList = header;
            EditDataViewObject = item;

            ShowDataViewDetailModal("Edit");
        }


        private void PrepareAttachmentForEdit(VmUsrOrDefChapterManagement item, string header)
        {
            selectedList = header;
            editObject = item;

            ShowChapterAttachmentModal();
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
                editFeeObject.FeeObject.SeqNo = lstFees
                                    .OrderByDescending(A => A.FeeObject.SeqNo)
                                    .Select(A => A.FeeObject.SeqNo)
                                    .FirstOrDefault() + 1;
            }

            editObject.ChapterObject.SeqNo = editObject.ChapterObject.SeqNo is null
                                                        ? 1
                                                        : editObject.ChapterObject.SeqNo;

            editFeeObject.FeeObject.SeqNo = editFeeObject.FeeObject.SeqNo is null
                                                        ? 1
                                                        : editFeeObject.FeeObject.SeqNo;

            editObject.ChapterObject.ParentId = selectedChapterId;

            ShowChapterDetailModal("Insert");
        }

        private void PrepareDataViewForInsert(string header)
        {
            selectedList = header;
            EditDataViewObject = new VmDataViews { DataView = new DataViews() } ;

            if(ListVmDataViews.Count > 0)
            {

                EditDataViewObject.DataView.BlockNo = ListVmDataViews
                                                       .OrderByDescending(D => D.DataView.BlockNo)
                                                       .Select(D => D.DataView.BlockNo)
                                                       .FirstOrDefault() + 1;
            }
            else
            {
                EditDataViewObject.DataView.BlockNo = 1;
            }


            ShowDataViewDetailModal("Insert");
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
                //case "Fees":
                //    lstItems = lstFees;
                //    break;
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

                if (listType == "Chapters")
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


        protected async void MoveBlockNo(DataViews selectobject, string listType, string direction)
        {
            seqMoving = true; //prevents changes to the form whilst process of changing seq is carried out

            int incrementBy;

            incrementBy = (direction.ToLower() == "up" ? -1 : 1);

            rowChanged = (int)(selectobject.BlockNo + incrementBy);

            var swapItem = ListVmDataViews.Where(D => D.DataView.BlockNo == (selectobject.BlockNo + incrementBy)).SingleOrDefault();
            if (!(swapItem is null))
            {
                selectobject.BlockNo += incrementBy;
                swapItem.DataView.BlockNo = swapItem.DataView.BlockNo + (incrementBy * -1);

                SelectedChapterObject.ChapterData = JsonConvert.SerializeObject(selectedChapter);
                await chapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);

            }

            await RefreshChapterItems(listType);
            await InvokeAsync(() =>
            {
                StateHasChanged();
            });


            seqMoving = false;

        }

        protected async void MoveFeeSeqNo(Fee selectobject, string listType, string direction)
        {
            seqMoving = true; //prevents changes to the form whilst process of changing seq is carried out

            int incrementBy;

            incrementBy = (direction.ToLower() == "up" ? -1 : 1);

            rowChanged = (int)(selectobject.SeqNo + incrementBy);

            var swapItem = lstFees.Where(D => D.FeeObject.SeqNo == (selectobject.SeqNo + incrementBy)).SingleOrDefault();
            if (!(swapItem is null))
            {
                selectobject.SeqNo += incrementBy;
                swapItem.FeeObject.SeqNo = swapItem.FeeObject.SeqNo + (incrementBy * -1);

                SelectedChapterObject.ChapterData = JsonConvert.SerializeObject(selectedChapter);
                await chapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);

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
                //case "Fees":
                //    listItems = lstFees;
                //    break;
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

        protected async void CondenseBlockNo(string ListType)
        {
            await RefreshChapterItems(ListType);

            int seqNo = 0;

            foreach (var item in ListVmDataViews.OrderBy(A => A.DataView.BlockNo))
            {
                seqNo += 1;
                item.DataView.BlockNo = seqNo;
            }

            SelectedChapterObject.ChapterData = JsonConvert.SerializeObject(selectedChapter);
            await chapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);

            await RefreshChapterItems(ListType);

            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }

        protected async void CondenseFeeSeq(string ListType)
        {
            await RefreshChapterItems(ListType);

            var ListItems = lstFees;

            int seqNo = 0;

            foreach (VmFee item in ListItems.OrderBy(A => A.FeeObject.SeqNo))
            {
                seqNo += 1;
                item.FeeObject.SeqNo = seqNo;
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

            Modal.Show<ChapterCopy>("Copy Smartflow", parameters, options);
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

            Modal.Show<ChapterAddOrEdit>("Smartflow", parameters, options);
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

            Modal.Show<ChapterCaseTypeEdit>("Smartflow", parameters, options);
        }


        protected void ShowChapterDetailViewModal(VmUsrOrDefChapterManagement selectedObject, string type)
        {
            selectedList = type;

            var parameters = new ModalParameters();
            parameters.Add("Object", selectedObject);
            parameters.Add("SelectedList", selectedList);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-chapter-comparison"
            };

            Modal.Show<ChapterDetailView>(selectedList, parameters, options);
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
                PopupAlert = editObject.ChapterObject.PopupAlert,
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

        protected void ShowDataViewDetailModal(string option)
        {
            Action action = RefreshSelectedList;

            var copyObject = new DataViews
            {
                BlockNo = EditDataViewObject.DataView.BlockNo,
                DisplayName = EditDataViewObject.DataView.DisplayName,
                ViewName = EditDataViewObject.DataView.ViewName
            };

            var parameters = new ModalParameters();
            parameters.Add("TaskObject", EditDataViewObject.DataView);
            parameters.Add("ListPartnerViews", ListP4WViews);
            parameters.Add("CopyObject", copyObject);
            parameters.Add("DataChanged", action);
            parameters.Add("SelectedChapter", selectedChapter);
            parameters.Add("SelectedChapterObject", SelectedChapterObject);
            parameters.Add("Option", option);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-chapter-item"
            };

            Modal.Show<DataViewDetail>("Data View", parameters, options);
        }



        protected void ShowChapterAttachmentModal()
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
                PopupAlert = editObject.ChapterObject.PopupAlert,
                NextStatus = editObject.ChapterObject.NextStatus,
                FollowUpDocs = editObject.ChapterObject.FollowUpDocs
            };

            var attachment = copyObject.FollowUpDocs is null ? new FollowUpDoc() : copyObject.FollowUpDocs.FirstOrDefault();

            attachment = attachment is null ? new FollowUpDoc() : attachment;

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
            parameters.Add("Attachment", attachment);

            string className = "modal-chapter-item";

            if (selectedList == "Steps and Documents")
            {
                className = "modal-chapter-doc";
            }
            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal " + className
            };

            Modal.Show<ChapterAttachments>(selectedList, parameters, options);
        }

        protected void PrepareFeeForInsert (string option)
        {
            Fee taskObject = new Fee();
            if (!(lstFees is null ) && lstFees.Count() > 0)
            {
                taskObject.SeqNo = lstFees.Select(F => F.FeeObject.SeqNo).OrderByDescending(F => F).FirstOrDefault() + 1;
            }
            else
            {
                taskObject.SeqNo = 1;
            }
            ShowChapterFeesModal(option, taskObject);
        }

        protected void ShowChapterFeesModal(string option, Fee taskObject)
        {
            Action dataChanged = CondenseFeeSeq;
            Fee copyObject = new Fee { 
                FeeName = taskObject.FeeName 
                , FeeCategory = taskObject.FeeCategory
                , SeqNo = taskObject.SeqNo
                , Amount = taskObject.Amount
                , VATable = taskObject.VATable
                , PostingType = taskObject.PostingType
            };
            var parameters = new ModalParameters();
            parameters.Add("Option", option);
            parameters.Add("TaskObject", taskObject);
            parameters.Add("CopyObject", copyObject);
            parameters.Add("SelectedChapter", selectedChapter);
            parameters.Add("SelectedChapterObject", SelectedChapterObject);
            parameters.Add("DataChanged", dataChanged);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-chapter-fees"
            };


            Modal.Show<ChapterFees>("Fees", parameters, options);
        }

        protected void ShowChapterFeeViewModal(VmFee selectedObject)
        {
            var parameters = new ModalParameters();
            parameters.Add("Object", selectedObject);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-chapter-comparison"
            };

            Modal.Show<ChapterFeeView>("Fee", parameters, options);
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

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal"
            };

            Modal.Show<ModalDelete>("Delete?", parameters, options);
        }

        protected void PrepareChapterFeeDelete(VmFee selectedChapterItem)
        {
            editFeeObject = selectedChapterItem;

            Action SelectedDeleteAction = HandleChapterFeeDelete;
            var parameters = new ModalParameters();
            parameters.Add("InfoHeader", "Delete?");
            parameters.Add("ModalHeight", "300px");
            parameters.Add("ModalWidth", "500px");
            parameters.Add("DeleteAction", SelectedDeleteAction);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal"
            };

            Modal.Show<ModalDelete>("Delete?", parameters, options);
        }


        protected void PrepareDataViewDelete(VmDataViews selectedDataView)
        {
            EditDataViewObject = selectedDataView;

            Action SelectedDeleteAction = HandleDataViewDelete;
            var parameters = new ModalParameters();
            parameters.Add("InfoHeader", "Delete?");
            parameters.Add("ModalHeight", "300px");
            parameters.Add("ModalWidth", "500px");
            parameters.Add("DeleteAction", SelectedDeleteAction);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal"
            };

            Modal.Show<ModalDelete>("Delete?", parameters, options);
        }

        protected void ShowDataViewDisplayModal(VmDataViews selectedObject)
        {
            var parameters = new ModalParameters();
            parameters.Add("Object", selectedObject);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-chapter-comparison"
            };

            Modal.Show<DataViewDisplay>("Data View", parameters, options);
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


            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal"
            };

            Modal.Show<ModalDelete>("Delete?", parameters, options);
        }


        protected void PrepareBackUpForDelete(FileDesc selectedFile)
        {
            SelectedFileDescription = selectedFile;

            Action SelectedDeleteAction = HandleDeleteFile;
            var parameters = new ModalParameters();
            parameters.Add("InfoHeader", "Delete?");
            parameters.Add("ModalHeight", "300px");
            parameters.Add("ModalWidth", "500px");
            parameters.Add("DeleteAction", SelectedDeleteAction);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal"
            };

            Modal.Show<ModalDelete>("Delete?", parameters, options);
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

            Modal.Show<ChapterItemComparison>("Synchronise Smartflow Item", parameters, options);
        }

        private void PrepareFeeForComparison(VmFee selectedItem)
        {
            editFeeObject = selectedItem;

            ShowFeeComparisonModal();
        }

        protected void ShowFeeComparisonModal()
        {
            Action Compare = CompareFeeItemsToAltSytemAction;

            var parameters = new ModalParameters();
            parameters.Add("Object", editFeeObject);
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

            Modal.Show<ChapterFeeComparison>("Synchronise Smartflow Item", parameters, options);
        }

        private async void HandleChapterDetailDelete()
        {

            selectedChapter.ChapterItems.Remove(editObject.ChapterObject);
            SelectedChapterObject.ChapterData = JsonConvert.SerializeObject(selectedChapter);
            await chapterManagementService.Update(SelectedChapterObject);

            await RefreshChapterItems(navDisplay);
            StateHasChanged();
        }

        private async void HandleChapterFeeDelete()
        {

            selectedChapter.Fees.Remove(editFeeObject.FeeObject);
            SelectedChapterObject.ChapterData = JsonConvert.SerializeObject(selectedChapter);
            await chapterManagementService.Update(SelectedChapterObject);

            await RefreshChapterItems(navDisplay);
            StateHasChanged();
        }

        private async void HandleDataViewDelete()
        {
            selectedChapter.DataViews.Remove(EditDataViewObject.DataView);
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

            if (AltChapterObject is null)
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
            if (navDisplay.ToLower() == "chapter")
            {
                SyncAll();
            }
            else
            {
                SyncToAltSystem(navDisplay);
            }

        }

        public void GetFile(FileDesc fileDesc)
        {
            NavigationManager.NavigateTo(fileDesc.FilePath + "//" + fileDesc.FileName, true);
        }

        public void WriteChapterJSONToFile()
        {
            var fileName = selectedChapter.Name + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt";

            ChapterFileUpload.WriteChapterToFile(SelectedChapterObject.ChapterData, fileName);

            GetSeletedChapterFileList();
            StateHasChanged();
        }

        private async void HandleFileSelection(IFileListEntry[] entryFiles)
        {
            var files = new List<IFileListEntry>();
            IList<string> fileErrorDescs = new List<string>();

            foreach (var file in entryFiles)
            {
                if (file != null)
                {
                    if(!(file.Name.Contains(".txt") || file.Name.Contains(".JSON")))
                    {
                        fileErrorDescs.Add($"The file: {file.Name} is not the correct type for a backup. Backup files must be either .txt or .JSON");
                    }
                    else
                    {
                        if(ListFileDescriptions.Where(F => F.FileName == file.Name).FirstOrDefault() is null)
                        {
                            await ChapterFileUpload.UploadChapterFiles(file);
                            files.Add(file);
                        }
                        else
                        {
                            fileErrorDescs.Add($"The file: {file.Name} already exists on the system");
                        }

                        
                    }
                    
                }

            }
            if (files != null && files.Count > 0)
            {
                StateHasChanged();
            }

            if(fileErrorDescs.Count > 0)
            {
                ShowErrorModal("File Upload Error", "The following errors occured during the upload:", fileErrorDescs);
            }

            GetSeletedChapterFileList();

            StateHasChanged();
        }


        private void GetSeletedChapterFileList()
        {
            ListFileDescriptions = ChapterFileUpload.GetFileListForChapter();
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

        private bool BlockNoIsValid()
        {
            if (seqMoving == false | compareSystems == true)
            {

                bool isValid = true;

                for (int i = 0; i < ListVmDataViews.Count; i++)
                {
                    if (ListVmDataViews[i].DataView.BlockNo != i + 1)
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

        private bool FeeSeqNoIsValid()
        {
            if (seqMoving == false | compareSystems == true)
            {

                bool isValid = true;

                for (int i = 0; i < lstFees.Count; i++)
                {
                    if (lstFees[i].FeeObject.SeqNo != i + 1)
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

            if (!(AltChapterObject is null))
            {
                if (!string.IsNullOrEmpty(AltChapterObject.ChapterData))
                {
                    selectedCopyItems = JsonConvert.DeserializeObject<VmChapter>(AltChapterObject.ChapterData);
                }
            }
            else
            {
                AltChapterObject = new UsrOrDefChapterManagement();
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
                ChapterItems = selectedCopyItems.ChapterItems,
                DataViews = selectedChapter.DataViews
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

        private void ReadBackUpFile(string filePath)
        {
            var readJSON = ChapterFileUpload.readJson(filePath);
            SaveJson(readJSON);
        }

        private async void DownloadFile(FileDesc file)
        {
            var data = ChapterFileUpload.ReadFileToByteArray(file.FilePath);

            await jsRuntime.InvokeAsync<object>(
                 "DownloadTextFile",
                 file.FileName,
                 Convert.ToBase64String(data));


        }

        private void HandleDeleteFile()
        {
            DeleteFile(SelectedFileDescription);
        }

        private void DeleteFile(FileDesc file)
        {
            ChapterFileUpload.DeleteFile(file.FilePath);
            GetSeletedChapterFileList();
            StateHasChanged();
        }


        private async void SaveJson(string Json)
        {
            JSONErrors = new List<string>();
            JSONErrors = ChapterFileUpload.ValidateChapterJSON(Json);

            if (JSONErrors.Count == 0)
            {
                var chapterData = JsonConvert.DeserializeObject<VmChapter>(Json);
                selectedChapter.ChapterItems = chapterData.ChapterItems;
                selectedChapter.DataViews = chapterData.DataViews;
                selectedChapter.Fees = chapterData.Fees;
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
                ShowErrorModal("Backup Restore Error", "The following errors occured during the restore:", JSONErrors);
            }
        }


        protected void ShowErrorModal(string header, string errorDesc, IList<string> errorDets)
        {
            var parameters = new ModalParameters();
            parameters.Add("ErrorDesc", errorDesc);
            parameters.Add("ErrorDetails", errorDets);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-chapter-import"
            };

            Modal.Show<ModalErrorInfo>(header, parameters, options);
        }

        private void RefreshJson()
        {
            SaveJson(SelectedChapterObject.ChapterData);
        }


        protected void ShowChapterImportModel()
        {

            while (!(ListFileDescriptions.Where(F => F.FilePath.Contains(".xlsx")).FirstOrDefault() is null))
            {
                ChapterFileUpload.DeleteFile(ListFileDescriptions.Where(F => F.FilePath.Contains(".xlsx")).FirstOrDefault().FilePath);
                GetSeletedChapterFileList();
            }


            Action WriteBackUp = WriteChapterJSONToFile;

            Action SelectedAction = RefreshJson;
            var parameters = new ModalParameters();
            parameters.Add("TaskObject", SelectedChapterObject);
            parameters.Add("ListFileDescriptions", ListFileDescriptions);
            parameters.Add("DataChanged", SelectedAction);
            parameters.Add("WriteBackUp", WriteBackUp);
            parameters.Add("OriginalDataViews", ListVmDataViews);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-chapter-import"
            };

            Modal.Show<ChapterImport>("Excel Import", parameters, options);
        }

        private async void ExportSmartflowToExcel()
        {
            await ChapterFileUpload.WriteChapterDataToExcel(selectedChapter, dropDownChapterList);
        }

        public void CancelCreateP4WStep()
        {
            selectedChapter.StepName = "";
            selectedChapter.SelectedStep = "";
            selectedChapter.StepName = $"SF {selectedChapter.Name} Smartflow";
            StateHasChanged();
        }

        

        protected async void CreateP4WSmartflowStep()
        {
            IList<string> Errors = new List<string>(); 

            if (string.IsNullOrEmpty(selectedChapter.P4WCaseTypeGroup))
            {
                Errors.Add("Missing Case Type Group");
            }

            if (string.IsNullOrEmpty(selectedChapter.SelectedView))
            {
                Errors.Add("Missing View");
            }

            if (string.IsNullOrEmpty(selectedChapter.StepName))
            {
                Errors.Add("Missing Step Name");
            }
                

            if (Errors.Count == 0)
            {
                ChapterP4WStep = new ChapterP4WStepSchema
                {
                    StepName = selectedChapter.StepName
                    ,
                    P4WCaseTypeGroup = selectedChapter.P4WCaseTypeGroup
                    ,
                    GadjITCaseTypeGroup = selectedChapter.CaseTypeGroup
                    ,
                    GadjITCaseType = selectedChapter.CaseType
                    ,
                    Smartflow = selectedChapter.Name
                    ,
                    Questions = new List<ChapterP4WStepQuestion>{
                    new ChapterP4WStepQuestion {QNo = 1, QText= "HQ - Set Current Chapter Details" }
                    ,new ChapterP4WStepQuestion {QNo = 2, QText= "HQ - Show View" }
                    ,new ChapterP4WStepQuestion {QNo = 3, QText= "HQ - Run Required Steps" }
                    ,new ChapterP4WStepQuestion {QNo = 4, QText= "HQ - Check if need to reschedule" }
                    ,new ChapterP4WStepQuestion {QNo = 5, QText= "HQ - Reinsert this step with asname" }
                    ,new ChapterP4WStepQuestion {QNo = 6, QText= "HQ - Rename Step" }
                    ,new ChapterP4WStepQuestion {QNo = 7, QText= "HQ - Delete Previous Instances" }
                    ,new ChapterP4WStepQuestion {QNo = 8, QText= "HQ - Check if completion name exists" }
                    ,new ChapterP4WStepQuestion {QNo = 9, QText= "HQ - Delete Step" }
                }
                    ,
                    Answers = new List<ChapterP4WStepAnswer>{
                        new ChapterP4WStepAnswer {QNo = 1, GoToData= $"2 [SQL: UPDATE Usr_ORSF_MT_Control SET Current_SF = '{selectedChapter.Name}', Current_Case_Type_Group = '{selectedChapter.CaseTypeGroup}', Current_Case_Type = '{selectedChapter.CaseType}', Default_Step = '{selectedChapter.StepName}', Date_Schedule_For = DATEADD(d, 7, getdate()), Steps_To_Run = '', Schedule_AsName = (SELECT CASE WHEN dbo.fn_OR_IsAllCap (Description) = 0 THEN '{selectedChapter.StepName}' ELSE Description END + '|' + CONVERT(VARCHAR(20),ISNULL(Date_Schedule_For,DATEADD(d, 7, getdate())),103) + '|' + '[matters.feeearnerref]' FROM Cm_CaseItems WHERE ItemID = [currentstep.stepid]), Complete_AsName = ''WHERE EntityRef = '[matters.entityref]' AND MatterNo = [matters.number]][SQL: UPDATE Usr_ORSF_MT_Control SET Screen_Opened_Via_Step = 'Y' WHERE EntityRef='[matters.entityref]' AND matterNo=[matters.number]]" }
                    ,new ChapterP4WStepAnswer {QNo = 2, GoToData= $"3 [VIEW: '{selectedChapter.SelectedView}' UPDATE=Yes]" }
                    ,new ChapterP4WStepAnswer {QNo = 3, GoToData= $"4 [SQL: SELECT dbo.fn_OR_DEBT_GetStepsFromList('[~Usr_ORSF_MT_Control.Steps_To_Run]')] [SQL: UPDATE Usr_ORSF_MT_Control SET Screen_Opened_Via_Step = null WHERE EntityRef='[matters.entityref]' AND matterNo=[matters.number]]" }
                    ,new ChapterP4WStepAnswer {QNo = 4, GoToData= $"[SQL: SELECT CASE WHEN '[!Usr_ORSF_MT_Control.Do_Not_Reschedule]' <> 'Y' THEN 5 ELSE 8 END] [SQL: UPDATE Usr_ORSF_MT_Control SET Date_Schedule_For = isnull(Date_Schedule_For, Cast(getdate() as Date)) WHERE EntityRef = '[matters.entityref]' AND MatterNo = [matters.number]]" }
                    ,new ChapterP4WStepAnswer {QNo = 5, GoToData= $"8 [SQL: SELECT ScheduleCommand FROM fn_OR_CHAPTER_GetScheduleItems(NULL, '[Usr_ORSF_MT_Control.Schedule_AsName]' , '{selectedChapter.StepName}') ]" }
                    ,new ChapterP4WStepAnswer {QNo = 6, GoToData= $"[SQL: UPDATE cm_caseitems set CompletionDate = GETDATE(), description = UPPER('[!Usr_ORSF_MT_Control.Complete_AsName]') where itemid = [currentstep.stepid]]" }
                    ,new ChapterP4WStepAnswer {QNo = 7, GoToData= $"8 [SQL: exec up_OR_DEBT_DeleteDueStep '', [currentstep.stepid], '{selectedChapter.StepName}']" }
                    ,new ChapterP4WStepAnswer {QNo = 8, GoToData= $"[SQL: SELECT CASE WHEN ISNULL('[!Usr_ORSF_MT_Control.Complete_AsName]','') <> '' THEN 6 ELSE 9 END]" }
                    ,new ChapterP4WStepAnswer {QNo = 9, GoToData= $"[SQL: DELETE FROM cm_caseitems where itemid = [currentstep.stepid]]" }
                }
                };

                string stepJSON = JsonConvert.SerializeObject(ChapterP4WStep);

                bool creationSuccess;



                creationSuccess = await chapterManagementService.CreateStep(new VmChapterP4WStepSchemaJSONObject { StepSchemaJSON = stepJSON });

                if (creationSuccess)
                {
                    dropDownChapterList = await chapterManagementService.GetDocumentList(selectedChapter.CaseType);

                    selectedChapter.SelectedStep = selectedChapter.StepName;

                    SelectedChapterObject.ChapterData = JsonConvert.SerializeObject(selectedChapter);
                    await chapterManagementService.Update(SelectedChapterObject);

                    StateHasChanged();
                }
            }
            else
            {
                ShowErrorModal("Step Creation", "Step creation could not be completed:", Errors);
            }
        }
    }
}
