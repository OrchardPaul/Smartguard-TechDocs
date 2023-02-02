using GadjIT_ClientContext.P4W;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GadjIT_App.Services;
using GadjIT_App.Services.SessionState;
using Blazored.Modal.Services;
using Blazored.Modal;
using GadjIT_App.Pages._Shared.Modals;
using GadjIT_ClientContext.P4W.Custom;
using Newtonsoft.Json;
using Microsoft.JSInterop;
using GadjIT_App.Data.Admin;
using System.Globalization;
using System.Timers;
using GadjIT_App.Services.AppState;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using GadjIT_AppContext.GadjIT_App;


namespace GadjIT_App.Pages.Chapters
{
    public partial class ChapterDetail
    {

        
        [Parameter]
        public VmUsrOrsfSmartflows _SelectedChapterObject { get; set; }

        [Parameter]
        public EventCallback _SelectHome {get; set;}

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
        private IIdentityUserAccess UserAccess { get; set; }

        [Inject]
        public IAppChapterState AppChapterState { get; set; }

        [Inject]
        private ICompanyDbAccess CompanyDbAccess { get; set; }

        [Inject]
        private ILogger<ChapterList> Logger { get; set; }

        [Inject]
        public IConfiguration Configuration { get; set;}

        

        private int ValidTicketMessageCount { get; set; } 


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

        public String UserGuideURL => Configuration["AppSettings:UserGuideURL"];
        
        public LinkedItem AttachObject = new LinkedItem();


        public VmSmartflow SelectedChapter { get; set; } = new VmSmartflow { Items = new List<GenSmartflowItem>() }; //SmartflowData

        int RowChanged { get; set; } = 0; //moved partial

        private int SelectedChapterId { get; set; } = -1;

        public string NavDisplay = "Chapter";

        private bool SeqMoving = false;

        protected bool CompareSystems = false;

        public bool DisplaySpinner = true;


        public bool ShowJSON = false;






#region Page Events
        protected override async Task OnInitializedAsync()
        {
            NavDisplay = "Chapter";

            await SelectChapter();

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
                    GenericErrorLog(false,e, "OnAfterRenderAsync", e.Message);  
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
                                SelectedChapter = JsonConvert.DeserializeObject<VmSmartflow>(currentChapter.SmartflowData);

                                RefreshChapterItems("All");
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
                GenericErrorLog(true,e, "OnTimerInterval", e.Message);
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

#region Navigation 


        public async void SelectHome()
        {
            await _SelectHome.InvokeAsync();
        }


    

#endregion

#region Chapter Details

        private async Task SelectChapter()
        {
            try
            {

                //Single Smartflow selected, display library details
                P4WCaseTypeGroups = await PartnerAccessService.GetPartnerCaseTypeGroups();
                

                if (!(_SelectedChapterObject.SmartflowObject.SmartflowData is null))
                {
                    SelectedChapter = JsonConvert.DeserializeObject<VmSmartflow>(_SelectedChapterObject.SmartflowObject.SmartflowData);

                }
                else
                {

                    //Initialise the VmSmartflow in case of null Json
                    SelectedChapter = new VmSmartflow
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
                //SelectedChapter.Items = SelectedChapter.Items is null ? new List<GenSmartflowItem>() : SelectedChapter.Items;
                //SelectedChapter.DataViews = SelectedChapter.DataViews is null ? new List<DataView>() : SelectedChapter.DataViews;
                //SelectedChapter.Fees = SelectedChapter.Fees is null ? new List<Fee>() : SelectedChapter.Fees;
                //SelectedChapter.TickerMessages = SelectedChapter.TickerMessages is null ? new List<TickerMessage>() : SelectedChapter.TickerMessages;
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

                    GenericErrorLog(false,e, "SelectChapter", $"Getting document list after Smartflow selected for edit: {e.Message}");

                    DisplaySpinner = false;

                    //return user to the full list of Smartflows. Do not present the user with the details of the Smartflow as none of the features will work.
                    SelectHome();

                    return;
                }


                //await RefreshChapterItems("All");


                if (!string.IsNullOrEmpty(SelectedChapter.BackgroundImage))
                {
                    UserSession.SetTempBackground(SelectedChapter.BackgroundImage.Replace("/wwwroot", ""), NavigationManager.Uri);
                    UserSession.RefreshHome?.Invoke();
                }

                // await SetSmartflowFilePath();
                // GetSeletedChapterFileList();


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

                GenericErrorLog(true,ex, "SelectChapter", $"Loading selected Smartflow: {ex.Message}");

            }
            finally
            {
                
                StateHasChanged();


            }


        }


        private async Task ChapterUpdated(VmSmartflow _selectedChapter)
        {

            SelectedChapter = _selectedChapter;

            _SelectedChapterObject.SmartflowObject.SmartflowData = JsonConvert.SerializeObject(SelectedChapter);
            await ChapterManagementService.Update(_SelectedChapterObject.SmartflowObject);

            if(PreviewChapterImage)
            {
                UpdatePreviewImage(true);
            }

            AppChapterState.SetLastUpdated(UserSession, SelectedChapter);
            
        }



        private void RefreshChapterItems(string listType)
        {
            try
            {
            
                /*
                * listType = All when chapter is selected, 
                * listType = nav selected e.g. Agenda when and object in a specific list has been altered
                * 
                */
                if (listType == "Agenda" | listType == "All")
                {
                    LstAgendas = SelectedChapter.Items
                                        .Select(L => new VmGenSmartflowItem { ChapterObject = L })
                                        .Where(L => L.ChapterObject.Type == "Agenda")
                                        .OrderBy(L => L.ChapterObject.Name)
                                        .ToList();

                }
                if (listType == "Docs" | listType == "All")
                {

                    Dictionary<int?, string> docTypes = new Dictionary<int?, string> { { 1, "Doc" }, { 4, "Form" }, { 6, "Step" }, { 8, "Date" }, { 9, "Email" }, { 11, "Doc" }, { 12, "Email" }, { 13, "Csv" } };
                    LstDocs = SelectedChapter.Items
                                        .Select(L => new VmGenSmartflowItem { ChapterObject = L })
                                        .Where(L => L.ChapterObject.Type == "Doc")
                                        .OrderBy(L => L.ChapterObject.SeqNo)
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
                                                                                                    .Select(LI => {
                                                                                                        LI.DocType = LibraryDocumentsAndSteps
                                                                                                                .Where(D => D.Name.ToUpper() == LI.DocName.ToUpper())
                                                                                                                .Select(D => docTypes.ContainsKey(D.DocumentType) ? docTypes[D.DocumentType] : "Doc")
                                                                                                                .FirstOrDefault();
                                                                                                        return LI;
                                                                                                    }
                                                                                        ).ToList();
                                            return A;
                                        })
                                        .ToList();

                    if(LstDocs.Where(C => C.ChapterObject.SeqNo != LstDocs.IndexOf(C) + 1).Count() > 0) //If any SeqNos are out of sequence
                    { 
                        LstDocs.Select(C => { C.ChapterObject.SeqNo = LstDocs.IndexOf(C) + 1; return C; }).ToList();
                    }



                }
                if (listType == "Fees" | listType == "All")
                {
                    LstFees = SelectedChapter.Fees.Select(F => new VmFee { FeeObject = F })
                                                            .OrderBy(F => F.FeeObject.SeqNo)
                                                            .ToList();

                    if(LstFees.Where(C => C.FeeObject.SeqNo != (LstFees.IndexOf(C) + 1)).Count() > 0) //If any SeqNos are out of sequence
                    { 
                        LstFees.Select(C => { C.FeeObject.SeqNo = LstFees.IndexOf(C) + 1; return C; }).ToList();
                    }


                }
                if (listType == "Status" | listType == "All")
                {
                    LstStatus = SelectedChapter.Items
                                        .Select(L => new VmGenSmartflowItem { ChapterObject = L })
                                        .Where(L => L.ChapterObject.Type == "Status")
                                        .OrderBy(L => L.ChapterObject.SeqNo)
                                        .ToList();
                    
                    if(LstStatus.Where(C => C.ChapterObject.SeqNo != LstStatus.IndexOf(C) + 1).Count() > 0) //If any SeqNos are out of sequence
                    { 
                        LstStatus.Select(C => { C.ChapterObject.SeqNo = LstStatus.IndexOf(C) + 1; return C; }).ToList();
                    }
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

                    if(LstDataViews.Where(C => C.DataView.SeqNo != LstDataViews.IndexOf(C) + 1).Count() > 0) //If any SeqNos are out of sequence
                    { 
                        LstDataViews.Select(C => { C.DataView.SeqNo = LstDataViews.IndexOf(C) + 1; return C; }).ToList(); //update all SeqNo by setting to the Index (+1)
                    }

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
                    
                    if(LstTickerMessages.Where(C => C.Message.SeqNo != LstTickerMessages.IndexOf(C) + 1).Count() > 0) //If any SeqNos are out of sequence
                    { 
                        LstTickerMessages.Select(C => { C.Message.SeqNo = LstTickerMessages.IndexOf(C) + 1; return C; }).ToList(); //update all SeqNo by setting to the Index (+1)
                    }

                    TickerValidation();

                }
            
                SeqMoving = false;

                InvokeAsync(() => StateHasChanged()); //maybe called from a Timer event and not direct from UI thread so change state safely
                
    
            }
            catch (Exception e)
            {
                GenericErrorLog(true,e, "RefreshChapterItems", $"Refreshing Smartflow tab items: {e.Message}");

            }
            
        }

        
        protected void ShowNav(string displayChange)
        {
            CompareSystems = false;
            RowChanged = 0;

            RefreshChapterItems(displayChange);

            NavDisplay = displayChange;
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

         //Used on Header tab and Background tab so keep generic in here and share resulting properties

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

                

#endregion



#region Error Handling



        /****************************************/
        /* ERROR HANDLING */
        /****************************************/
        private void GenericErrorLog(bool showNotificationMsg, Exception e, string _method, string _message)
        {
            if(showNotificationMsg)
            {
                NotificationManager.ShowNotification("Danger", $"Oops! Something went wrong.");
            }

            using (LogContext.PushProperty("SourceSystem", UserSession.SelectedSystem))
            using (LogContext.PushProperty("SourceCompanyId", UserSession.Company.Id))
            using (LogContext.PushProperty("SourceUserId", UserSession.User.Id))
            using (LogContext.PushProperty("SourceContext", nameof(ChapterDetail)))
            {
                Logger.LogError(e,"Error - Method: {0}, Message: {1}",_method, _message);
            }

        }


#endregion

    }
}
