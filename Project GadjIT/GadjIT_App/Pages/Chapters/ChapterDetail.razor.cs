using GadjIT_ClientContext.P4W;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GadjIT_App.Services;
using System.Web;
using GadjIT_App.Services.SessionState;
using Blazored.Modal.Services;
using Blazored.Modal;
using GadjIT_App.Pages._Shared.Modals;
using GadjIT_ClientContext.P4W.Custom;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using BlazorInputFile;
using GadjIT_App.FileManagement.FileClassObjects.FileOptions;
using GadjIT_App.FileManagement.FileClassObjects;
using Microsoft.JSInterop;
using GadjIT_App.FileManagement.FileProcessing.Interface;
using GadjIT_App.Data.Admin;
using System.Globalization;
using AutoMapper;
using System.Timers;
using GadjIT_App.Services.AppState;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using GadjIT_App.Pages.Chapters.ComponentsChapterDetail._Agenda;
using GadjIT_App.Pages.Chapters.ComponentsChapterDetail._DataView;
using GadjIT_App.Pages.Chapters.ComponentsChapterDetail._Documents;
using GadjIT_App.Pages.Chapters.ComponentsChapterDetail._Header;
using GadjIT_App.Pages.Chapters.ComponentsChapterDetail._Messages;
using GadjIT_App.Pages.Chapters.ComponentsChapterDetail._SharedItems;
using GadjIT_App.Pages.Chapters.ComponentsChapterDetail._Status;
using GadjIT_App.Pages.Chapters.FileUpload;
using GadjIT_AppContext.GadjIT_App;
using GadjIT_App.Pages.Chapters.ComponentsChapterList;
using static GadjIT_App.Shared.StaticObjects.LookUps;


namespace GadjIT_App.Pages.Chapters
{
    public partial class ChapterDetail
    {

        
        [Parameter]
        public VmUsrOrsfSmartflows _SelectedChapterObject { get; set; }

        [Parameter]
        public EventCallback _SelectHome {get; set;}


        [Inject]
        IModalService Modal { get; set; }

        [Inject]
        public IMapper Mapper { get; set; }

        [Inject]
        private IJSRuntime JSRuntime {get; set;}

        [Inject]
        public INotificationManager NotificationManager {get; set;}

        [Inject]
        private IChapterManagementService ChapterManagementService { get; set; }

        [Inject]
        private IPartnerAccessService PartnerAccessService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public IUserSessionState UserSession { get; set; }

        [Inject]
        private IChapterFileUpload ChapterFileUpload { get; set; }

        [Inject]
        private IIdentityUserAccess UserAccess { get; set; }

        [Inject]
        public IAppChapterState AppChapterState { get; set; }

        [Inject]
        private ICompanyDbAccess CompanyDbAccess { get; set; }

        [Inject]
        private ILogger<ChapterList> Logger { get; set; }

        [Inject]
        public IConfiguration Configuration { get; set;}

        

        private List<FileDesc> ListFilesForBackups { get; set; }

        private List<FileDesc> ListFilesForBgImages { get; set; }

        private FileDesc SelectedFileDescription { get; set; }

        private int ValidTicketMessageCount { get; set; } 


        private List<VmFee> LstAltSystemFeeItems { get; set; } = new List<VmFee>();

        private List<VmGenSmartflowItem> LstAgendas { get; set; } = new List<VmGenSmartflowItem>();
        private List<VmFee> LstFees { get; set; } = new List<VmFee>();
        private List<VmGenSmartflowItem> LstDocs { get; set; } = new List<VmGenSmartflowItem>();
        private List<VmGenSmartflowItem> LstStatus { get; set; } = new List<VmGenSmartflowItem>();

        private List<VmDataView> LstDataViews { get; set; } = new List<VmDataView>();
        private List<VmTickerMessage> LstTickerMessages { get; set; } = new List<VmTickerMessage>();

        public List<MpSysViews> ListP4WViews;
        public List<DmDocuments> LibraryDocumentsAndSteps;
        public List<TableDate> TableDates;
        public List<CaseTypeGroups> P4WCaseTypeGroups;

        //=======================================================
        //to be deleted
        public VmFee EditFeeObject = new VmFee { FeeObject = new Fee() };
        //=======================================================

        public string EditCaseType { get; set; } = "";
        public bool CreateNewSmartflow { get; set; }

        public float ScrollPosition { get; set; }

        





        public String UserGuideURL => Configuration["AppSettings:UserGuideURL"];

        public VmDataView EditDataViewObject = new VmDataView { DataView = new DataView() };
        public VmTickerMessage EditTickerMessageObject = new VmTickerMessage { Message = new TickerMessage() };
        
        
        public VmGenSmartflowItem EditObject = new VmGenSmartflowItem { ChapterObject = new GenSmartflowItem() };

        public LinkedItems AttachObject = new LinkedItems();

        

        


        public VmChapter SelectedChapter { get; set; } = new VmChapter { Items = new List<GenSmartflowItem>() }; //SmartflowData

        public VmUsrOrsfSmartflows AltChapterObject { get; set; } = new VmUsrOrsfSmartflows();
        public VmChapter AltChapter { get; set; } = new VmChapter();

        
        int RowChanged { get; set; } = 0; //moved partial

        private int SelectedChapterId { get; set; } = -1;

        private int? altSysSelectedChapterId { get; set; }

        public string NavDisplay = "Chapter";

        private bool SeqMoving = false;

        protected bool CompareSystems = false;

        public bool DisplaySpinner = true;

        public string AlertMsgJSOM { get; set; }

        public bool ShowJSON = false;


        public IList<string> JsonErrors { get; set; }

        public ChapterP4WStepSchema ChapterP4WStep { get; set; }


        [Inject]
        public IFileHelper FileHelper { get; set; }
        



        

        



#region Page Events
        protected override async Task OnInitializedAsync()
        {
            SelectChapter();

            NavDisplay = "Chapter";
        }

        private Timer timer;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                try
                {
                    await JSRuntime.InvokeVoidAsync("showPageAfterFirstRender");
                }
                catch (Exception e)
                {
                    await GenericErrorLog(false,e, "OnAfterRenderAsync", e.Message);  
                }
            }
            else
            {
                //TODO: reinstate after testing
                // timer = new Timer();
                // timer.Interval = 10000; //10 seconds
                // timer.Elapsed += OnTimerInterval;
                // timer.AutoReset = true;
                // // Start the timer
                // timer.Enabled = true;
            }
            base.OnAfterRender(firstRender);
        }

        private async void OnTimerInterval(object sender, ElapsedEventArgs eventArgs)
        {
            //Compare current session with Chapter State to see if any other users have updated the Chapter
            //If an update is detected (ChapterLastUpdated is greater than session's ChapterLastCompared) then 
            // reload the data and invoke StateHasChanged to refresh the page.
            bool gotLock = AppChapterState.Lock;
            while (gotLock)
            {
                await Task.Yield();
                gotLock = AppChapterState.Lock;
            }

            gotLock = UserSession.Lock;
            while (gotLock)
            {
                await Task.Yield();
                gotLock = UserSession.Lock;
            }

            try
            {
                if (!(SelectedChapter is null))
                {
                    if (!(SelectedChapter.Name == "" | SelectedChapter.Name is null))
                    {
                        var ChapterLastUpdated = AppChapterState.GetLastUpdatedDate(UserSession, SelectedChapter);

                        if (UserSession.ChapterLastCompared != ChapterLastUpdated)
                        {
                            gotLock = CompanyDbAccess.Lock;
                            while (gotLock)
                            {
                                await Task.Yield();
                                gotLock = CompanyDbAccess.Lock;
                            }

                            SmartflowRecords currentChapter = await CompanyDbAccess.GetSmartflow(UserSession
                                                                                                        , _SelectedChapterObject.SmartflowObject.CaseTypeGroup
                                                                                                        , _SelectedChapterObject.SmartflowObject.CaseType
                                                                                                        , _SelectedChapterObject.SmartflowObject.SmartflowName
                                                                                                        );

                            if (currentChapter != null && currentChapter.SmartflowData != null)
                            {
                                SelectedChapter = JsonConvert.DeserializeObject<VmChapter>(currentChapter.SmartflowData);

                                await RefreshChapterItems("All");
                                await InvokeAsync(() =>
                                {
                                    StateHasChanged();
                                });

                            }
                            
                            UserSession.ChapterLastCompared = ChapterLastUpdated;
                            
                        }
                    }
                }


                gotLock = AppChapterState.Lock;
                while (gotLock)
                {
                    await Task.Yield();
                    gotLock = AppChapterState.Lock;
                }

                AppChapterState.SetUsersCurrentChapter(UserSession, SelectedChapter);
            }
            catch(HttpRequestException)
            {
                //do nothing, already dealt with
            }
            catch (Exception e)
            {
                await GenericErrorLog(true,e, "OnTimerInterval", e.Message);
            }


        }

        public void Dispose()
        {
            // During prerender, this component is rendered without calling OnAfterRender and then immediately disposed
            // this mean timer will be null so we have to check for null or use the Null-conditional operator ? 
            timer?.Dispose();
            AppChapterState.DisposeUser(UserSession);
        }
#endregion

#region Navigation and Drag Drop


        public async void SelectHome()
        {
            await _SelectHome.InvokeAsync();
        }


        




        

#endregion

#region Chapter Details

        private async void SelectChapter()
        {
            try
            {
                
                DisplaySpinner = true;

                //Single Smartflow selected, display library details
                P4WCaseTypeGroups = await PartnerAccessService.GetPartnerCaseTypeGroups();
                

                if (!(_SelectedChapterObject.SmartflowObject.SmartflowData is null))
                {
                    SelectedChapter = JsonConvert.DeserializeObject<VmChapter>(_SelectedChapterObject.SmartflowObject.SmartflowData);

                }
                else
                {

                    //Initialise the VmChapter in case of null Json
                    SelectedChapter = new VmChapter
                    {
                        Items = new List<GenSmartflowItem>()
                                                     ,
                        DataViews = new List<DataView>()
                                                     ,
                        TickerMessages = new List<TickerMessage>()
                                                     ,
                        Fees = new List<Fee>()
                    };
                    SelectedChapter.CaseTypeGroup = _SelectedChapterObject.SmartflowObject.CaseTypeGroup;
                    SelectedChapter.CaseType = _SelectedChapterObject.SmartflowObject.CaseType;
                    SelectedChapter.Name = _SelectedChapterObject.SmartflowObject.SmartflowName;

                }

                SelectedChapter.StepName = $"SF {_SelectedChapterObject.SmartflowObject.SmartflowName} Smartflow";
                SelectedChapter.SelectedStep = SelectedChapter.SelectedStep is null || SelectedChapter.SelectedStep == "Create New" ? "" : SelectedChapter.SelectedStep;
                SelectedChapter.Fees = SelectedChapter.Fees is null ? new List<Fee>() : SelectedChapter.Fees;
                SelectedChapter.Items = SelectedChapter.Items is null ? new List<GenSmartflowItem>() : SelectedChapter.Items;
                SelectedChapter.TickerMessages = SelectedChapter.TickerMessages is null ? new List<TickerMessage>() : SelectedChapter.TickerMessages;
                SelectedChapter.DataViews = SelectedChapter.DataViews is null ? new List<DataView>() : SelectedChapter.DataViews;
                SelectedChapterId = _SelectedChapterObject.SmartflowObject.Id;
                CompareSystems = false;
                RowChanged = 0;
                NavDisplay = "Chapter";
                ShowJSON = false;


                try
                {
                    LibraryDocumentsAndSteps = await ChapterManagementService.GetDocumentList(SelectedChapter.CaseType);
                    //LibraryDocumentsAndSteps = LibraryDocumentsAndSteps.Where(D => !(D.Name is null)).ToList();
                    TableDates = await ChapterManagementService.GetDatabaseTableDateFields();
                    ListP4WViews = await PartnerAccessService.GetPartnerViews();
                }
                catch (Exception e)
                {
                    await NotificationManager.ShowNotification("Danger", $"Oops! Something went wrong accessing the Smartflow details");

                    await GenericErrorLog(false,e, "SelectChapter", $"Getting document list after Smartflow selected for edit: {e.Message}");

                    DisplaySpinner = false;

                    //return user to the full list of Smartflows. Do not present the user with the details of the Smartflow as none of the features will work.
                    SelectHome();

                    return;
                }


                await RefreshChapterItems("All");


                if (!string.IsNullOrEmpty(SelectedChapter.BackgroundImage))
                {
                    UserSession.SetTempBackground(SelectedChapter.BackgroundImage.Replace("/wwwroot", ""), NavigationManager.Uri);
                    UserSession.RefreshHome?.Invoke();
                }

                SetSmartflowFilePath();
                GetSeletedChapterFileList();


                //set the session ChapterLastCompared to be the same as the Chapter State value to prevent an immediate 
                // refresh of the page (by OnTimerInterval)
                if (!(SelectedChapter.Name == "" | SelectedChapter.Name is null))
                {

                    bool gotLock = AppChapterState.Lock;
                    while (gotLock)
                    {
                        await Task.Yield();
                        gotLock = AppChapterState.Lock;
                    }

                    UserSession.ChapterLastCompared = AppChapterState.GetLastUpdatedDate(UserSession, SelectedChapter);
                }

                await JSRuntime.InvokeVoidAsync("moveToPosition", 0);

            }
            catch (Exception ex)
            {

                await GenericErrorLog(true,ex, "SelectChapter", $"Loading selected Smartflow: {ex.Message}");

            }
            finally
            {
                DisplaySpinner = false;

                StateHasChanged();


            }


        }


        public async void RefreshSelectedList()
        {
            await RefreshChapterItems("All");
            

            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }

        private async Task RefreshChapterItems(string listType)
        {
            try
            {
            
                var lstChapterItems = SelectedChapter.Items
                                        .Select(L => new VmGenSmartflowItem { ChapterObject = L })
                                        .ToList();



                /*
                    * listType = All when chapter is selected, 
                    * listType = nav selected e.g. Agenda when and object in a specific list has been altered
                    * 
                    */
                if (listType == "Agenda" | listType == "All")
                {
                    LstAgendas = lstChapterItems
                                        .OrderBy(A => A.ChapterObject.SeqNo)
                                        .Where(A => A.ChapterObject.Type == "Agenda")
                                        .ToList();

                }
                if (listType == "Docs" | listType == "All")
                {

                    Dictionary<int?, string> docTypes = new Dictionary<int?, string> { { 1, "Doc" }, { 4, "Form" }, { 6, "Step" }, { 8, "Date" }, { 9, "Email" }, { 11, "Doc" }, { 12, "Email" }, { 13, "Csv" } };
                    LstDocs = lstChapterItems
                                        .OrderBy(A => A.ChapterObject.SeqNo)
                                        .Where(A => A.ChapterObject.Type == "Doc")
                                        .Select(A => {
                                        //Make sure all Items have the DocType set by comparing against dm_documents for matches
                                        A.DocType = LibraryDocumentsAndSteps.Where(D => D.Name.ToUpper() == A.ChapterObject.Name.ToUpper())
                                                                                    .Select(D => docTypes.ContainsKey(D.DocumentType) ? docTypes[D.DocumentType] : "Doc")
                                                                                    .FirstOrDefault();
                                            A.ChapterObject.RescheduleDays = !string.IsNullOrEmpty(A.ChapterObject.AsName) && A.ChapterObject.RescheduleDays is null ? 0 : A.ChapterObject.RescheduleDays;
                                            A.ChapterObject.Action = (A.ChapterObject.Action == "" ? "INSERT" : A.ChapterObject.Action);
                                        //Make sure all Linked Items have the DocType set by comparing against dm_documents for matches
                                        A.ChapterObject.LinkedItems = A.ChapterObject.LinkedItems == null
                                                                                    ? null
                                                                                    : A.ChapterObject.LinkedItems
                                                                                                    .Select(L => {
                                                                                                        L.DocType = LibraryDocumentsAndSteps
                                                                                                                .Where(D => D.Name.ToUpper() == L.DocName.ToUpper())
                                                                                                                .Select(D => docTypes.ContainsKey(D.DocumentType) ? docTypes[D.DocumentType] : "Doc")
                                                                                                                .FirstOrDefault();
                                                                                                        return L;
                                                                                                    }
                                                                                        ).ToList();
                                            return A;
                                        })
                                        .ToList();



                }
                if (listType == "Fees" | listType == "All")
                {
                    LstFees = SelectedChapter.Fees.Select(F => new VmFee { FeeObject = F })
                                                            .OrderBy(F => F.FeeObject.SeqNo)
                                                            .ToList();


                }
                if (listType == "Status" | listType == "All")
                {
                    LstStatus = lstChapterItems
                                        .OrderBy(A => A.ChapterObject.SeqNo)
                                        .Where(A => A.ChapterObject.Type == "Status")
                                        .ToList();
                }
                if (listType == "DataViews" | listType == "All")
                {
                    LstDataViews = (SelectedChapter.DataViews is null)
                                                    ? new List<VmDataView>()
                                                    : SelectedChapter
                                                            .DataViews
                                                            .Select(D => new VmDataView { DataView = D })
                                                            .OrderBy(D => D.DataView.SeqNo)
                                                            .ToList();
                }
                if (listType == "TickerMessages" | listType == "All")
                {
                    LstTickerMessages = (SelectedChapter.TickerMessages is null)
                                                    ? new List<VmTickerMessage>()
                                                    : SelectedChapter
                                                            .TickerMessages
                                                            .Select(D => new VmTickerMessage { Message = D })
                                                            .OrderBy(D => D.Message.SeqNo)
                                                            .ToList();
                    TickerValidation();

                }
            
                await InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            }
            catch (Exception e)
            {
                await GenericErrorLog(true,e, "RefreshChapterItems", $"Refreshing Smartflow tab items: {e.Message}");

            }
            
        }

        private async void SaveAttachmentTracking()
        {
            try
            {
                _SelectedChapterObject.SmartflowObject.SmartflowData = JsonConvert.SerializeObject(SelectedChapter);

                await ChapterManagementService.Update(_SelectedChapterObject.SmartflowObject).ConfigureAwait(false);

                //keep track of time last updated ready for comparison by other sessions checking for updates
                AppChapterState.SetLastUpdated(UserSession, SelectedChapter);
            }
            catch (Exception e)
            {

                await GenericErrorLog(true,e, "SaveAttachmentTracking", $"Toggling document tracking: {e.Message}");

            }
        }

        

        

        private async void SaveShowPartnerNotes()
        {
            try
            {
                _SelectedChapterObject.SmartflowObject.SmartflowData = JsonConvert.SerializeObject(SelectedChapter);

                await ChapterManagementService.Update(_SelectedChapterObject.SmartflowObject).ConfigureAwait(false);

                //keep track of time last updated ready for comparison by other sessions checking for updates
                AppChapterState.SetLastUpdated(UserSession, SelectedChapter);
            }
            catch (Exception e)
            {
                await GenericErrorLog(true,e, "SaveShowPartnerNotes", $"Saving notes: {e.Message}");
            }
        }

        
        protected void ShowNav(string displayChange)
        {
            CompareSystems = false;
            RowChanged = 0;
            NavDisplay = displayChange;
        }


 

        protected async Task PrepareBackUpForDelete(FileDesc selectedFile)
        {
            try
            {
                await SetSmartflowFilePath();

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
            catch(Exception e)
            {
                await GenericErrorLog(false,e, "PrepareBackUpForDelete", e.Message);
            }
        }


        

        
        
        
        private void TickerValidation()
        {
            var currentDate = DateTime.Now.Date;

            ValidTicketMessageCount = 1;
            foreach (VmTickerMessage msg in LstTickerMessages)
            {

                if (DateTime.ParseExact(msg.Message.ToDate, "yyyyMMdd", CultureInfo.InvariantCulture) < DateTime.ParseExact(msg.Message.FromDate, "yyyyMMdd", CultureInfo.InvariantCulture))
                {
                    msg.MsgValidation = "Invalid";
                    msg.MsgTooltip = "Invalid date range.";
                }
                else if (DateTime.ParseExact(msg.Message.FromDate, "yyyyMMdd", CultureInfo.InvariantCulture) > currentDate)
                {
                    msg.MsgValidation = "Future";
                    msg.MsgTooltip = "Message set for future date.";
                }
                else if (DateTime.ParseExact(msg.Message.ToDate, "yyyyMMdd", CultureInfo.InvariantCulture) < currentDate)
                {
                    msg.MsgValidation = "Expired";
                    msg.MsgTooltip = "Message expired.";
                }
                else if (ValidTicketMessageCount > 3)
                {
                    msg.MsgValidation = "Exceeded";
                    msg.MsgTooltip = "The maximum number of valid ticker messages are three.";
                }
                else
                {
                    ValidTicketMessageCount += 1;
                    msg.MsgValidation = "Valid";
                    msg.MsgTooltip = "Message will be shown on the Smarflow screen.";
                }
            }

        }


        





#endregion

#region Images

private async void HandleBgImageFileSelection(IFileListEntry[] entryFiles)
        {
            try
            {
                FileHelper.CustomPath = $"wwwroot/images/Companies/{UserSession.Company.CompanyName}/BackgroundImages";
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
            catch(Exception e)
            {
                GenericErrorLog(false,e, "HandleBgImageFileSelection", e.Message);
            }
            
        }

        protected async Task PrepareBgImageForDelete(FileDesc selectedFile)
        {
            try
            {
                FileHelper.CustomPath = $"wwwroot/images/Companies/{UserSession.Company.CompanyName}/BackgroundImages";

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
            catch(Exception e)
            {
                GenericErrorLog(false,e, "PrepareBgImageForDelete", e.Message);
            }
        }

        private void HandleDeleteBgImageFile()
        {
            DeleteBgImageFile(SelectedFileDescription);
        }

        private async void SelectBgImage(FileDesc fileDesc)
        {
            try
            {
                SelectedChapter.BackgroundImage = fileDesc.FileURL.Replace("/wwwroot", "");
                SelectedChapter.BackgroundImageName = fileDesc.FileName;

                _SelectedChapterObject.SmartflowObject.SmartflowData = JsonConvert.SerializeObject(SelectedChapter);

                await ChapterManagementService.Update(_SelectedChapterObject.SmartflowObject).ConfigureAwait(false);


                //keep track of time last updated ready for comparison by other sessions checking for updates
                AppChapterState.SetLastUpdated(UserSession, SelectedChapter);

                UserSession.SetTempBackground(SelectedChapter.BackgroundImage.Replace("/wwwroot", ""), NavigationManager.Uri);
                UserSession.RefreshHome?.Invoke();

                await InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            }
            catch(HttpRequestException)
            {
                //do nothing, already dealt with
            }
            catch(Exception e)
            {
                GenericErrorLog(false,e, "SelectBgImage", e.Message);
            }
        }

        private async Task DeleteBgImageFile(FileDesc file)
        {
            try
            {
                FileHelper.CustomPath = $"wwwroot/images/Companies/{UserSession.Company.CompanyName}/BackgroundImages";

                ChapterFileUpload.DeleteFile(file.FilePath);

                ListFilesForBgImages = FileHelper.GetFileList();
                StateHasChanged();
            }
            catch(Exception e)
            {
                GenericErrorLog(false,e, "DeleteBgImageFile", e.Message);
            }
        }

        

        public async void SaveSelectedBackgroundColour (string colour)
        {
            try
            {
                SelectedChapter.BackgroundColour = ListChapterColours.Where(C => C.ColourName == colour).Select(C => C.ColourCode).FirstOrDefault();
                SelectedChapter.BackgroundColourName = colour;

                _SelectedChapterObject.SmartflowObject.SmartflowData = JsonConvert.SerializeObject(SelectedChapter);

                await ChapterManagementService.Update(_SelectedChapterObject.SmartflowObject).ConfigureAwait(false);


                //keep track of time last updated ready for comparison by other sessions checking for updates
                AppChapterState.SetLastUpdated(UserSession, SelectedChapter);
            }
            catch (Exception e)
            {
                GenericErrorLog(true,e, "SaveSelectedBackgroundColour", $"Saving background colour: {e.Message}");
            }
        }

         

        public void UpdatePreviewImage(bool preview)
        {
            PreviewChapterImage = preview;
        }

        private bool PreviewChapterImage
        {
            get 
            {
                return UserSession.User.DisplaySmartflowPreviewImage; 
            }
            set
            {
                if (value)
                {
                    if(!string.IsNullOrEmpty(SelectedChapter.BackgroundImageName))
                    {
                        UserSession.SetTempBackground(SelectedChapter.BackgroundImage.Replace("/wwwroot",""), NavigationManager.Uri);
                    }
                    else
                    {
                        UserSession.SetTempBackground("", NavigationManager.Uri);
                    }
                }
                else
                {
                    UserSession.TempBackGroundImage = "";
                }

                UserSession.User.DisplaySmartflowPreviewImage = value;
                UserSession.RefreshHome?.Invoke();
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

            try
            {
                await UserAccess.UpdateUserDetails(UserSession.User).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                GenericErrorLog(true,e, "SavePreviewChapterImage", e.Message);
            }
        }

        public string SelectColour { 
            get 
            { 
                return SelectedChapter.BackgroundColourName; 
            }
            set
            {
                SaveSelectedBackgroundColour(value);
            }
        }

        
        public async void WriteChapterJSONToFile()
        {
            try
            {
                await SetSmartflowFilePath();

                var fileName = SelectedChapter.Name + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt";

                ChapterFileUpload.WriteChapterToFile(_SelectedChapterObject.SmartflowObject.SmartflowData, fileName);

                GetSeletedChapterFileList();
                StateHasChanged();
            }
            catch(Exception e)
            {
                await GenericErrorLog(false,e, "WriteChapterJSONToFile", e.Message);
            }
            
        }

        /// <summary>
        /// TODO: Need to work out what this method does
        /// </summary>
        private async Task SetSmartflowFilePath()
        {
            try
            {
                ChapterFileOptions chapterFileOption;

                chapterFileOption = new ChapterFileOptions
                {
                    Company = UserSession.Company.CompanyName,
                    CaseTypeGroup = SelectedChapter.CaseTypeGroup,
                    CaseType = SelectedChapter.CaseType,
                    Chapter = SelectedChapter.Name
                };

                ChapterFileUpload.SetChapterOptions(chapterFileOption);
            }
            catch (Exception ex)
            {
                await GenericErrorLog(true,ex, "SetSmartflowFilePath", $"Setting Smartflow file path: {ex.Message}");
            }
        }

        

#endregion

#region Modals


        

        

#endregion

#region Chapter Detail Comparrisons

    
        protected void PrepareChapterSync()
        {
            string infoText;

            switch (NavDisplay)
            {
                case "Chapter":
                    infoText = $"Make the {(UserSession.SelectedSystem == "Live" ? "Dev" : "Live")} system the same as {UserSession.SelectedSystem} for all chapter items.";
                    break;
                default:
                    infoText = $"Make the {(UserSession.SelectedSystem == "Live" ? "Dev" : "Live")} system the same as {UserSession.SelectedSystem} for all {NavDisplay}.";
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


        private void HandleChapterSync()
        {
            if (NavDisplay.ToLower() == "chapter")
            {
                SyncAll();
            }
            else
            {
                SyncToAltSystem(NavDisplay);
            }

        }

        private async void SyncAll()
        {
            //CompareSystems = true;
            //await CompareSelectedChapterToAltSystem();
            //CompareSystems = false;
            SyncToAltSystem("All");
        }

        private async void SyncToAltSystem(string option)
        {
            try
            {
                var selectedCopyItems = new VmChapter { Items = new List<GenSmartflowItem>()
                                                        , Fees = new List<Fee>()
                                                        , DataViews = new List<DataView>()
                                                        , TickerMessages = new List<TickerMessage>()};

                if (!(AltChapterObject is null))
                {
                    if (!string.IsNullOrEmpty(AltChapterObject.SmartflowObject.SmartflowData))
                    {
                        selectedCopyItems = JsonConvert.DeserializeObject<VmChapter>(AltChapterObject.SmartflowObject.SmartflowData);
                    }
                }
                else
                {
                    AltChapterObject.SmartflowObject = new UsrOrsfSmartflows 
                                        { 
                                            CaseType = _SelectedChapterObject.SmartflowObject.CaseType
                                            ,CaseTypeGroup = _SelectedChapterObject.SmartflowObject.CaseTypeGroup
                                            ,SeqNo = _SelectedChapterObject.SmartflowObject.SeqNo
                                            ,SmartflowName = _SelectedChapterObject.SmartflowObject.SmartflowName
                                            ,VariantName = _SelectedChapterObject.SmartflowObject.VariantName
                                            ,VariantNo = _SelectedChapterObject.SmartflowObject.VariantNo
                                        };
                }


                if (option == "Agenda" | option == "All")
                {
                    foreach (var item in selectedCopyItems.Items.Where(C => C.Type == "Agenda").ToList())
                    {
                        selectedCopyItems.Items.Remove(item);
                    }

                    selectedCopyItems.Items.AddRange(SelectedChapter.Items.Where(C => C.Type == "Agenda").ToList());
                }



                if (option == "Status" | option == "All")
                {
                    foreach (var item in selectedCopyItems.Items.Where(C => C.Type == "Status").ToList())
                    {
                        selectedCopyItems.Items.Remove(item);
                    }

                    selectedCopyItems.Items.AddRange(SelectedChapter.Items.Where(C => C.Type == "Status").ToList());
                }



                if (option == "Docs" | option == "All")
                {
                    foreach (var item in selectedCopyItems.Items.Where(C => C.Type == "Doc").ToList())
                    {
                        selectedCopyItems.Items.Remove(item);
                    }

                    selectedCopyItems.Items.AddRange(SelectedChapter.Items.Where(C => C.Type == "Doc").ToList());
                }


                if (option == "Fees" | option == "All")
                {
                    foreach (var item in selectedCopyItems.Fees.ToList())
                    {
                        selectedCopyItems.Fees.Remove(item);
                    }

                    selectedCopyItems.Fees.AddRange(SelectedChapter.Fees.ToList());
                }


                if (option == "DataViews" | option == "All")
                {
                    foreach (var item in selectedCopyItems.DataViews.ToList())
                    {
                        selectedCopyItems.DataViews.Remove(item);
                    }

                    selectedCopyItems.DataViews.AddRange(SelectedChapter.DataViews.ToList());
                }

                if (option == "TickerMessages" | option == "All")
                {
                    foreach (var item in selectedCopyItems.TickerMessages.ToList())
                    {
                        selectedCopyItems.TickerMessages.Remove(item);
                    }

                    selectedCopyItems.TickerMessages.AddRange(SelectedChapter.TickerMessages.ToList());
                }

                AltChapterObject.SmartflowObject.SmartflowData = JsonConvert.SerializeObject(new VmChapter
                {
                    CaseTypeGroup = _SelectedChapterObject.SmartflowObject.CaseTypeGroup,
                    CaseType = _SelectedChapterObject.SmartflowObject.CaseType,
                    Name = _SelectedChapterObject.SmartflowObject.SmartflowName,
                    SeqNo = _SelectedChapterObject.SmartflowObject.SeqNo.GetValueOrDefault(),
                    Items = selectedCopyItems.Items,
                    DataViews = SelectedChapter.DataViews,
                    Fees = selectedCopyItems.Fees,
                    TickerMessages = selectedCopyItems.TickerMessages
                });

                await UserSession.SwitchSelectedSystem();

                if (AltChapterObject.SmartflowObject.Id == 0)
                {
                    var returnObject = await ChapterManagementService.Add(AltChapterObject.SmartflowObject);
                    AltChapterObject.SmartflowObject.Id = returnObject.Id;

                    bool gotLock = CompanyDbAccess.Lock;
                    while (gotLock)
                    {
                        await Task.Yield();
                        gotLock = CompanyDbAccess.Lock;
                    }

                    await CompanyDbAccess.SaveSmartFlowRecord(AltChapterObject.SmartflowObject, UserSession);
                }
                else
                {
                    await ChapterManagementService.Update(AltChapterObject.SmartflowObject);
                }

                await UserSession.ResetSelectedSystem();

                //await CompareSelectedChapterToAltSystem();

                StateHasChanged();
            }
            catch(HttpRequestException)
            {
                //do nothing, already dealt with
            }
            catch(Exception e)
            {
                await GenericErrorLog(false,e, "SyncToAltSystem", e.Message).ConfigureAwait(false);
            }
        }

        

        protected void ShowChapterComparisonModal(VmGenSmartflowItem _editObject) //called as EventCallback from ChapterDetailDocument
        {
            EditObject = _editObject;

            //Action Compare = CompareChapterItemsToAltSytemAction;

            var parameters = new ModalParameters();
            parameters.Add("Object", EditObject);
            //parameters.Add("ComparisonRefresh", Compare);
            parameters.Add("UserSession", UserSession);
            parameters.Add("CurrentSysParentId", SelectedChapterId);
            parameters.Add("AlternateSysParentId", altSysSelectedChapterId);
            parameters.Add("CurrentChapter", SelectedChapter);
            parameters.Add("AltChapter", AltChapter);
            parameters.Add("CurrentChapterRow", _SelectedChapterObject);
            parameters.Add("AltChapterRow", AltChapterObject);
            parameters.Add("CompanyDbAccess", CompanyDbAccess);
            parameters.Add("CreateNewSmartflow", CreateNewSmartflow);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-chapter-comparison"
            };

            Modal.Show<ChapterDocumentComparison>("Synchronise Smartflow Item", parameters, options);

        }




        
        

        protected async Task ShowChapterDeleteAltObject(VmGenSmartflowItem selectedItem)
        {
            try
            {
                EditObject = selectedItem;

                Action SelectedDeleteAction = HandleAltDetailDelete;
                var parameters = new ModalParameters();
                parameters.Add("_ItemName", EditObject.ChapterObject.Name);
                parameters.Add("_DeleteAction", SelectedDeleteAction);
                parameters.Add("_InfoText", $"Are you sure you wish to delete the '{EditObject.ChapterObject.Name}' {EditObject.ChapterObject.Type}?");

                var options = new ModalOptions()
                {
                    Class = "blazored-custom-modal"
                };

                Modal.Show<ModalChapterDetailDelete>($"Delete {EditObject.ChapterObject.Type}", parameters, options);
            }
            catch(Exception e)
            {
                await GenericErrorLog(false,e, "ShowDeleteAltObject", e.Message);
            }

        }

        protected async Task ShowChapterDeleteAltDataView(VmDataView selectedItem)//change to ShowDeleteAltDataView
        {
            try
            {
                EditDataViewObject = selectedItem;

                Action SelectedDeleteAction = HandleAltDataViewDelete;
                var parameters = new ModalParameters();
                parameters.Add("ItemName", EditObject.ChapterObject.Name);
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
            catch(Exception e)
            {
                await GenericErrorLog(false,e, "PrepareDeleteAltDataView", e.Message);
            }
        }

        

        protected async Task ShowChapterDeleteAltMessage(VmTickerMessage selectedItem)
        {
            try
            {
                EditTickerMessageObject = selectedItem;

                Action SelectedDeleteAction = HandleAltMessageDelete;
                var parameters = new ModalParameters();
                parameters.Add("ItemName", EditObject.ChapterObject.Name);
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
            catch(Exception e)
            {
                await GenericErrorLog(false,e, "PrepareDeleteAltMessage", e.Message);
            }
        }

        private async void HandleAltDetailDelete()
        {
            await UserSession.SwitchSelectedSystem();
            AltChapter.Items.Remove(EditObject.ChapterObject);
            AltChapterObject.SmartflowObject.SmartflowData = JsonConvert.SerializeObject(AltChapter);
            await ChapterManagementService.Update(AltChapterObject.SmartflowObject);

            await UserSession.ResetSelectedSystem();

            //await CompareSelectedChapterToAltSystem();
        }
        

        

        private async void HandleAltDataViewDelete()
        {
            await UserSession.SwitchSelectedSystem();
            AltChapter.DataViews.Remove(EditDataViewObject.DataView);
            AltChapterObject.SmartflowObject.SmartflowData = JsonConvert.SerializeObject(AltChapter);
            await ChapterManagementService.Update(AltChapterObject.SmartflowObject);

            await UserSession.ResetSelectedSystem();

            //await CompareSelectedChapterToAltSystem();
        }

        private async void HandleAltMessageDelete()
        {
            await UserSession.SwitchSelectedSystem();
            AltChapter.TickerMessages.Remove(EditTickerMessageObject.Message);
            AltChapterObject.SmartflowObject.SmartflowData = JsonConvert.SerializeObject(AltChapter);
            await ChapterManagementService.Update(AltChapterObject.SmartflowObject);

            await UserSession.ResetSelectedSystem();

            //await CompareSelectedChapterToAltSystem();
        }



        protected void ShowChapterMessageComparisonModal(VmTickerMessage selectedItem)
        {
            EditTickerMessageObject = selectedItem;

            //Action Compare = CompareChapterItemsToAltSytemAction;

            var parameters = new ModalParameters();
            parameters.Add("_Object", EditTickerMessageObject);
            //parameters.Add("_ComparisonRefresh", Compare);
            parameters.Add("_CurrentSysParentId", SelectedChapterId);
            parameters.Add("_AlternateSysParentId", altSysSelectedChapterId);
            parameters.Add("_CurrentChapter", SelectedChapter);
            parameters.Add("_AltChapter", AltChapter);
            parameters.Add("_CurrentChapterRow", _SelectedChapterObject);
            parameters.Add("_AltChapterRow", AltChapterObject);
            parameters.Add("_CreateNewSmartflow",CreateNewSmartflow);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-chapter-comparison"
            };

            Modal.Show<ChapterMessageComparison>("Synchronise Smartflow Item", parameters, options);
        }


        protected void ShowChapterDataViewComparisonModal(VmDataView _editObject)
        {
            EditDataViewObject = _editObject;

            //Action Compare = CompareChapterItemsToAltSytemAction;

            var parameters = new ModalParameters();
            parameters.Add("Object", EditDataViewObject);
            //parameters.Add("ComparisonRefresh", Compare);
            parameters.Add("UserSession", UserSession);
            parameters.Add("CurrentSysParentId", SelectedChapterId);
            parameters.Add("AlternateSysParentId", altSysSelectedChapterId);
            parameters.Add("CurrentChapter", SelectedChapter);
            parameters.Add("AltChapter", AltChapter);
            parameters.Add("CurrentChapterRow", _SelectedChapterObject);
            parameters.Add("AltChapterRow", AltChapterObject);
            parameters.Add("CompanyDbAccess", CompanyDbAccess);
            parameters.Add("CreateNewSmartflow", CreateNewSmartflow);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-chapter-comparison"
            };

            Modal.Show<ChapterDataViewComparison>("Synchronise Smartflow Item", parameters, options);
        }



#endregion

#region File Handling

        

        private async void HandleBackupFileSelection(IFileListEntry[] entryFiles)
        {
            try
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
            catch(Exception e)
            {
                await GenericErrorLog(false,e, "HandleBackupFileSelection", e.Message);
            }
        }

        private void GetSeletedChapterFileList()
        {
            ListFilesForBackups = ChapterFileUpload.GetFileListForChapter();
        }

        private void ReadBackUpFile(string filePath)
        {
            var readJSON = ChapterFileUpload.readJson(filePath);
            SaveJson(readJSON);
        }

        private async void DownloadFile(FileDesc file)
        {
            var data = ChapterFileUpload.ReadFileToByteArray(file.FilePath);

            await JSRuntime.InvokeAsync<object>(
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
            JsonErrors = new List<string>();
            JsonErrors = ChapterFileUpload.ValidateChapterJSON(Json);

            if (JsonErrors.Count == 0)
            {
                try
                {
                    var chapterData = JsonConvert.DeserializeObject<VmChapter>(Json);
                    SelectedChapter.Items = chapterData.Items;
                    SelectedChapter.DataViews = chapterData.DataViews;
                    SelectedChapter.Fees = chapterData.Fees;
                    SelectedChapter.TickerMessages = chapterData.TickerMessages;
                    SelectedChapter.P4WCaseTypeGroup = chapterData.P4WCaseTypeGroup;
                    SelectedChapter.SelectedStep = chapterData.SelectedStep;
                    SelectedChapter.SelectedView = chapterData.SelectedView;
                    SelectedChapter.ShowPartnerNotes = chapterData.ShowPartnerNotes;
                    SelectedChapter.ShowDocumentTracking = chapterData.ShowDocumentTracking;

                    _SelectedChapterObject.SmartflowObject.SmartflowData = JsonConvert.SerializeObject(SelectedChapter);
                    
                    await ChapterManagementService.Update(_SelectedChapterObject.SmartflowObject);


                    //keep track of time last updated ready for comparison by other sessions checking for updates
                    AppChapterState.SetLastUpdated(UserSession, SelectedChapter);

                    SelectChapter();

                    ShowJSON = false;

                    await InvokeAsync(() =>
                    {
                        StateHasChanged();
                    });
                }
                catch(Exception e)
                {
                    JsonErrors.Add("Error processing data");
                    ShowErrorModal("Backup Restore Error", "The following errors occured during the restore:", JsonErrors);
                }

            }
            else
            {
                ShowErrorModal("Backup Restore Error", "The following errors occured during the restore:", JsonErrors);
            }
        }

        

        

        

#endregion 

#region Error Handling


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

        /****************************************/
        /* ERROR HANDLING */
        /****************************************/
        private async Task GenericErrorLog(bool showNotificationMsg, Exception e, string _method, string _message)
        {
            using (LogContext.PushProperty("SourceSystem", UserSession.SelectedSystem))
            using (LogContext.PushProperty("SourceCompanyId", UserSession.Company.Id))
            using (LogContext.PushProperty("SourceUserId", UserSession.User.Id))
            using (LogContext.PushProperty("SourceContext", nameof(ChapterDetail)))
            {
                Logger.LogError(e,"Error - Method: {0}, Message: {1}",_method, _message);
            }

            if(showNotificationMsg)
            {
                await NotificationManager.ShowNotification("Danger", $"Oops! Something went wrong.");
            }
        }


#endregion

    }
}
