using GadjIT.ClientContext.P4W;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GadjIT_V1_02.Services;
using System.Web;
using GadjIT_V1_02.Services.SessionState;
using Blazored.Modal.Services;
using Blazored.Modal;
using GadjIT_V1_02.Pages.Shared.Modals;
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
using GadjIT_V1_02.FileManagement.FileClassObjects.FileOptions;
using GadjIT_V1_02.FileManagement.FileClassObjects;
using System.Net;
using Microsoft.JSInterop;
using GadjIT_V1_02.FileManagement.FileProcessing.Interface;
using GadjIT_V1_02.Data.Admin;
using System.Globalization;
using GadjIT.GadjitContext.GadjIT_App;
using AutoMapper;
using System.Timers;
using GadjIT_V1_02.Services.AppState;

namespace GadjIT_V1_02.Pages.Chapters
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
        public IMapper mapper { get; set; }

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

        [Inject]
        private IIdentityUserAccess UserAccess { get; set; }

        [Inject]
        public IAppChapterState appChapterState { get; set; }

        [Inject]
        private ICompanyDbAccess CompanyDbAccess { get; set; }

        private ChapterFileOptions ChapterFileOption { get; set; }

        private List<FileDesc> ListFilesForBackups { get; set; }

        private List<FileDesc> ListFilesForBgImages { get; set; }

        private FileDesc SelectedFileDescription { get; set; }

        private int ValidTicketMessageCount { get; set; } 

        private List<VmUsrOrsfSmartflows> lstChapters { get; set; } = new List<VmUsrOrsfSmartflows>();

        private List<VmUsrOrsfSmartflows> lstAltSystemChapters { get; set; } = new List<VmUsrOrsfSmartflows>();

        private List<VmUsrOrDefChapterManagement> lstAll { get; set; } = new List<VmUsrOrDefChapterManagement>();
        private List<VmUsrOrDefChapterManagement> lstAltSystemChapterItems { get; set; } = new List<VmUsrOrDefChapterManagement>();
        private List<VmFee> lstAltSystemFeeItems { get; set; } = new List<VmFee>();

        private List<VmDataViews> lstAltSystemDataViews { get; set; } = new List<VmDataViews>();

        private List<VmTickerMessages> lstAltSystemTickerMessages { get; set; } = new List<VmTickerMessages>();

        private List<VmUsrOrDefChapterManagement> lstAgendas { get; set; } = new List<VmUsrOrDefChapterManagement>();
        private List<VmFee> lstFees { get; set; } = new List<VmFee>();
        private List<VmUsrOrDefChapterManagement> lstDocs { get; set; } = new List<VmUsrOrDefChapterManagement>();
        private List<VmUsrOrDefChapterManagement> lstStatus { get; set; } = new List<VmUsrOrDefChapterManagement>();

        private List<VmDataViews> ListVmDataViews { get; set; } = new List<VmDataViews>();
        private List<VmTickerMessages> ListVmTickerMessages { get; set; } = new List<VmTickerMessages>();

        public List<MpSysViews> ListP4WViews;
        public List<DmDocuments> dropDownChapterList;
        public List<TableDate> TableDates;
        public List<CaseTypeGroups> partnerCaseTypeGroups;

        public string editCaseType { get; set; } = "";
        public string updateJSON { get; set; } = "";
        public bool CreateNewSmartflow { get; set; }

        public string selectColour { 
            get 
            { 
                return selectedChapter.BackgroundColourName; 
            }
            set
            {
                SaveSelectedBackgroundColour(value);
            }
        }

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
                sessionState.HomeActionSmartflow = SelectHome;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            

        }

        private Timer timer;

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                timer = new Timer();
                timer.Interval = 10000; //10 seconds
                timer.Elapsed += OnTimerInterval;
                timer.AutoReset = true;
                // Start the timer
                timer.Enabled = true;
            }
            base.OnAfterRender(firstRender);
        }

        private async void OnTimerInterval(object sender, ElapsedEventArgs e)
        {
            //Compare current session with Chapter State to see if any other users have updated the Chapter
            //If an update is detected (ChapterLastUpdated is greater than session's ChapterLastCompared) then 
            // reload the data and invoke StateHasChanged to refresh the page.
            bool gotLock = appChapterState.Lock;
            while (gotLock)
            {
                await Task.Yield();
                gotLock = appChapterState.Lock;
            }

            var ChapterLastUpdated = appChapterState.GetLastUpdatedDate(sessionState, selectedChapter);

            if (!(selectedChapter.Name == "" | selectedChapter.Name is null))
            {
                if (sessionState.ChapterLastCompared != ChapterLastUpdated)
                {
                    gotLock = CompanyDbAccess.Lock;
                    while (gotLock)
                    {
                        await Task.Yield();
                        gotLock = CompanyDbAccess.Lock;
                    }


                    var lstC = await chapterManagementService.GetAllChapters();

                    await CompanyDbAccess.SyncAdminSysToClient(lstC, sessionState);

                    var id = SelectedChapterObject.Id;

                    var lsrSR = await CompanyDbAccess.GetAllSmartflowRecords(sessionState);
                    lstChapters = lsrSR.Select(A => new VmUsrOrsfSmartflows { SmartflowObject = mapper.Map(A, new UsrOrsfSmartflows()) }).ToList();

                    SelectedChapterObject = lstChapters.Where(C => C.SmartflowObject.Id == id).Select(C => C.SmartflowObject).FirstOrDefault();

                    if (!(SelectedChapterObject.SmartflowData is null))
                    {
                        selectedChapter = JsonConvert.DeserializeObject<VmChapter>(SelectedChapterObject.SmartflowData);
                    }
                    else
                    {
                        //Initialise the VmChapter in case of null Json
                        selectedChapter = new VmChapter
                        {
                            Items = new List<GenSmartflowItem>()
                                                         ,
                            DataViews = new List<DataViews>()
                                                         ,
                            TickerMessages = new List<TickerMessages>()
                                                         ,
                            Fees = new List<Fee>()
                        };
                        selectedChapter.CaseTypeGroup = SelectedChapterObject.CaseTypeGroup;
                        selectedChapter.CaseType = SelectedChapterObject.CaseType;
                        selectedChapter.Name = SelectedChapterObject.SmartflowName;
                    }


                    await RefreshChapterItems("All");
                    await InvokeAsync(() =>
                    {
                        StateHasChanged();
                    });

                    sessionState.ChapterLastCompared = ChapterLastUpdated;
                }
            }

            gotLock = appChapterState.Lock;
            while (gotLock)
            {
                await Task.Yield();
                gotLock = appChapterState.Lock;
            }

            appChapterState.SetUsersCurrentChapter(sessionState, selectedChapter);

        }

        public void Dispose()
        {
            // During prerender, this component is rendered without calling OnAfterRender and then immediately disposed
            // this mean timer will be null so we have to check for null or use the Null-conditional operator ? 
            timer?.Dispose();
            appChapterState.DisposeUser(sessionState);
        }

        public async void SaveSelectedBackgroundColour (string colour)
        {
            selectedChapter.BackgroundColour = ListChapterColours.Where(C => C.ColourName == colour).Select(C => C.ColourCode).FirstOrDefault();
            selectedChapter.BackgroundColourName = colour;

            //if (ListFileImages.Select(I => I.FileName).ToList().Contains(colour))
            //{
            //    sessionState.SetTempBackground(ListFileImages.Where(I => I.FileName == colour).Select(F => F.FileDirectory.Replace("\\", "/").Replace("wwwroot/", "") + "/" + F.FileName).SingleOrDefault(), NavigationManager.Uri);
            //}
            //else
            //{
            //    sessionState.SetTempBackground(selectedChapter.BackgroundColour, NavigationManager.Uri);
            //}

            SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(selectedChapter);

            await chapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);


            //keep track of time last updated ready for comparison by other sessions checking for updates
            appChapterState.SetLastUpdated(sessionState, selectedChapter);
        }

        public UsrOrsfSmartflows editChapter { get; set; }
        public string isCaseTypeOrGroup { get; set; } = "";

        public VmDataViews EditDataViewObject = new VmDataViews { DataView = new DataViews() };
        public VmTickerMessages EditTickerMessageObject = new VmTickerMessages { Message = new TickerMessages() };
        
        public VmChapterComparison editChapterComparison = new VmChapterComparison();

        public VmUsrOrDefChapterManagement editObject = new VmUsrOrDefChapterManagement { ChapterObject = new GenSmartflowItem() };

        public LinkedItems attachObject = new LinkedItems();

        public VmFee editFeeObject = new VmFee { FeeObject = new Fee() };

        public UsrOrsfSmartflows editChapterObject = new UsrOrsfSmartflows ();


        string selectedList = string.Empty;

        string displaySection { get; set; } = "";

        [Parameter]
        public string UrlCaseTypeGroup { set { selectedChapter.CaseTypeGroup = value; } }

        [Parameter]
        public string UrlCaseType { set { selectedChapter.CaseType = value; } }

        [Parameter]
        public string UrlChapter { set { selectedChapter.Name = value; } }

        [Parameter]
        public VmChapter selectedChapter { get; set; } = new VmChapter { Items = new List<GenSmartflowItem>() };

        

        public VmChapter altChapter { get; set; } = new VmChapter();

        public UsrOrsfSmartflows SelectedChapterObject { get; set; } = new UsrOrsfSmartflows();

        public UsrOrsfSmartflows AltChapterObject { get; set; } = new UsrOrsfSmartflows();

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

        public ChapterP4WStepSchema ChapterP4WStep { get; set; }

        public bool showNewStep { get; set; } = false;

        [Inject]
        public IFileHelper FileHelper { get; set; }
        private List<ChapterColour> ListChapterColours { 
            get
            {
                List<ChapterColour> listChapterColours = new List<ChapterColour>
                {
                    new ChapterColour { ColourName = "", ColourCode = ""},
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



                //foreach ( FileDesc bgImage in ListFileImages)
                //{
                //    string imgName = bgImage.FileName;
                //    string imgPath = @"/images/backgroundimages/" + imgName;
                //    //imgName = imgName.Replace(".jpg", "");
                //    listChapterColours.Add(new ChapterColour { ColourName = imgName, ColourCode = imgPath });
                //}
                return listChapterColours;
            }
        } 

        

        



        public bool PreviewChapterImage
        {
            get 
            {
                return sessionState.User.DisplaySmartflowPreviewImage; 
            }
            set
            {
                if (value)
                {
                    if(!string.IsNullOrEmpty(selectedChapter.BackgroundImageName))
                    {
                        sessionState.SetTempBackground(selectedChapter.BackgroundImage.Replace("/wwwroot",""), NavigationManager.Uri);
                    }
                    else
                    {
                        sessionState.SetTempBackground("", NavigationManager.Uri);
                    }
                }
                else
                {
                    sessionState.TempBackGroundImage = "";
                }

                sessionState.User.DisplaySmartflowPreviewImage = value;
                sessionState.RefreshHome?.Invoke();
                SavePreviewChapterImage();
            }

        }


        private async void SavePreviewChapterImage()
        {
            bool gotLock = UserAccess.Lock;
            while (gotLock)
            {
                await Task.Yield();
                gotLock = UserAccess.Lock;
            }

            await UserAccess.UpdateUserDetails(sessionState.User).ConfigureAwait(false);
        }


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
                
                SaveShowPartnerNotes();
            }

        }

        private async void SaveShowPartnerNotes()
        {
            SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(selectedChapter);

            await chapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);

            //keep track of time last updated ready for comparison by other sessions checking for updates
            appChapterState.SetLastUpdated(sessionState, selectedChapter);
        }

        public bool ShowDocumentTracking
        {
            get { return (selectedChapter.ShowDocumentTracking == "Y" ? true : false); }
            set
            {
                if (value)
                {
                    selectedChapter.ShowDocumentTracking = "Y";
                }
                else
                {
                    selectedChapter.ShowDocumentTracking = "N";
                }

                SaveAttachmentTracking();
            }

        }

        private async void SaveAttachmentTracking()
        {
            SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(selectedChapter);

            await chapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);

            //keep track of time last updated ready for comparison by other sessions checking for updates
            appChapterState.SetLastUpdated(sessionState, selectedChapter);
        }


        public void DirectToLogin()
        {
            string returnUrl = HttpUtility.UrlEncode($"/");
            NavigationManager.NavigateTo($"Identity/Account/Login?returnUrl={returnUrl}", true);
        }

        void SelectHome()
        {
            //NavigationManager.NavigateTo($"Smartflow/{selectedChapter.CaseTypeGroup}/{selectedChapter.CaseType}",true);
            if (!string.IsNullOrEmpty(sessionState.TempBackGroundImage))
            {
                sessionState.TempBackGroundImage = "";
                sessionState.RefreshHome?.Invoke();
            }
            compareSystems = false;
            selectedChapter.Name = "";
            rowChanged = 0;

            StateHasChanged();
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
            
        }

        private void NavigateToChapter(UsrOrsfSmartflows chapter)
        {
            NavigationManager.NavigateTo($"Smartflow/{chapter.CaseTypeGroup}/{chapter.CaseType}/{chapter.SmartflowName}",true);
        }

        private async void SelectChapter(UsrOrsfSmartflows chapter)
        {
            displaySpinner = true;

            lstAll = new List<VmUsrOrDefChapterManagement>();

            SelectedChapterObject = chapter;

            if (!(chapter.SmartflowData is null))
            {
                selectedChapter = JsonConvert.DeserializeObject<VmChapter>(chapter.SmartflowData);
            }
            else
            {
                //Initialise the VmChapter in case of null Json
                selectedChapter = new VmChapter { Items = new List<GenSmartflowItem>()
                                                 , DataViews = new List<DataViews>()
                                                 , TickerMessages = new List<TickerMessages>()
                                                 , Fees = new List<Fee>()};
                selectedChapter.CaseTypeGroup = chapter.CaseTypeGroup;
                selectedChapter.CaseType = chapter.CaseType;
                selectedChapter.Name = chapter.SmartflowName;
            }

            selectedChapter.StepName = $"SF {chapter.SmartflowName} Smartflow";
            selectedChapter.SelectedStep = selectedChapter.SelectedStep is null || selectedChapter.SelectedStep == "Create New" ? "" : selectedChapter.SelectedStep;
            selectedChapter.Fees = selectedChapter.Fees is null ? new List<Fee>() : selectedChapter.Fees;
            selectedChapter.Items = selectedChapter.Items is null ? new List<GenSmartflowItem>() : selectedChapter.Items;
            selectedChapterId = chapter.Id;
            compareSystems = false;
            rowChanged = 0;
            navDisplay = "Chapter";
            showJSON = false;



            dropDownChapterList = await chapterManagementService.GetDocumentList(selectedChapter.CaseType);
            TableDates = await chapterManagementService.GetDatabaseTableDateFields();

            await RefreshChapterItems("All");
            
            
            //set path to point to the BackgroundImage path for the current company
            FileHelper.CustomPath = $"wwwroot/images/Companies/{sessionState.Company.CompanyName}/BackgroundImages";
            ListFilesForBgImages = FileHelper.GetFileList();

            if (!string.IsNullOrEmpty(selectedChapter.BackgroundImage))
            {
                sessionState.SetTempBackground(selectedChapter.BackgroundImage.Replace("/wwwroot", ""), NavigationManager.Uri);
                sessionState.RefreshHome?.Invoke();
            }

            SetSmartflowFilePath();
            GetSeletedChapterFileList();


            //set the session ChapterLastCompared to be the same as the Chapter State value to prevent an immediate 
            // refresh of the page (by OnTimerInterval)
            if (!(selectedChapter.Name == "" | selectedChapter.Name is null))
            {

                bool gotLock = appChapterState.Lock;
                while (gotLock)
                {
                    await Task.Yield();
                    gotLock = appChapterState.Lock;
                }

                sessionState.ChapterLastCompared = appChapterState.GetLastUpdatedDate(sessionState, selectedChapter);
            }

            StateHasChanged();

        }

        private async void RefreshDocList()
        {
            dropDownChapterList = await chapterManagementService.GetDocumentList(selectedChapter.CaseType);
            StateHasChanged();
        }

        private void SetSmartflowFilePath()
        {
            ChapterFileOption = new ChapterFileOptions
            {
                Company = sessionState.Company.CompanyName,
                CaseTypeGroup = selectedChapter.CaseTypeGroup,
                CaseType = selectedChapter.CaseType,
                Chapter = selectedChapter.Name
            };

            ChapterFileUpload.SetChapterOptions(ChapterFileOption);
        }

        private async void SaveChapterDetails()
        {
            SelectedChapterObject.SmartflowName = selectedChapter.Name;

            SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(selectedChapter);
            await chapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);


            //keep track of time last updated ready for comparison by other sessions checking for updates
            appChapterState.SetLastUpdated(sessionState, selectedChapter);

            await RefreshChapterItems("All");
            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }




        private async void RefreshChapters()
        {
            ListChapterLoaded = false;

            //var lstC = await chapterManagementService.GetAllChapters();
            //lstChapters = lstC.Select(A => new VmUsrOrDefChapterManagement { ChapterObject = A }).ToList();

            bool gotLock = CompanyDbAccess.Lock;
            while (gotLock)
            {
                await Task.Yield();
                gotLock = CompanyDbAccess.Lock;
            }

            try
            {
                var lsrSR = await CompanyDbAccess.GetAllSmartflowRecords(sessionState);
                lstChapters = lsrSR.Select(A => new VmUsrOrsfSmartflows { SmartflowObject = mapper.Map(A, new UsrOrsfSmartflows()) }).ToList();
            }
            catch
            {
                Console.WriteLine("Error Caught");
            }


            if (!(selectedChapter.Name is null) & selectedChapter.Name != "")
            {
                partnerCaseTypeGroups = await partnerAccessService.GetPartnerCaseTypeGroups();
                ListP4WViews = await partnerAccessService.GetPartnerViews();

                SelectChapter(lstChapters
                                    .Where(C => C.SmartflowObject.CaseTypeGroup == selectedChapter.CaseTypeGroup)
                                    .Where(C => C.SmartflowObject.CaseType == selectedChapter.CaseType)
                                    .Where(C => C.SmartflowObject.SmartflowName == selectedChapter.Name)
                                    .Select(C => C.SmartflowObject)
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
                //var lstC = await chapterManagementService.GetAllChapters();
                //lstChapters = lstC.Select(A => new VmUsrOrDefChapterManagement { ChapterObject = A }).ToList();

                bool gotLock = CompanyDbAccess.Lock;
                while (gotLock)
                {
                    await Task.Yield();
                    gotLock = CompanyDbAccess.Lock;
                }

                var lsrSR = await CompanyDbAccess.GetAllSmartflowRecords(sessionState);
                lstChapters = lsrSR.Select(A => new VmUsrOrsfSmartflows { SmartflowObject = mapper.Map(A, new UsrOrsfSmartflows()) }).ToList();

            }
            else
            {
                var lst = selectedChapter.Items;
                Dictionary<int?, string> docTypes = new Dictionary<int?, string> { { 1, "Doc" },{ 4, "Form" }, {6, "Step" }, { 8, "Date" }, { 9, "Email" }, {11,"Doc" } , { 12, "Email" } };

                lstAll = lst.Select(L => new VmUsrOrDefChapterManagement { ChapterObject = L })
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
                                        .Where(A => A.ChapterObject.Type == "Doc")
                                        .Select(A => {
                                            A.DocType = dropDownChapterList.Where(D => D.Name.ToUpper() == A.ChapterObject.Name.ToUpper())
                                                                                        .Select(D => string.IsNullOrEmpty(docTypes[D.DocumentType]) ? "Doc" : docTypes[D.DocumentType])
                                                                                        .FirstOrDefault();
                                            A.ChapterObject.RescheduleDays = !string.IsNullOrEmpty(A.ChapterObject.AsName) && A.ChapterObject.RescheduleDays is null ? 0 : A.ChapterObject.RescheduleDays;
                                                        return A;
                                        })
                                        .ToList();

                }
                if (listType == "Fees" | listType == "All")
                {
                    lstFees = selectedChapter.Fees.Select(F => new VmFee { FeeObject = F }).ToList();


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
                if (listType == "TickerMessages" | listType == "All")
                {
                    ListVmTickerMessages = (selectedChapter.TickerMessages is null)
                                                    ? new List<VmTickerMessages>()
                                                    : selectedChapter
                                                            .TickerMessages
                                                            .Select(D => new VmTickerMessages { Message = D })
                                                            .OrderBy(D => D.Message.SeqNo)
                                                            .ToList();
                    TickerValidation();
                    
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


                bool gotLock = CompanyDbAccess.Lock;
                while (gotLock)
                {
                    await Task.Yield();
                    gotLock = CompanyDbAccess.Lock;
                }

                var lsrSR = await CompanyDbAccess.GetAllSmartflowRecords(sessionState);

                if(!(lsrSR is null))
                {
                    lstAltSystemChapters = lsrSR.Select(A => new VmUsrOrsfSmartflows { SmartflowObject = mapper.Map(A, new UsrOrsfSmartflows()) }).ToList();
                }
                


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

            var test = await RefreshChapterItems(navDisplay);

            await RefreshAltSystemChaptersList();

            AltChapterObject = lstAltSystemChapters
                                    .Where(A => A.SmartflowObject.SmartflowName == SelectedChapterObject.SmartflowName)
                                    .Where(A => A.SmartflowObject.CaseType == SelectedChapterObject.CaseType)
                                    .Where(A => A.SmartflowObject.CaseTypeGroup == SelectedChapterObject.CaseTypeGroup)
                                    .Select(A => A.SmartflowObject)
                                    .SingleOrDefault();

            CreateNewSmartflow = false;

            if (!(AltChapterObject is null))
            {
                altChapter = JsonConvert.DeserializeObject<VmChapter>(AltChapterObject.SmartflowData);

                var cItems = altChapter.Items;


                lstAltSystemChapterItems = cItems.Select(T => new VmUsrOrDefChapterManagement { ChapterObject = T, Compared = false }).ToList();

                //Compare header items
                CompareChapterToAltSytem();

                foreach (var item in lstAgendas)
                {
                    CompareChapterItemsToAltSytem(item);
                }

                foreach (var item in lstStatus)
                {
                    CompareChapterItemsToAltSytem(item);
                }

                foreach (var item in lstDocs)
                {
                    CompareChapterItemsToAltSytem(item);
                }

                var fItems = altChapter.Fees is null ? new List<Fee>() : altChapter.Fees;

                lstAltSystemFeeItems = fItems.Select(T => new VmFee { FeeObject = T, Compared = false }).ToList();

                foreach (var item in lstFees)
                {
                    CompareFeeItemsToAltSytem(item);
                }

                var dItems = altChapter.DataViews is null ? new List<DataViews>() : altChapter.DataViews;

                lstAltSystemDataViews = dItems.Select(T => new VmDataViews { DataView = T, Compared = false }).ToList();

                foreach (var item in ListVmDataViews)
                {
                    CompareDataViewsToAltSytem(item);
                }

                var tItems = altChapter.TickerMessages is null ? new List<TickerMessages>() : altChapter.TickerMessages;

                lstAltSystemTickerMessages = tItems.Select(T => new VmTickerMessages { Message = T, Compared = false }).ToList();

                foreach (var item in ListVmTickerMessages)
                {
                    CompareTickerMessagesToAltSytem(item);
                }

            }
            else
            {
                CreateNewSmartflow = true;

                ListVmTickerMessages = ListVmTickerMessages.Select(T => { T.ComparisonIcon = "times"; T.ComparisonResult = "No match"; return T; }).ToList();
                ListVmDataViews = ListVmDataViews.Select(T => { T.ComparisonIcon = "times"; T.ComparisonResult = "No match"; return T; }).ToList();
                lstFees = lstFees.Select(T => { T.ComparisonIcon = "times"; T.ComparisonResult = "No match"; return T; }).ToList();
                lstDocs = lstDocs.Select(T => { T.ComparisonIcon = "times"; T.ComparisonResult = "No match"; return T; }).ToList();
                lstStatus = lstStatus.Select(T => { T.ComparisonIcon = "times"; T.ComparisonResult = "No match"; return T; }).ToList();
                lstAgendas = lstAgendas.Select(T => { T.ComparisonIcon = "times"; T.ComparisonResult = "No match"; return T; }).ToList();
            }


            await InvokeAsync(() =>
            {
                StateHasChanged();
            });


            return true;
        }

        private async void CompareAllChapters()
        {
            lstChapters = lstChapters.Select(C => { C.ComparisonIcon = null; C.ComparisonResult = null; return C; }).ToList();

            var test = new string(SelectedChapterObject.SmartflowData);

            lstAltSystemChapters = new List<VmUsrOrsfSmartflows>();
            AltChapterObject = new UsrOrsfSmartflows();
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
                if(!(lstAltSystemChapters is null) && lstAltSystemChapters.Count > 0)
                {
                    foreach (var chapter in lstChapters)
                    {
                        var chapterItems = JsonConvert.DeserializeObject<VmChapter>(chapter.SmartflowObject.SmartflowData);

                        AltChapterObject = lstAltSystemChapters
                                            .Where(A => A.SmartflowObject.SmartflowName == chapter.SmartflowObject.SmartflowName)
                                            .Where(A => A.SmartflowObject.CaseType == chapter.SmartflowObject.CaseType)
                                            .Where(A => A.SmartflowObject.CaseTypeGroup == chapter.SmartflowObject.CaseTypeGroup)
                                            .Select(C => C.SmartflowObject)
                                            .SingleOrDefault();

                        if (AltChapterObject is null)
                        {
                            chapter.ComparisonResult = "No match";
                            chapter.ComparisonIcon = "times";
                        }
                        else
                        {
                            altChapter = JsonConvert.DeserializeObject<VmChapter>(AltChapterObject.SmartflowData);
                            
                            
                            if (!(altChapter.Items is null)!)
                            {
                                lstAltSystemChapterItems = altChapter.Items.Select(T => new VmUsrOrDefChapterManagement { ChapterObject = T }).ToList();

                                var vmChapterItems = chapterItems.Items is null 
                                                    ? new List<VmUsrOrDefChapterManagement>() 
                                                    : chapterItems.Items.Select(C => new VmUsrOrDefChapterManagement { ChapterObject = C }).ToList();

                                foreach (var item in vmChapterItems)
                                {
                                    CompareChapterItemsToAltSytem(item);
                                }


                                var fItems = altChapter.Fees is null ? new List<Fee>() : altChapter.Fees;

                                lstAltSystemFeeItems = fItems.Select(T => new VmFee { FeeObject = T, Compared = false }).ToList();

                                lstFees = chapterItems.Fees is null
                                                    ? new List<VmFee>()
                                                    : chapterItems.Fees.Select(C => new VmFee { FeeObject = C }).ToList();

                                foreach (var item in lstFees)
                                {
                                    CompareFeeItemsToAltSytem(item);
                                }

                                var dItems = altChapter.DataViews is null ? new List<DataViews>() : altChapter.DataViews;

                                lstAltSystemDataViews = dItems.Select(T => new VmDataViews { DataView = T, Compared = false }).ToList();

                                ListVmDataViews = chapterItems.DataViews is null
                                                    ? new List<VmDataViews>()
                                                    : chapterItems.DataViews.Select(C => new VmDataViews { DataView = C }).ToList();

                                foreach (var item in ListVmDataViews)
                                {
                                    CompareDataViewsToAltSytem(item);
                                }

                                var tItems = altChapter.TickerMessages is null ? new List<TickerMessages>() : altChapter.TickerMessages;

                                lstAltSystemTickerMessages = tItems.Select(T => new VmTickerMessages { Message = T, Compared = false }).ToList();

                                ListVmTickerMessages = chapterItems.TickerMessages is null
                                                    ? new List<VmTickerMessages>()
                                                    : chapterItems.TickerMessages.Select(C => new VmTickerMessages { Message = C }).ToList();

                                foreach (var item in ListVmTickerMessages)
                                {
                                    CompareTickerMessagesToAltSytem(item);
                                }

                                if (vmChapterItems.Where(C => C.ComparisonResult == "No match" | C.ComparisonResult == "Partial match").Count() > 0 | vmChapterItems.Count() != lstAltSystemChapterItems.Count())
                                {
                                    chapter.ComparisonResult = "Partial match";
                                    chapter.ComparisonIcon = "exclamation";
                                }
                                else if (lstFees.Where(C => C.ComparisonResult == "No match" | C.ComparisonResult == "Partial match").Count() > 0 | lstFees.Count() != lstAltSystemFeeItems.Count())
                                {
                                    chapter.ComparisonResult = "Partial match";
                                    chapter.ComparisonIcon = "exclamation";
                                }
                                else if (ListVmDataViews.Where(C => C.ComparisonResult == "No match" | C.ComparisonResult == "Partial match").Count() > 0 | ListVmDataViews.Count() != lstAltSystemDataViews.Count())
                                {
                                    chapter.ComparisonResult = "Partial match";
                                    chapter.ComparisonIcon = "exclamation";
                                }
                                else if (ListVmTickerMessages.Where(C => C.ComparisonResult == "No match" | C.ComparisonResult == "Partial match").Count() > 0 | ListVmTickerMessages.Count() != lstAltSystemTickerMessages.Count())
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
                            else
                            {
                                chapter.ComparisonResult = "Partial match";
                                chapter.ComparisonIcon = "exclamation";
                            }


                            
                            
                        }
                    }
                }
                else
                {
                    lstChapters = lstChapters.Select(T => { T.ComparisonIcon = "times"; T.ComparisonResult = "No match"; return T; }).ToList();
                }

               
            }
            return true;
        }

        private VmChapterComparison CompareChapterToAltSytem()
        {
            editChapterComparison = new VmChapterComparison { CurrentChapter = selectedChapter };

            if (altChapter is null)
            {
                editChapterComparison.ComparisonResult = "No match";
                editChapterComparison.ComparisonIcon = "times";
            }
            else
            {
                if (editChapterComparison.IsChapterMatch(altChapter))
                {
                    editChapterComparison.ComparisonResult = "Exact match";
                    editChapterComparison.ComparisonIcon = "check";

                }
                else
                {
                    editChapterComparison.ComparisonResult = "Partial match";
                    editChapterComparison.ComparisonIcon = "exclamation";

                }

            }

            return editChapterComparison;
        }


        private VmUsrOrDefChapterManagement CompareChapterItemsToAltSytem(VmUsrOrDefChapterManagement chapterItem)
        {
            var altObject = lstAltSystemChapterItems
                                .Where(A => A.ChapterObject.Type == chapterItem.ChapterObject.Type)
                                .Where(A => A.ChapterObject.Name == chapterItem.ChapterObject.Name)
                                .Where(A => !A.Compared)
                                .FirstOrDefault();

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

        private VmDataViews CompareDataViewsToAltSytem(VmDataViews dataView)
        {
            var altObject = lstAltSystemDataViews
                                .Where(A => A.DataView.ViewName == dataView.DataView.ViewName)
                                .Where(A => !A.Compared)
                                .SingleOrDefault();

            if (altObject is null)
            {
                dataView.ComparisonResult = "No match";
                dataView.ComparisonIcon = "times";
            }
            else
            {
                if (dataView.IsDataViewMatch(altObject))
                {
                    dataView.ComparisonResult = "Exact match";
                    dataView.ComparisonIcon = "check";
                }
                else
                {
                    dataView.ComparisonResult = "Partial match";
                    dataView.ComparisonIcon = "exclamation";
                }

            }

            return dataView;
        }

        private VmTickerMessages CompareTickerMessagesToAltSytem(VmTickerMessages tickerMessage)
        {
            var altObject = lstAltSystemTickerMessages
                                .Where(A => A.Message.Message == tickerMessage.Message.Message)
                                .Where(A => !A.Compared)
                                .FirstOrDefault();

            if (altObject is null)
            {
                tickerMessage.ComparisonResult = "No match";
                tickerMessage.ComparisonIcon = "times";
            }
            else
            {
                if (tickerMessage.IsTickerMessageMatch(altObject))
                {
                    tickerMessage.ComparisonResult = "Exact match";
                    tickerMessage.ComparisonIcon = "check";
                }
                else
                {
                    tickerMessage.ComparisonResult = "Partial match";
                    tickerMessage.ComparisonIcon = "exclamation";
                }

            }

            return tickerMessage;
        }


        private VmFee CompareFeeItemsToAltSytem(VmFee chapterItem)
        {
            var altObject = lstAltSystemFeeItems
                                .Where(A => A.FeeObject.FeeName == chapterItem.FeeObject.FeeName)
                                .Where(A => !A.Compared)
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

        private void PrepareTickerMessageForEdit(VmTickerMessages item, string header)
        {
            selectedList = header;
            EditTickerMessageObject = item;

            ShowTickerMessageDetailModal("Edit");
        }


        private void PrepareForInsert(string header, string type)
        {
            selectedList = type;

            editObject = new VmUsrOrDefChapterManagement { ChapterObject = new GenSmartflowItem() };
            editObject.ChapterObject.Type = (type == "Steps and Documents") ? "Doc" : type;
            editObject.ChapterObject.Action = "INSERT";
            
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
        //TODO: Change display to reflect the top 3 items and from-to dates
        private void PrepareTickerMessageForInsert(string header)
        {
            selectedList = header;
            EditTickerMessageObject = new VmTickerMessages { Message = new TickerMessages() };

            EditTickerMessageObject.Message.FromDate = DateTime.Now.ToString("yyyyMMdd");
            EditTickerMessageObject.Message.ToDate = DateTime.Now.ToString("yyyyMMdd");

            if (ListVmTickerMessages.Count > 0)
            {

                EditTickerMessageObject.Message.SeqNo = ListVmTickerMessages
                                                       .OrderByDescending(D => D.Message.SeqNo)
                                                       .Select(D => D.Message.SeqNo)
                                                       .FirstOrDefault() + 1;
            }
            else
            {
                EditTickerMessageObject.Message.SeqNo = 1;
            }


            ShowTickerMessageDetailModal("Insert");
        }


        private void PrepNewChapter()
        {
            editChapterObject = new UsrOrsfSmartflows();

            if (!(SelectedChapterObject.CaseTypeGroup == ""))
            {
                editChapterObject.CaseTypeGroup = selectedChapter.CaseTypeGroup;
            }
            else
            {
                editChapterObject.CaseTypeGroup = "";
            }

            if (!(SelectedChapterObject.CaseType == ""))
            {
                editChapterObject.CaseType = selectedChapter.CaseType;
            }
            else
            {
                editChapterObject.CaseType = "";
            }

            if (!string.IsNullOrWhiteSpace(selectedChapter.CaseTypeGroup) & !string.IsNullOrWhiteSpace(selectedChapter.CaseType))
            {
                editChapterObject.SeqNo = lstChapters
                                                  .Where(C => C.SmartflowObject.CaseType == selectedChapter.CaseType)
                                                  .Where(C => C.SmartflowObject.CaseTypeGroup == selectedChapter.CaseTypeGroup)
                                                  .OrderByDescending(C => C.SmartflowObject.SeqNo)
                                                  .Select(C => C.SmartflowObject.SeqNo)
                                                  .FirstOrDefault() + 1;
            }
            else
            {
                editChapterObject.SeqNo = 1;
            }
        }

        private void PrepareChapterForInsert()
        {
            PrepNewChapter();
            ShowChapterAddOrEditModel();
        }

        private void PrepareChapterForCopy()
        {
            PrepNewChapter();
            ShowChapterCopyModel();
        }

        private void PrepareCaseTypeForEdit(string caseType, string option)
        {
            editCaseType = caseType;
            isCaseTypeOrGroup = option;

            ShowCaseTypeEditModal();
        }

        private void PrepareChapterForEdit(UsrOrsfSmartflows chapter, string option)
        {
            editChapter = chapter;
            isCaseTypeOrGroup = option;

            ShowCaseTypeEditModal();
        }

        private async void PrepChapterList()
        {
            if (!(selectedChapter.CaseType == ""))
            {
                dropDownChapterList = await chapterManagementService.GetDocumentList(SelectedChapterObject.CaseType);
                TableDates = await chapterManagementService.GetDatabaseTableDateFields();
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
        /// Moves a sequecnce item up or down a list of type [GenSmartflowItem]
        /// </summary>
        /// <remarks>
        /// <para>Up: swaps the item with the preceding item in the lest by reducing sequence number by 1 </para>
        /// <para>Up: swaps the item with the following item in the lest by increasing sequence number by 1 </para>
        /// </remarks>
        /// <param name="selectobject">: current list item</param>
        /// <param name="listType">: Docs or Fees</param>
        /// <param name="direction">: Up or Down</param>
        /// <returns>No return</returns>
        protected async void MoveSeq(GenSmartflowItem selectobject, string listType, string direction)
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

                //case "Chapters":
                //    lstItems = lstChapters
                //                        .Where(A => A.ChapterObject.CaseTypeGroup == selectedChapter.CaseTypeGroup)
                //                        .Where(A => A.ChapterObject.CaseType == selectedChapter.CaseType)
                //                        .OrderBy(A => A.ChapterObject.SeqNo)
                //                        .ToList();
                //    break;
            }

            var swapItem = lstItems.Where(D => D.ChapterObject.SeqNo == (selectobject.SeqNo + incrementBy)).SingleOrDefault();
            if (!(swapItem is null))
            {
                selectobject.SeqNo += incrementBy;
                swapItem.ChapterObject.SeqNo = swapItem.ChapterObject.SeqNo + (incrementBy * -1);

                //if (listType == "Chapters")
                //{
                //    await chapterManagementService.UpdateMainItem(selectobject).ConfigureAwait(false);
                //    await chapterManagementService.UpdateMainItem(swapItem.ChapterObject).ConfigureAwait(false);
                //}

                SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(selectedChapter);
                await chapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);


                //keep track of time last updated ready for comparison by other sessions checking for updates
                appChapterState.SetLastUpdated(sessionState, selectedChapter);

            }

            await RefreshChapterItems(listType);
            await InvokeAsync(() =>
            {
                StateHasChanged();
            });


            seqMoving = false;

        }

        protected async void MoveSmartFlowSeq(UsrOrsfSmartflows selectobject, string listType, string direction)
        {
            seqMoving = true; //prevents changes to the form whilst process of changing seq is carried out

            var lstItems = new List<VmUsrOrsfSmartflows>();
            int incrementBy;

            incrementBy = (direction.ToLower() == "up" ? -1 : 1);

            rowChanged = (int)(selectobject.SeqNo + incrementBy);

            lstItems = lstChapters
                        .Where(A => A.SmartflowObject.CaseTypeGroup == selectedChapter.CaseTypeGroup)
                        .Where(A => A.SmartflowObject.CaseType == selectedChapter.CaseType)
                        .OrderBy(A => A.SmartflowObject.SeqNo)
                        .ToList();


            var swapItem = lstItems.Where(D => D.SmartflowObject.SeqNo == (selectobject.SeqNo + incrementBy)).SingleOrDefault();
            if (!(swapItem is null))
            {
                selectobject.SeqNo += incrementBy;
                swapItem.SmartflowObject.SeqNo = swapItem.SmartflowObject.SeqNo + (incrementBy * -1);

                await chapterManagementService.UpdateMainItem(selectobject).ConfigureAwait(false);
                await chapterManagementService.UpdateMainItem(swapItem.SmartflowObject).ConfigureAwait(false);
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

                SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(selectedChapter);
                await chapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);


                //keep track of time last updated ready for comparison by other sessions checking for updates
                appChapterState.SetLastUpdated(sessionState, selectedChapter);

            }

            await RefreshChapterItems(listType);
            await InvokeAsync(() =>
            {
                StateHasChanged();
            });


            seqMoving = false;

        }

        protected async void MoveMessageSeqNo(TickerMessages selectobject, string listType, string direction)
        {
            seqMoving = true; //prevents changes to the form whilst process of changing seq is carried out

            int incrementBy;

            incrementBy = (direction.ToLower() == "up" ? -1 : 1);

            rowChanged = (int)(selectobject.SeqNo + incrementBy);

            var swapItem = ListVmTickerMessages.Where(D => D.Message.SeqNo == (selectobject.SeqNo + incrementBy)).SingleOrDefault();
            if (!(swapItem is null))
            {
                selectobject.SeqNo += incrementBy;
                swapItem.Message.SeqNo = swapItem.Message.SeqNo + (incrementBy * -1);

                SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(selectedChapter);
                await chapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);


                //keep track of time last updated ready for comparison by other sessions checking for updates
                appChapterState.SetLastUpdated(sessionState, selectedChapter);

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

                SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(selectedChapter);
                await chapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);


                //keep track of time last updated ready for comparison by other sessions checking for updates
                appChapterState.SetLastUpdated(sessionState, selectedChapter);

            }

            await RefreshChapterItems(listType);
            await InvokeAsync(() =>
            {
                StateHasChanged();
            });


            seqMoving = false;

        }

        private List<int?> GetSeqNumbers(string listType)
        {
            var listItems = new List<int?>();

            switch (listType)
            {
                case "Docs":
                    listItems = lstDocs.OrderBy(D => D.ChapterObject.SeqNo).Select(D => D.ChapterObject.SeqNo).ToList();
                    break;
                case "Status":
                    listItems = lstStatus.OrderBy(D => D.ChapterObject.SeqNo).Select(D => D.ChapterObject.SeqNo).ToList();
                    break;
                case "Chapters":
                    listItems = lstChapters
                        
                        .Where(D => D.SmartflowObject.CaseType == selectedChapter.CaseType)
                        .Where(D => D.SmartflowObject.CaseTypeGroup == selectedChapter.CaseTypeGroup)
                        .OrderBy(D => D.SmartflowObject.SeqNo)
                        .Select(D => D.SmartflowObject.SeqNo).ToList();
                    break;
            }

            return listItems;
        }


        private List<VmUsrOrDefChapterManagement> GetRelevantChapterList(string listType)
        {
            var listItems = new List<VmUsrOrDefChapterManagement>();

            switch (listType)
            {
                case "Docs":
                    listItems = lstDocs;
                    break;
                case "Status":
                    listItems = lstStatus;
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

            SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(selectedChapter);
            await chapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);

            
            //keep track of time last updated ready for comparison by other sessions checking for updates
            appChapterState.SetLastUpdated(sessionState, selectedChapter);

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

            SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(selectedChapter);
            await chapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);


            //keep track of time last updated ready for comparison by other sessions checking for updates
            appChapterState.SetLastUpdated(sessionState, selectedChapter);

            await RefreshChapterItems(ListType);

            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }

        protected async void CondenseMessageSeqNo(string ListType)
        {
            await RefreshChapterItems(ListType);

            int seqNo = 0;

            foreach (var item in ListVmTickerMessages.OrderBy(A => A.Message.SeqNo))
            {
                seqNo += 1;
                item.Message.SeqNo = seqNo;
            }

            SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(selectedChapter);
            await chapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);


            //keep track of time last updated ready for comparison by other sessions checking for updates
            appChapterState.SetLastUpdated(sessionState, selectedChapter);

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

            SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(selectedChapter);
            await chapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);


            //keep track of time last updated ready for comparison by other sessions checking for updates
            appChapterState.SetLastUpdated(sessionState, selectedChapter);

            await RefreshChapterItems(ListType);

            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }

        protected async void CondenseChapterSeq()
        {
            var ListItems = lstChapters
                                .Where(C => C.SmartflowObject.CaseTypeGroup == selectedChapter.CaseTypeGroup)
                                .Where(C => C.SmartflowObject.CaseType == selectedChapter.CaseType)
                                .ToList();

            int seqNo = 0;

            foreach (VmUsrOrsfSmartflows item in ListItems.OrderBy(A => A.SmartflowObject.SeqNo))
            {
                seqNo += 1;
                item.SmartflowObject.SeqNo = seqNo;

                await chapterManagementService.UpdateMainItem(item.SmartflowObject);
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
            parameters.Add("sessionState", sessionState);
            parameters.Add("CompanyDbAccess", CompanyDbAccess);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-chapter-chapter"
            };
            string title = $"Copy {selectedChapter.Name} to...";
            Modal.Show<ChapterCopy>(title, parameters, options);
        }


        protected void ShowChapterAddOrEditModel()
        {
            Action Action = RefreshChapters;

            var parameters = new ModalParameters();
            parameters.Add("TaskObject", editChapterObject);
            parameters.Add("DataChanged", Action);
            parameters.Add("AllObjects", lstChapters);
            parameters.Add("sessionState", sessionState);
            parameters.Add("CompanyDbAccess", CompanyDbAccess);

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
            parameters.Add("TaskObject", (isCaseTypeOrGroup == "Chapter") ? editChapter.SmartflowData : editCaseType);
            parameters.Add("originalName", (isCaseTypeOrGroup == "Chapter") ? editChapter.SmartflowData : editCaseType);
            if (isCaseTypeOrGroup == "Chapter")
            {
                parameters.Add("Chapter", editChapter);
            }
            parameters.Add("DataChanged", Action);
            parameters.Add("isCaseTypeOrGroup", isCaseTypeOrGroup);
            parameters.Add("caseTypeGroupName", selectedChapter.CaseTypeGroup);
            parameters.Add("ListChapters", lstChapters);
            parameters.Add("CompanyDbAccess", CompanyDbAccess);
            parameters.Add("sessionState", sessionState);

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
            Action RefreshDocList = this.RefreshDocList;

            var copyObject = new GenSmartflowItem
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
                Action = editObject.ChapterObject.Action,
                TrackingMethod = editObject.ChapterObject.TrackingMethod,
                ChaserDesc = editObject.ChapterObject.ChaserDesc,
                RescheduleDataItem = editObject.ChapterObject.RescheduleDataItem,
                MilestoneStatus = editObject.ChapterObject.MilestoneStatus
        };

            var parameters = new ModalParameters();
            parameters.Add("TaskObject", editObject.ChapterObject);
            parameters.Add("RefreshDocList", RefreshDocList);
            parameters.Add("CopyObject", copyObject);
            parameters.Add("DataChanged", action);
            parameters.Add("selectedList", selectedList);
            parameters.Add("dropDownChapterList", dropDownChapterList);
            parameters.Add("TableDates", TableDates);
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

        public async void RefreshViewList()
        {
            ListP4WViews = await partnerAccessService.GetPartnerViews();
            StateHasChanged();
        }

        protected void ShowDataViewDetailModal(string option)
        {
            Action action = RefreshSelectedList;
            Action refreshViewList = RefreshViewList; 

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
            parameters.Add("PartnerAccessService", partnerAccessService);
            parameters.Add("RefreshViewList", refreshViewList);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-chapter-data"
            };

            Modal.Show<DataViewDetail>("Data View", parameters, options);
        }

        protected void ShowTickerMessageDetailModal(string option)
        {
            Action action = RefreshSelectedList;

            var copyObject = new TickerMessages
            {
                SeqNo = EditTickerMessageObject.Message.SeqNo,
                Message = EditTickerMessageObject.Message.Message,
                FromDate = EditTickerMessageObject.Message.FromDate,
                ToDate = EditTickerMessageObject.Message.ToDate
            };

            var parameters = new ModalParameters();
            parameters.Add("TaskObject", EditTickerMessageObject.Message);
            parameters.Add("CopyObject", copyObject);
            parameters.Add("DataChanged", action);
            parameters.Add("SelectedChapter", selectedChapter);
            parameters.Add("SelectedChapterObject", SelectedChapterObject);
            parameters.Add("Option", option);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-chapter-item"
            };

            Modal.Show<TickerMessageDetail>("Ticker Messages", parameters, options);
        }

        private void PrepareAttachmentForAdd(VmUsrOrDefChapterManagement item)
        {
            selectedList = "New Attachement";
            editObject = item;
            attachObject = null;

            ShowChapterAttachmentModal();
        }

        private void PrepareAttachmentForEdit(VmUsrOrDefChapterManagement item, LinkedItems LinkedItems)
        {
            selectedList = "Edit Attachement";
            editObject = item;
            attachObject = LinkedItems;

            ShowChapterAttachmentModal();
        }

        protected void ShowChapterAttachmentModal()
        {
            Action action = RefreshSelectedList;
            Action RefreshDocList = this.RefreshDocList;

            var copyObject = new GenSmartflowItem
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
                LinkedItems = editObject.ChapterObject.LinkedItems is null ? new List<LinkedItems>() : editObject.ChapterObject.LinkedItems
            };

           
            var attachment = attachObject is null ? new LinkedItems { Action = "INSERT"} : attachObject;


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
            parameters.Add("CompanyDbAccess", CompanyDbAccess);
            parameters.Add("sessionState", sessionState);
            parameters.Add("RefreshDocList", RefreshDocList);
            parameters.Add("Attachment", attachment);
            parameters.Add("TableDates", TableDates);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-chapter-doc"
            };

            Modal.Show<ChapterAttachments>("Linked Item", parameters, options);
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

            string itemName = (string.IsNullOrEmpty(selectedChapterItem.ChapterObject.AltDisplayName) ? selectedChapterItem.ChapterObject.Name : selectedChapterItem.ChapterObject.AltDisplayName);
            string itemType = selectedChapterItem.ChapterObject.Type;

            Action SelectedDeleteAction = HandleChapterDetailDelete;
            var parameters = new ModalParameters();
            parameters.Add("ItemName", itemName);
            parameters.Add("ModalHeight", "300px");
            parameters.Add("ModalWidth", "500px");
            parameters.Add("DeleteAction", SelectedDeleteAction);
            parameters.Add("InfoText", $"Are you sure you wish to delete the '{itemName}' {itemType.ToLower()}? ");

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal"
            };

            Modal.Show<ModalDelete>($"Delete {itemType}", parameters, options);
        }

        protected void PrepareChapterFeeDelete(VmFee selectedChapterItem)
        {
            editFeeObject = selectedChapterItem;

            string itemName = selectedChapterItem.FeeObject.FeeName;

            Action SelectedDeleteAction = HandleChapterFeeDelete;
            var parameters = new ModalParameters();
            parameters.Add("ItemName", itemName);
            parameters.Add("ModalHeight", "300px");
            parameters.Add("ModalWidth", "500px");
            parameters.Add("DeleteAction", SelectedDeleteAction);
            parameters.Add("InfoText", $"Are you sure you wish to delete the '{itemName}' fee?");

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal"
            };

            Modal.Show<ModalDelete>("Delete Fee", parameters, options);
        }


        protected void PrepareDataViewDelete(VmDataViews selectedDataView)
        {
            EditDataViewObject = selectedDataView;

            string itemName = (string.IsNullOrEmpty(selectedDataView.DataView.DisplayName) ? selectedDataView.DataView.ViewName : selectedDataView.DataView.DisplayName);

            Action SelectedDeleteAction = HandleDataViewDelete;
            var parameters = new ModalParameters();
            parameters.Add("ItemName", itemName);
            parameters.Add("ModalHeight", "300px");
            parameters.Add("ModalWidth", "500px");
            parameters.Add("DeleteAction", SelectedDeleteAction);
            parameters.Add("InfoText", $"Are you sure you wish to delete the '{itemName}' data view?");

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal"
            };

            Modal.Show<ModalDelete>($"Delete Data View", parameters, options);
        }

        protected void PrepareTickerMessageDelete(VmTickerMessages selectedTickerMessage)
        {
            EditTickerMessageObject = selectedTickerMessage;
            
            string itemName = selectedTickerMessage.Message.Message;

            Action SelectedDeleteAction = HandleTickerMessageDelete;
            var parameters = new ModalParameters();
            parameters.Add("ItemName", itemName);
            parameters.Add("ModalHeight", "300px");
            parameters.Add("ModalWidth", "500px");
            parameters.Add("DeleteAction", SelectedDeleteAction);
            parameters.Add("InfoText", $"Are you sure you wish to delete the message?");

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal"
            };

            Modal.Show<ModalDelete>($"Delete Message", parameters, options);
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

        protected void ShowTickerMessageDisplayModal(VmTickerMessages selectedObject)
        {
            var parameters = new ModalParameters();
            parameters.Add("Object", selectedObject);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-chapter-comparison"
            };

            Modal.Show<TickerMessageDisplay>("Ticker Message", parameters, options);
        }

        protected void PrepareChapterDelete(VmUsrOrsfSmartflows selectedChapterItem)
        {
            editChapter = selectedChapterItem.SmartflowObject;

            string itemName = selectedChapterItem.SmartflowObject.SmartflowName;

            Action SelectedDeleteAction = HandleChapterDelete;
            var parameters = new ModalParameters();
            parameters.Add("ItemName", itemName);
            parameters.Add("ModalHeight", "300px");
            parameters.Add("ModalWidth", "500px");
            parameters.Add("DeleteAction", SelectedDeleteAction);
            parameters.Add("InfoText", $"Are you sure you wish to delete the '{itemName}' smartflow?");


            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal"
            };

            Modal.Show<ModalDelete>("Delete Smartflow", parameters, options);
        }

        private async void HandleBgImageFileSelection(IFileListEntry[] entryFiles)
        {
            FileHelper.CustomPath = $"wwwroot/images/Companies/{sessionState.Company.CompanyName}/BackgroundImages";
            ListFilesForBgImages = FileHelper.GetFileList();

            var files = new List<IFileListEntry>();
            IList<string> fileErrorDescs = new List<string>();

            foreach (var file in entryFiles)
            {
                if (file != null)
                {
                    if (!(file.Name.Contains(".jpg") || file.Name.Contains(".png")))
                    {
                        fileErrorDescs.Add($"The file: {file.Name} is not a valid image. Image files must be either .jpg or .png");
                    }
                    else
                    {
                        if (ListFilesForBgImages.Where(F => F.FileName == file.Name).FirstOrDefault() is null)
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

            if (fileErrorDescs.Count > 0)
            {
                ShowErrorModal("File Upload", "The following errors occured during the upload:", fileErrorDescs);
            }

            ListFilesForBgImages = FileHelper.GetFileList();

            StateHasChanged();
        }

        protected void PrepareBgImageForDelete(FileDesc selectedFile)
        {
            FileHelper.CustomPath = $"wwwroot/images/Companies/{sessionState.Company.CompanyName}/BackgroundImages";

            SelectedFileDescription = selectedFile;

            string itemName = selectedFile.FileName;

            Action SelectedDeleteAction = HandleDeleteBgImageFile;
            var parameters = new ModalParameters();
            parameters.Add("ItemName", itemName);
            parameters.Add("ModalHeight", "300px");
            parameters.Add("ModalWidth", "500px");
            parameters.Add("DeleteAction", SelectedDeleteAction);
            parameters.Add("InfoText", $"Are you sure you wish to delete the '{itemName}' ?");

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal"
            };

            Modal.Show<ModalDelete>("Delete Backup File", parameters, options);
        }

        private void HandleDeleteBgImageFile()
        {
            DeleteBgImageFile(SelectedFileDescription);
        }

        private async void SelectBgImage(FileDesc fileDesc)
        {
            selectedChapter.BackgroundImage = fileDesc.FileURL.Replace("/wwwroot", "");
            selectedChapter.BackgroundImageName = fileDesc.FileName;

            SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(selectedChapter);

            await chapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);


            //keep track of time last updated ready for comparison by other sessions checking for updates
            appChapterState.SetLastUpdated(sessionState, selectedChapter);

            sessionState.SetTempBackground(selectedChapter.BackgroundImage.Replace("/wwwroot", ""), NavigationManager.Uri);
            sessionState.RefreshHome?.Invoke();

            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }

        

        private void DeleteBgImageFile(FileDesc file)
        {
            FileHelper.CustomPath = $"wwwroot/images/Companies/{sessionState.Company.CompanyName}/BackgroundImages";

            ChapterFileUpload.DeleteFile(file.FilePath);

            ListFilesForBgImages = FileHelper.GetFileList();
            StateHasChanged();
        }

        protected void PrepareBackUpForDelete(FileDesc selectedFile)
        {
            SetSmartflowFilePath();

            SelectedFileDescription = selectedFile;

            string itemName = selectedFile.FileName;

            Action SelectedDeleteAction = HandleDeleteBackupFile;
            var parameters = new ModalParameters();
            parameters.Add("ItemName", itemName);
            parameters.Add("ModalHeight", "300px");
            parameters.Add("ModalWidth", "500px");
            parameters.Add("DeleteAction", SelectedDeleteAction);
            parameters.Add("InfoText", $"Are you sure you wish to delete the '{itemName}' backup file?");

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal"
            };

            Modal.Show<ModalDelete>("Delete Backup File", parameters, options);
        }

        private async void PrepareHeaderForComparison()
        {
            await CompareSelectedChapterToAltSystem();
            
            ShowHeaderComparisonModal();
        }

        protected void ShowHeaderComparisonModal()
        {
            Action Compare = CompareChapterItemsToAltSytemAction;

            var parameters = new ModalParameters();
            parameters.Add("Object", editChapterComparison);
            parameters.Add("ComparisonRefresh", Compare);
            parameters.Add("sessionState", sessionState);
            parameters.Add("CurrentSysParentId", selectedChapterId);
            parameters.Add("AlternateSysParentId", altSysSelectedChapterId);
            parameters.Add("CurrentChapter", selectedChapter);
            parameters.Add("AltChapter", altChapter);
            parameters.Add("CurrentChapterRow", SelectedChapterObject);
            parameters.Add("AltChapterRow", AltChapterObject);
            parameters.Add("CompanyDbAccess", CompanyDbAccess);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-chapter-comparison"
            };

            Modal.Show<ChapterHeaderComparison>("Synchronise Smartflow Item", parameters, options);
        }



        protected void PrepareDeleteAltObject(VmUsrOrDefChapterManagement selectedItem)
        {
            editObject = selectedItem;

            Action SelectedDeleteAction = HandleAltDetailDelete;
            var parameters = new ModalParameters();
            parameters.Add("ItemName", editObject.ChapterObject.Name);
            parameters.Add("ModalHeight", "300px");
            parameters.Add("ModalWidth", "500px");
            parameters.Add("DeleteAction", SelectedDeleteAction);
            parameters.Add("InfoText", $"Are you sure you wish to delete the '{editObject.ChapterObject.Name}' {editObject.ChapterObject.Type}?");

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal"
            };

            Modal.Show<ModalDelete>($"Delete {editObject.ChapterObject.Type}", parameters, options);
        }


        protected void PrepareDeleteAltDataView(VmDataViews selectedItem)
        {
            EditDataViewObject = selectedItem;

            Action SelectedDeleteAction = HandleAltDataViewDelete;
            var parameters = new ModalParameters();
            parameters.Add("ItemName", editObject.ChapterObject.Name);
            parameters.Add("ModalHeight", "300px");
            parameters.Add("ModalWidth", "500px");
            parameters.Add("DeleteAction", SelectedDeleteAction);
            parameters.Add("InfoText", $"Are you sure you wish to delete the '{EditDataViewObject.DataView.ViewName}' View?");

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal"
            };

            Modal.Show<ModalDelete>($"Delete Data View", parameters, options);
        }


        protected void PrepareDeleteAltFee(VmFee selectedItem)
        {
            editFeeObject = selectedItem;

            Action SelectedDeleteAction = HandleAltFeeDelete;
            var parameters = new ModalParameters();
            parameters.Add("ItemName", editObject.ChapterObject.Name);
            parameters.Add("ModalHeight", "300px");
            parameters.Add("ModalWidth", "500px");
            parameters.Add("DeleteAction", SelectedDeleteAction);
            parameters.Add("InfoText", $"Are you sure you wish to delete the '{editFeeObject.FeeObject.FeeName}' Fee?");

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal"
            };

            Modal.Show<ModalDelete>($"Delete Fee", parameters, options);
        }

        protected void PrepareDeleteAltMessage(VmTickerMessages selectedItem)
        {
            EditTickerMessageObject = selectedItem;

            Action SelectedDeleteAction = HandleAltMessageDelete;
            var parameters = new ModalParameters();
            parameters.Add("ItemName", editObject.ChapterObject.Name);
            parameters.Add("ModalHeight", "300px");
            parameters.Add("ModalWidth", "500px");
            parameters.Add("DeleteAction", SelectedDeleteAction);
            parameters.Add("InfoText", $"Are you sure you wish to delete this message?");

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal"
            };

            Modal.Show<ModalDelete>($"Delete Message", parameters, options);
        }


        private async void HandleAltDetailDelete()
        {
            await sessionState.SwitchSelectedSystem();
            altChapter.Items.Remove(editObject.ChapterObject);
            AltChapterObject.SmartflowData = JsonConvert.SerializeObject(altChapter);
            await chapterManagementService.Update(AltChapterObject);

            await sessionState.ResetSelectedSystem();

            await CompareSelectedChapterToAltSystem();
        }

        private async void HandleAltFeeDelete()
        {
            await sessionState.SwitchSelectedSystem();
            altChapter.Fees.Remove(editFeeObject.FeeObject);
            AltChapterObject.SmartflowData = JsonConvert.SerializeObject(altChapter);
            await chapterManagementService.Update(AltChapterObject);

            await sessionState.ResetSelectedSystem();

            await CompareSelectedChapterToAltSystem();
        }

        private async void HandleAltDataViewDelete()
        {
            await sessionState.SwitchSelectedSystem();
            altChapter.DataViews.Remove(EditDataViewObject.DataView);
            AltChapterObject.SmartflowData = JsonConvert.SerializeObject(altChapter);
            await chapterManagementService.Update(AltChapterObject);

            await sessionState.ResetSelectedSystem();

            await CompareSelectedChapterToAltSystem();
        }

        private async void HandleAltMessageDelete()
        {
            await sessionState.SwitchSelectedSystem();
            altChapter.TickerMessages.Remove(EditTickerMessageObject.Message);
            AltChapterObject.SmartflowData = JsonConvert.SerializeObject(altChapter);
            await chapterManagementService.Update(AltChapterObject);

            await sessionState.ResetSelectedSystem();

            await CompareSelectedChapterToAltSystem();
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
            parameters.Add("CompanyDbAccess", CompanyDbAccess);
            parameters.Add("CreateNewSmartflow", CreateNewSmartflow);

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
            Action Compare = CompareChapterItemsToAltSytemAction;

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
            parameters.Add("CompanyDbAccess", CompanyDbAccess);
            parameters.Add("CreateNewSmartflow", CreateNewSmartflow);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-chapter-comparison"
            };

            Modal.Show<ChapterFeeComparison>("Synchronise Smartflow Item", parameters, options);
        }

        private void PrepareDataViewForComparison(VmDataViews selectedItem)
        {
            EditDataViewObject = selectedItem;

            ShowDataViewComparisonModal();
        }

        private void PrepareTickerMessageForComparison(VmTickerMessages selectedItem)
        {
            EditTickerMessageObject = selectedItem;

            ShowTickerMessageComparisonModal();
        }

        protected void ShowDataViewComparisonModal()
        {
            Action Compare = CompareChapterItemsToAltSytemAction;

            var parameters = new ModalParameters();
            parameters.Add("Object", EditDataViewObject);
            parameters.Add("ComparisonRefresh", Compare);
            parameters.Add("sessionState", sessionState);
            parameters.Add("CurrentSysParentId", selectedChapterId);
            parameters.Add("AlternateSysParentId", altSysSelectedChapterId);
            parameters.Add("CurrentChapter", selectedChapter);
            parameters.Add("AltChapter", altChapter);
            parameters.Add("CurrentChapterRow", SelectedChapterObject);
            parameters.Add("AltChapterRow", AltChapterObject);
            parameters.Add("CompanyDbAccess", CompanyDbAccess);
            parameters.Add("CreateNewSmartflow", CreateNewSmartflow);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-chapter-comparison"
            };

            Modal.Show<ChapterDataViewComparison>("Synchronise Smartflow Item", parameters, options);
        }

        protected void ShowTickerMessageComparisonModal()
        {
            Action Compare = CompareChapterItemsToAltSytemAction;

            var parameters = new ModalParameters();
            parameters.Add("Object", EditTickerMessageObject);
            parameters.Add("ComparisonRefresh", Compare);
            parameters.Add("sessionState", sessionState);
            parameters.Add("CurrentSysParentId", selectedChapterId);
            parameters.Add("AlternateSysParentId", altSysSelectedChapterId);
            parameters.Add("CurrentChapter", selectedChapter);
            parameters.Add("AltChapter", altChapter);
            parameters.Add("CurrentChapterRow", SelectedChapterObject);
            parameters.Add("AltChapterRow", AltChapterObject);
            parameters.Add("CompanyDbAccess", CompanyDbAccess);
            parameters.Add("CreateNewSmartflow",CreateNewSmartflow);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-chapter-comparison"
            };

            Modal.Show<TickerMessageComparison>("Synchronise Smartflow Item", parameters, options);
        }


        private async void HandleChapterDetailDelete()
        {

            selectedChapter.Items.Remove(editObject.ChapterObject);
            SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(selectedChapter);
            await chapterManagementService.Update(SelectedChapterObject);

            //keep track of time last updated ready for comparison by other sessions checking for updates
            appChapterState.SetLastUpdated(sessionState, selectedChapter);

            CondenseSeq(navDisplay);

            StateHasChanged();
        }

        private async void HandleChapterFeeDelete()
        {
            selectedChapter.Fees.Remove(editFeeObject.FeeObject);
            SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(selectedChapter);
            await chapterManagementService.Update(SelectedChapterObject);

            //keep track of time last updated ready for comparison by other sessions checking for updates
            appChapterState.SetLastUpdated(sessionState, selectedChapter);

            await RefreshChapterItems(navDisplay);
            StateHasChanged();
        }

        private async void HandleDataViewDelete()
        {
            selectedChapter.DataViews.Remove(EditDataViewObject.DataView);
            SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(selectedChapter);
            await chapterManagementService.Update(SelectedChapterObject);

            //keep track of time last updated ready for comparison by other sessions checking for updates
            appChapterState.SetLastUpdated(sessionState, selectedChapter);

            await RefreshChapterItems(navDisplay);
            StateHasChanged();
        }

        private async void HandleTickerMessageDelete()
        {
            selectedChapter.TickerMessages.Remove(EditTickerMessageObject.Message);
            SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(selectedChapter);
            await chapterManagementService.Update(SelectedChapterObject);

            //keep track of time last updated ready for comparison by other sessions checking for updates
            appChapterState.SetLastUpdated(sessionState, selectedChapter);

            await RefreshChapterItems(navDisplay);
            StateHasChanged();
        }

        private async void HandleChapterDelete()
        {
            await chapterManagementService.Delete(editChapter.Id);


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
                                        .Where(A => A.SmartflowObject.SmartflowName == SelectedChapterObject.SmartflowName)
                                        .Where(A => A.SmartflowObject.CaseType == SelectedChapterObject.CaseType)
                                        .Where(A => A.SmartflowObject.CaseTypeGroup == SelectedChapterObject.CaseTypeGroup)
                                        .Select(C => C.SmartflowObject)
                                        .SingleOrDefault();

            if (AltChapterObject is null)
            {
                var newAltChapterObject = new UsrOrsfSmartflows
                {
                    SeqNo = SelectedChapterObject.SeqNo,
                    CaseTypeGroup = SelectedChapterObject.CaseTypeGroup,
                    CaseType = SelectedChapterObject.CaseType,
                    SmartflowName = SelectedChapterObject.SmartflowName,
                    SmartflowData = SelectedChapterObject.SmartflowData,
                    VariantName = SelectedChapterObject.VariantName,
                    VariantNo = SelectedChapterObject.VariantNo
                };

                var returnObject = await chapterManagementService.Add(newAltChapterObject);
                newAltChapterObject.Id = returnObject.Id;

                bool gotLock = CompanyDbAccess.Lock;
                while (gotLock)
                {
                    await Task.Yield();
                    gotLock = CompanyDbAccess.Lock;
                }

                await CompanyDbAccess.SaveSmartFlowRecord(newAltChapterObject, sessionState);
            }
            else
            {
                AltChapterObject.SmartflowData = SelectedChapterObject.SmartflowData;

                await chapterManagementService.Update(AltChapterObject);
            }


            
            await sessionState.ResetSelectedSystem();

            await RefreshCompararisonAllChapters();

            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }


        protected void PrepareChapterCreateOnAlt(UsrOrsfSmartflows selectedChapter)
        {
            SelectedChapterObject = selectedChapter;

            string infoText = $"Do you wish to sync this smartflow to {(sessionState.selectedSystem == "Live" ? "Dev" : "Live")}.";

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
            SetSmartflowFilePath();

            var fileName = selectedChapter.Name + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt";

            ChapterFileUpload.WriteChapterToFile(SelectedChapterObject.SmartflowData, fileName);

            GetSeletedChapterFileList();
            StateHasChanged();
        }

        


        private async void HandleBackupFileSelection(IFileListEntry[] entryFiles)
        {
            SetSmartflowFilePath();
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
                        if(ListFilesForBackups.Where(F => F.FileName == file.Name).FirstOrDefault() is null)
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
            ListFilesForBackups = ChapterFileUpload.GetFileListForChapter();
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
                List<int?> listItems = GetSeqNumbers(listType);

                bool isValid = true;

                for (int i = 0; i < listItems.Count; i++)
                {
                    if (listItems[i] != i + 1)
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

        private bool MessagesSeqNoIsValid()
        {
            if (seqMoving == false | compareSystems == true)
            {

                bool isValid = true;

                for (int i = 0; i < ListVmTickerMessages.Count; i++)
                {
                    if (ListVmTickerMessages[i].Message.SeqNo != i + 1)
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
            var selectedCopyItems = new VmChapter { Items = new List<GenSmartflowItem>()
                                                    , Fees = new List<Fee>()
                                                    , DataViews = new List<DataViews>()
                                                    , TickerMessages = new List<TickerMessages>()};

            if (!(AltChapterObject is null))
            {
                if (!string.IsNullOrEmpty(AltChapterObject.SmartflowData))
                {
                    selectedCopyItems = JsonConvert.DeserializeObject<VmChapter>(AltChapterObject.SmartflowData);
                }
            }
            else
            {
                AltChapterObject = new UsrOrsfSmartflows 
                                    { 
                                        CaseType = SelectedChapterObject.CaseType
                                        ,CaseTypeGroup = SelectedChapterObject.CaseTypeGroup
                                        ,SeqNo = SelectedChapterObject.SeqNo
                                        ,SmartflowName = SelectedChapterObject.SmartflowName
                                        ,VariantName = SelectedChapterObject.VariantName
                                        ,VariantNo = SelectedChapterObject.VariantNo
                                    };
            }


            if (option == "Agenda" | option == "All")
            {
                foreach (var item in selectedCopyItems.Items.Where(C => C.Type == "Agenda").ToList())
                {
                    selectedCopyItems.Items.Remove(item);
                }

                selectedCopyItems.Items.AddRange(selectedChapter.Items.Where(C => C.Type == "Agenda").ToList());
            }



            if (option == "Status" | option == "All")
            {
                foreach (var item in selectedCopyItems.Items.Where(C => C.Type == "Status").ToList())
                {
                    selectedCopyItems.Items.Remove(item);
                }

                selectedCopyItems.Items.AddRange(selectedChapter.Items.Where(C => C.Type == "Status").ToList());
            }



            if (option == "Docs" | option == "All")
            {
                foreach (var item in selectedCopyItems.Items.Where(C => C.Type == "Doc").ToList())
                {
                    selectedCopyItems.Items.Remove(item);
                }

                selectedCopyItems.Items.AddRange(selectedChapter.Items.Where(C => C.Type == "Doc").ToList());
            }


            if (option == "Fees" | option == "All")
            {
                foreach (var item in selectedCopyItems.Fees.ToList())
                {
                    selectedCopyItems.Fees.Remove(item);
                }

                selectedCopyItems.Fees.AddRange(selectedChapter.Fees.ToList());
            }


            if (option == "DataViews" | option == "All")
            {
                foreach (var item in selectedCopyItems.DataViews.ToList())
                {
                    selectedCopyItems.DataViews.Remove(item);
                }

                selectedCopyItems.DataViews.AddRange(selectedChapter.DataViews.ToList());
            }

            if (option == "TickerMessages" | option == "All")
            {
                foreach (var item in selectedCopyItems.TickerMessages.ToList())
                {
                    selectedCopyItems.TickerMessages.Remove(item);
                }

                selectedCopyItems.TickerMessages.AddRange(selectedChapter.TickerMessages.ToList());
            }

            AltChapterObject.SmartflowData = JsonConvert.SerializeObject(new VmChapter
            {
                CaseTypeGroup = SelectedChapterObject.CaseTypeGroup,
                CaseType = SelectedChapterObject.CaseType,
                Name = SelectedChapterObject.SmartflowName,
                SeqNo = SelectedChapterObject.SeqNo.GetValueOrDefault(),
                Items = selectedCopyItems.Items,
                DataViews = selectedChapter.DataViews,
                Fees = selectedCopyItems.Fees,
                TickerMessages = selectedCopyItems.TickerMessages
            });

            await sessionState.SwitchSelectedSystem();

            if (AltChapterObject.Id == 0)
            {
                var returnObject = await chapterManagementService.Add(AltChapterObject);
                AltChapterObject.Id = returnObject.Id;

                bool gotLock = CompanyDbAccess.Lock;
                while (gotLock)
                {
                    await Task.Yield();
                    gotLock = CompanyDbAccess.Lock;
                }

                await CompanyDbAccess.SaveSmartFlowRecord(AltChapterObject, sessionState);
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

        private void HandleDeleteBackupFile()
        {
            DeleteBackupFile(SelectedFileDescription);
        }

        private void DeleteBackupFile(FileDesc file)
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
                try
                {
                    var chapterData = JsonConvert.DeserializeObject<VmChapter>(Json);
                    selectedChapter.Items = chapterData.Items;
                    selectedChapter.DataViews = chapterData.DataViews;
                    selectedChapter.Fees = chapterData.Fees;
                    selectedChapter.TickerMessages = chapterData.TickerMessages;
                    selectedChapter.P4WCaseTypeGroup = chapterData.P4WCaseTypeGroup;
                    selectedChapter.SelectedStep = chapterData.SelectedStep;
                    selectedChapter.SelectedView = chapterData.SelectedView;
                    selectedChapter.ShowPartnerNotes = chapterData.ShowPartnerNotes;
                    selectedChapter.ShowDocumentTracking = chapterData.ShowDocumentTracking;
                    SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(selectedChapter);
                    
                    await chapterManagementService.Update(SelectedChapterObject);


                    //keep track of time last updated ready for comparison by other sessions checking for updates
                    appChapterState.SetLastUpdated(sessionState, selectedChapter);

                    SelectChapter(SelectedChapterObject);

                    showJSON = false;

                    await InvokeAsync(() =>
                    {
                        StateHasChanged();
                    });
                }
                catch(Exception e)
                {
                    JSONErrors.Add("Error processing data");
                    ShowErrorModal("Backup Restore Error", "The following errors occured during the restore:", JSONErrors);
                }

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
            SaveJson(SelectedChapterObject.SmartflowData);
        }


        protected void ShowChapterImportModel()
        {
            SetSmartflowFilePath();

            while (!(ListFilesForBackups.Where(F => F.FilePath.Contains(".xlsx")).FirstOrDefault() is null))
            {
                ChapterFileUpload.DeleteFile(ListFilesForBackups.Where(F => F.FilePath.Contains(".xlsx")).FirstOrDefault().FilePath);
                GetSeletedChapterFileList();
            }


            Action WriteBackUp = WriteChapterJSONToFile;

            Action SelectedAction = RefreshJson;
            var parameters = new ModalParameters();
            parameters.Add("TaskObject", SelectedChapterObject);
            parameters.Add("ListFileDescriptions", ListFilesForBackups);
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
            WriteChapterJSONToFile();
            await ChapterFileUpload.WriteChapterDataToExcel(selectedChapter, dropDownChapterList, partnerCaseTypeGroups);

        }

        public void CancelCreateP4WStep()
        {
            selectedChapter.StepName = "";
            selectedChapter.SelectedStep = "";
            selectedChapter.StepName = $"SF {selectedChapter.Name} Smartflow";
            StateHasChanged();
        }

        

        protected void CreateP4WSmartflowStep()
        {
            IList<string> Errors = new List<string>();

            string confirmText = "";
            string confirmHeader = "";

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
            else if(dropDownChapterList
                .Where(D => D.DocumentType == 6)
                .Where(D => D.Notes.Contains("Smartflow:"))
                .Where(V => V.CaseTypeGroupRef == (string.IsNullOrEmpty(selectedChapter.P4WCaseTypeGroup)
                                                    ? -2
                                                    : selectedChapter.P4WCaseTypeGroup == "Global Documents"
                                                    ? 0
                                                    : selectedChapter.P4WCaseTypeGroup == "Entity Documents"
                                                    ? -1
                                                    : partnerCaseTypeGroups
                                                        .Where(P => P.Name == selectedChapter.P4WCaseTypeGroup)
                                                        .Select(P => P.Id)
                                                        .FirstOrDefault()))
                .OrderBy(D => D.Name)
                .Select(D => D.Name)
                .ToList()
                .Contains(selectedChapter.StepName))
            {
                confirmHeader = "Possible Conflict?";
                confirmText = $"This Step already exists in the case type group: {selectedChapter.P4WCaseTypeGroup}. Do you still wish to create?";
            }


            if (Errors.Count == 0 && confirmHeader == "")
            {
                CreateStep();
            }
            else if (Errors.Count > 0)
            {
                ShowErrorModal("Step Creation", "Step creation could not be completed:", Errors);
            }
            else
            {
                ShowModalConfirm(confirmText, confirmHeader, CreateStep);
            }

        }

        private async void CreateStep()
        {
            if (selectedChapter.P4WCaseTypeGroup == "Entity Documents")
            {
                ChapterP4WStep = new ChapterP4WStepSchema
                {
                    StepName = selectedChapter.StepName,
                    P4WCaseTypeGroup = selectedChapter.P4WCaseTypeGroup,
                    GadjITCaseTypeGroup = selectedChapter.CaseTypeGroup,
                    GadjITCaseType = selectedChapter.CaseType,
                    Smartflow = selectedChapter.Name,
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
                                    },
                    Answers = new List<ChapterP4WStepAnswer>{
                                     new ChapterP4WStepAnswer {QNo = 1, GoToData= $"2 [SQL: EXEC up_ORSF_CreateMatterTableEntries '[matters.entityref]', -1] [SQL: UPDATE Usr_ORSF_ENT_Control SET Current_SF = '{selectedChapter.Name}', Current_Case_Type_Group = '{selectedChapter.CaseTypeGroup}', Current_Case_Type = '{selectedChapter.CaseType}', Default_Step = '{selectedChapter.StepName}', Date_Schedule_For = DATEADD(d, 7, getdate()), Steps_To_Run = '', Schedule_AsName = (SELECT CASE WHEN dbo.fn_ORSF_IsAllCap (Description) = 1 THEN '{selectedChapter.StepName}' ELSE Description END + '|' + CONVERT(VARCHAR(20),ISNULL(Date_Schedule_For,DATEADD(d, 7, getdate())),103) + '|' + '[CurrentUser.Code]' FROM Cm_CaseItems WHERE ItemID = [currentstep.stepid]), Complete_AsName = ''WHERE EntityRef = '[Entity.Code]'][SQL: UPDATE Usr_ORSF_ENT_Control SET Screen_Opened_Via_Step = 'Y' WHERE EntityRef='[Entity.Code]']" }
                                    ,new ChapterP4WStepAnswer {QNo = 2, GoToData= $"3 [VIEW: '{selectedChapter.SelectedView}' UPDATE=Yes]" }
                                    ,new ChapterP4WStepAnswer {QNo = 3, GoToData= $"4 [SQL: SELECT dbo.fn_ORSF_GetStepsFromList('[~Usr_ORSF_MT_Control.Steps_To_Run]')] [SQL: UPDATE Usr_ORSF_ENT_Control SET Screen_Opened_Via_Step = null WHERE EntityRef='[Entity.Code]']" }
                                    ,new ChapterP4WStepAnswer {QNo = 4, GoToData= $"[SQL: SELECT CASE WHEN '[!Usr_ORSF_ENT_Control.Do_Not_Reschedule]' <> 'Y' THEN 5 ELSE 8 END] [SQL: UPDATE Usr_ORSF_ENT_Control SET Date_Schedule_For = isnull(Date_Schedule_For, Cast(getdate() as Date)) WHERE EntityRef = '[Entity.Code]']" }
                                    ,new ChapterP4WStepAnswer {QNo = 5, GoToData= $"8 [SQL: SELECT ScheduleCommand FROM fn_ORSF_GetScheduleItems(NULL, '[Usr_ORSF_ENT_Control.Schedule_AsName]' , '{selectedChapter.StepName}') ]" }
                                    ,new ChapterP4WStepAnswer {QNo = 6, GoToData= $"[SQL: UPDATE cm_caseitems set CompletionDate = GETDATE(), description = UPPER('[!Usr_ORSF_ENT_Control.Complete_AsName]') where itemid = [currentstep.stepid]]" }
                                    ,new ChapterP4WStepAnswer {QNo = 7, GoToData= $"8 [SQL: exec up_ORSF_DeleteDueStep '', [currentstep.stepid], '{selectedChapter.StepName}']" }
                                    ,new ChapterP4WStepAnswer {QNo = 8, GoToData= $"[SQL: SELECT CASE WHEN ISNULL('[!Usr_ORSF_ENT_Control.Complete_AsName]','') <> '' THEN 6 ELSE 9 END]" }
                                    ,new ChapterP4WStepAnswer {QNo = 9, GoToData= $"[SQL: DELETE FROM cm_caseitems where itemid = [currentstep.stepid]]" }
                                    }
                };
            }
            else
            {
                ChapterP4WStep = new ChapterP4WStepSchema
                {
                    StepName = selectedChapter.StepName,
                    P4WCaseTypeGroup = selectedChapter.P4WCaseTypeGroup,
                    GadjITCaseTypeGroup = selectedChapter.CaseTypeGroup,
                    GadjITCaseType = selectedChapter.CaseType,
                    Smartflow = selectedChapter.Name,
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
                                    },
                    Answers = new List<ChapterP4WStepAnswer>{
                                    new ChapterP4WStepAnswer {QNo = 1, GoToData= $"2 [SQL: EXEC up_ORSF_CreateMatterTableEntries '[matters.entityref]', [matters.number]] [SQL: UPDATE Usr_ORSF_MT_Control SET Current_SF = '{selectedChapter.Name}', Current_Case_Type_Group = '{selectedChapter.CaseTypeGroup}', Current_Case_Type = '{selectedChapter.CaseType}', Default_Step = '{selectedChapter.StepName}', Date_Schedule_For = DATEADD(d, 7, getdate()), Steps_To_Run = '', Schedule_AsName = '{selectedChapter.StepName}|' + (SELECT CASE WHEN dbo.fn_ORSF_IsAllCap (Description) = 1 THEN '{selectedChapter.StepName}' + '|' + CONVERT(VARCHAR(20),ISNULL(Date_Schedule_For,DATEADD(d, 7, getdate())),103) ELSE Description + '|' + CONVERT(VARCHAR(20),ISNULL(s.DiaryDate, getdate()),103) END + '|' + '[matters.feeearnerref]' FROM Cm_CaseItems i INNER JOIN Cm_Steps s on s.ItemID = i.ItemID WHERE i.ItemID = [currentstep.stepid]), Complete_AsName = '' WHERE EntityRef = '[matters.entityref]' AND MatterNo = [matters.number]] [SQL: UPDATE Usr_ORSF_MT_Control SET Screen_Opened_Via_Step = 'Y' WHERE EntityRef = '[matters.entityref]' AND matterNo =[matters.number]]" }
                                    ,new ChapterP4WStepAnswer {QNo = 2, GoToData= $"3 [VIEW: '{selectedChapter.SelectedView}' UPDATE=Yes]" }
                                    ,new ChapterP4WStepAnswer {QNo = 3, GoToData= $"4 [SQL: EXEC up_ORSF_GetStepsFromList '[matters.entityref]', [matters.number]] [SQL: UPDATE Usr_ORSF_MT_Control SET Screen_Opened_Via_Step = null WHERE EntityRef='[matters.entityref]' AND matterNo=[matters.number]]" }
                                    ,new ChapterP4WStepAnswer {QNo = 4, GoToData= $"[SQL: SELECT CASE WHEN '[!Usr_ORSF_MT_Control.Do_Not_Reschedule]' <> 'Y' THEN 5 ELSE 8 END] [SQL: UPDATE Usr_ORSF_MT_Control SET Date_Schedule_For = isnull(Date_Schedule_For, Cast(getdate() as Date)) WHERE EntityRef = '[matters.entityref]' AND MatterNo = [matters.number]]" }
                                    ,new ChapterP4WStepAnswer {QNo = 5, GoToData= $"8 [SQL: SELECT ScheduleCommand FROM fn_ORSF_GetScheduleItems(NULL, '[Usr_ORSF_MT_Control.Schedule_AsName]' , '{selectedChapter.StepName}') ]" }
                                    ,new ChapterP4WStepAnswer {QNo = 6, GoToData= $"[SQL: UPDATE cm_caseitems set CompletionDate = GETDATE(), description = UPPER('[!Usr_ORSF_MT_Control.Complete_AsName]') where itemid = [currentstep.stepid]]" }
                                    ,new ChapterP4WStepAnswer {QNo = 7, GoToData= $"8 [SQL: exec up_ORSF_DeleteDueStep '', [currentstep.stepid], '{selectedChapter.StepName}']" }
                                    ,new ChapterP4WStepAnswer {QNo = 8, GoToData= $"[SQL: SELECT CASE WHEN ISNULL('[!Usr_ORSF_MT_Control.Complete_AsName]','') <> '' THEN 6 ELSE 9 END]" }
                                    ,new ChapterP4WStepAnswer {QNo = 9, GoToData= $"[SQL: DELETE FROM cm_caseitems where itemid = [currentstep.stepid]]" }
                                    }
                };
            }




            string stepJSON = JsonConvert.SerializeObject(ChapterP4WStep);

            bool creationSuccess;



            creationSuccess = await chapterManagementService.CreateStep(new VmChapterP4WStepSchemaJSONObject { StepSchemaJSON = stepJSON });

            if (creationSuccess)
            {
                dropDownChapterList = await chapterManagementService.GetDocumentList(selectedChapter.CaseType);
                TableDates = await chapterManagementService.GetDatabaseTableDateFields();

                selectedChapter.SelectedStep = selectedChapter.StepName;

                SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(selectedChapter);

                bool gotLock = chapterManagementService.Lock;
                while (gotLock)
                {
                    await Task.Yield();
                    gotLock = chapterManagementService.Lock;
                }

                await chapterManagementService.Update(SelectedChapterObject);


                //keep track of time last updated ready for comparison by other sessions checking for updates
                appChapterState.SetLastUpdated(sessionState, selectedChapter);

                StateHasChanged();
            }
        }


        private void ShowModalConfirm(string infoText, string infoHeader, Action selectedAction)
        {
            var parameters = new ModalParameters();
            parameters.Add("InfoHeader", infoHeader);
            parameters.Add("InfoText", infoText);
            parameters.Add("ConfirmAction", selectedAction);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-confirm"
            };

            Modal.Show<ModalConfirm>(infoHeader, parameters, options);
        }

        
        private async void SaveP4WCaseTypeGroup(string caseTypeGroup)
        {
            selectedChapter.P4WCaseTypeGroup = caseTypeGroup;

            SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(selectedChapter);

            await chapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);


            //keep track of time last updated ready for comparison by other sessions checking for updates
            appChapterState.SetLastUpdated(sessionState, selectedChapter);
        }

        private async void SaveSelectedView(string view)
        {
            selectedChapter.SelectedView = view;

            SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(selectedChapter);

            await chapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);


            //keep track of time last updated ready for comparison by other sessions checking for updates
            appChapterState.SetLastUpdated(sessionState, selectedChapter);
        }
        
        private async void SaveSelectedStep(string step)
        {
            selectedChapter.SelectedStep = step;

            SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(selectedChapter);

            await chapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);


            //keep track of time last updated ready for comparison by other sessions checking for updates
            appChapterState.SetLastUpdated(sessionState, selectedChapter);
        }

        private void TickerValidation()
        {
            var currentDate = DateTime.Now.Date;

            ValidTicketMessageCount = 1;
            foreach (VmTickerMessages msg in ListVmTickerMessages)
            {

                if (DateTime.ParseExact(msg.Message.ToDate, "yyyyMMdd", CultureInfo.InvariantCulture) < DateTime.ParseExact(msg.Message.FromDate, "yyyyMMdd", CultureInfo.InvariantCulture))
                {
                    msg.msgValidation = "Invalid";
                    msg.msgTooltip = "Invalid date range.";
                }
                else if (DateTime.ParseExact(msg.Message.FromDate, "yyyyMMdd", CultureInfo.InvariantCulture) > currentDate)
                {
                    msg.msgValidation = "Future";
                    msg.msgTooltip = "Message set for future date.";
                }
                else if (DateTime.ParseExact(msg.Message.ToDate, "yyyyMMdd", CultureInfo.InvariantCulture) < currentDate)
                {
                    msg.msgValidation = "Expired";
                    msg.msgTooltip = "Message expired.";
                }
                else if (ValidTicketMessageCount > 3)
                {
                    msg.msgValidation = "Exceeded";
                    msg.msgTooltip = "The maximum number of valid ticker messages are three.";
                }
                else
                {
                    ValidTicketMessageCount += 1;
                    msg.msgValidation = "Valid";
                    msg.msgTooltip = "Message will be shown on the Smarflow screen.";
                }
            }

        }

        public async void SyncSmartFlowSystems()
        {
            var lstC = await chapterManagementService.GetAllChapters();

            await CompanyDbAccess.SyncAdminSysToClient(lstC, sessionState);

            RefreshChapters();
        }
    }
}
