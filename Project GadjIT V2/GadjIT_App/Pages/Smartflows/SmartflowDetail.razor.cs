using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GadjIT_App.Services;
using GadjIT_App.Services.SessionState;
using Newtonsoft.Json;
using Microsoft.JSInterop;
using GadjIT_App.Data.Admin;
using System.Globalization;
using GadjIT_App.Services.AppState;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using Microsoft.Extensions.Configuration;
using GadjIT_AppContext.GadjIT_App;
using GadjIT_ClientContext.Models.P4W;
using GadjIT_ClientContext.Models.Smartflow;
using GadjIT_ClientContext.Models.Smartflow.Client;

namespace GadjIT_App.Pages.Smartflows
{
    public partial class SmartflowDetail
    {

        
        [Parameter]
        public Client_VmSmartflowRecord _Selected_VmClientSmartflowRecord { get; set; }

        [Parameter]
        public string _SelectedCaseTypeGroup { get; set; }

        [Parameter]
        public string _SelectedCaseType { get; set; }

        [Parameter]
        public EventCallback _SelectHome {get; set;}

        [Inject]
        private IJSRuntime JSRuntime {get; set;}

        [Inject]
        public INotificationManager NotificationManager {get; set;}

        [Inject]
        private IClientApiManagementService ClientApiManagementService { get; set; }

        [Inject]
        private IPartnerAccessService PartnerAccessService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public IUserSessionState UserSession { get; set; }

        [Inject]
        private IIdentityUserAccess UserAccess { get; set; }

        [Inject]
        public IAppSmartflowsState AppSmartflowsState { get; set; }

        [Inject]
        private ICompanyDbAccess CompanyDbAccess { get; set; }

        [Inject]
        private ILogger<SmartflowList> Logger { get; set; }

        [Inject]
        public IConfiguration Configuration { get; set;}

        

        private int ValidTicketMessageCount { get; set; } 


        private List<VmSmartflowAgenda> LstAgendas { get; set; } = new List<VmSmartflowAgenda>();
        private List<VmSmartflowFee> LstFees { get; set; } = new List<VmSmartflowFee>();
        private List<VmSmartflowDocument> LstDocs { get; set; } = new List<VmSmartflowDocument>();
        private List<VmSmartflowStatus> LstStatus { get; set; } = new List<VmSmartflowStatus>();
        private List<VmSmartflowDataView> LstDataViews { get; set; } = new List<VmSmartflowDataView>();
        private List<VmSmartflowMessage> LstMessages { get; set; } = new List<VmSmartflowMessage>();

        public List<P4W_MpSysViews> ListP4WViews;
        public List<P4W_DmDocuments> LibraryDocumentsAndSteps;
        public List<P4W_TableDate> TableDates;
        public List<P4W_CaseTypeGroups> P4WCaseTypeGroups;

        public String UserGuideURL => Configuration["AppSettings:UserGuideURL"];
        
        
        public SmartflowV2 SelectedSmartflow { get; set; } = new SmartflowV2(); //SmartflowData

        int RowChanged { get; set; } = 0; //moved partial

        private int SelectedSmartflowId { get; set; } = -1;

        public string NavDisplay = "Chapter";

        private bool SeqMoving = false;

        protected bool CompareSystems = false;

        public bool DisplaySpinner = true;


        public bool ShowJSON = false;

        public bool SmartflowLockedForEdit {get; set;}

        public AspNetUser SmartflowLockedBy {get; set;} = new AspNetUser{ UserName = "", FullName = ""};

        private Timer TimerStateChanged;



#region Page Events
        protected override async Task OnInitializedAsync()
        {
            NavDisplay = "Chapter";

            try
            {
                //Only restrict access if another user has the Smartflow locked.
                //NOTE: A user can open another Smartflow in another Tab/Browser. In this instance
                //the latest Smartflow to be opened will get the lock and all other locks will be cleared 
                bool isLocked = AppSmartflowsState.IsSmartflowLockedByOtherUser(
                                                    UserSession.User
                                                    , _Selected_VmClientSmartflowRecord.ClientSmartflowRecord.Id
                                                );
                
                

                if(isLocked)
                {
                    
                    SmartflowLockedBy = AppSmartflowsState.SmartflowIsLockedBy(_Selected_VmClientSmartflowRecord.ClientSmartflowRecord.Id);

                    //Add entry into list of Smartflows currently open on AppSmartflowState
                    //but for viewing only
                    //This allows us to make sure the user only views/edits one Smartflow at a time
                    
                    AppSmartflowsState.LockSmartflow(
                                                    UserSession.User
                                                    , _Selected_VmClientSmartflowRecord.ClientSmartflowRecord.Id
                                                    , false //Not Locked for other users as only viewing
                                                    );

                    SmartflowLockedForEdit = false;
                }
                else
                {
                    AppSmartflowsState.DisposeUser(UserSession.User); //make sure no other locks against current user
                    
                    //Add entry into list of Smartflows currently open on AppSmartflowState
                    //but for editing so it prevents any other user from openeing for Edits
                    AppSmartflowsState.LockSmartflow(
                                                    UserSession.User
                                                    , _Selected_VmClientSmartflowRecord.ClientSmartflowRecord.Id
                                                    , true //Locked for other users
                                                    );

                    SmartflowLockedBy = UserSession.User;
                    SmartflowLockedForEdit = true;

                    Logger.LogInformation("Smartflow Locked: By: {0}, Company {1}, System: {2}, CTG: {3}, CT: {4}, SF: {5}"
                                                                            , UserSession.User.FullName
                                                                            , UserSession.Company.CompanyName
                                                                            , UserSession.SelectedSystem
                                                                            , _Selected_VmClientSmartflowRecord.ClientSmartflowRecord.CaseTypeGroup
                                                                            , _Selected_VmClientSmartflowRecord.ClientSmartflowRecord.CaseType
                                                                            , _Selected_VmClientSmartflowRecord.ClientSmartflowRecord.SmartflowName);
                }

                await SelectSmartflow();

                TimerStateChanged = new Timer(async _ =>
                {
                    try
                    {
                        await InvokeAsync(() =>
                        {
                            CheckIfLostLockOnSmartflow();
                        });

                    }
                    catch(Exception e)
                    {
                        GenericErrorLog(false,e, "OnInitializedAsync", $"Attempting to Invoke _RefreshSmartflowsTask"); 
                    }

                }, null, 25000, 20000);
            }
            catch(Exception e)
            {
                //await NotificationManager.ShowNotification("Danger", $"Oops! Something went wrong accessing the Smartflow details");

                GenericErrorLog(false,e, "OnInitializedAsync", $"Attempting to lock the Smartflow for editing: {e.Message}");
            }

        }


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
            
            base.OnAfterRender(firstRender);
        }

        private async void CheckIfLostLockOnSmartflow()
        {
            if(!Disposed && _Selected_VmClientSmartflowRecord.ClientSmartflowRecord != null && UserSession.User != null)
            {
                bool isLocked = AppSmartflowsState.IsSmartflowLockedByOtherUser(
                                                UserSession.User
                                                , _Selected_VmClientSmartflowRecord.ClientSmartflowRecord.Id
                                            );
                    
                if(isLocked)
                {
                    bool dataChanged = false;

                    var smartflowLockedBy = AppSmartflowsState.SmartflowIsLockedBy(_Selected_VmClientSmartflowRecord.ClientSmartflowRecord.Id);
                    if(
                        (
                            smartflowLockedBy != null 
                            && SmartflowLockedBy != null 
                            && smartflowLockedBy.Id != SmartflowLockedBy.Id //changed users
                        ) 
                        || 
                        (
                            smartflowLockedBy != null 
                            && SmartflowLockedBy == null  //user has lock but previously no user had the lock
                        )
                    )
                    {
                        SmartflowLockedBy = new AspNetUser{
                                                        Id = smartflowLockedBy.Id
                                                        , FullName = smartflowLockedBy.FullName
                                                        , UserName = smartflowLockedBy.UserName
                                                        , Email = smartflowLockedBy.Email
                                                        };
                        dataChanged = true;
                    }
                    else if(smartflowLockedBy == null && SmartflowLockedBy != null) //current user has lost lock but no other user has the lock (lock removed)
                    {
                        SmartflowLockedBy = null;

                        dataChanged = true;
                    }

                    if(SmartflowLockedForEdit)
                    {
                        //Make sure there are no other Smartflows locked
                        //Add an enty in the AppSmartflowState list for the current Smartflow but with no lock to prevent user from opening other Smartflows
                        AppSmartflowsState.DisposeUser(UserSession.User);

                        AppSmartflowsState.LockSmartflow(
                                                        UserSession.User
                                                        , _Selected_VmClientSmartflowRecord.ClientSmartflowRecord.Id
                                                        , false //Not Locked for other users as only viewing
                                                        );


                        SmartflowLockedForEdit = false;

                        await InvokeAsync(() =>
                        {
                            NotificationManager.ShowNotification("Warning", $"You no longer have control over this Smartflow. Re-open if you wish to make changes.");
                        });

                        dataChanged = true;
                        
                    }

                    if(dataChanged)
                    {
                        await InvokeAsync(() =>
                        {
                            StateHasChanged();
                        });

                    }

                    
                    
                }
            }
            
        }

        protected AspNetUser SmartflowIsLockedBy()
        {
            try
            {

                AspNetUser lockedBy = AppSmartflowsState.SmartflowIsLockedBy(_Selected_VmClientSmartflowRecord.ClientSmartflowRecord.Id);

                if(lockedBy == null)
                {
                    throw new Exception();
                }

                return lockedBy;

            }
            catch(Exception)
            {
                return new AspNetUser{ UserName = "", FullName = ""};
            }
        }

        

        private bool Disposed = false; 

        public void Dispose()
        {
            if(!Disposed)
            {
                AppSmartflowsState.DisposeUser(UserSession.User);
                Disposed = true;   
            } 
        }

#endregion

#region Navigation 


        public async void SelectHome()
        {
            AppSmartflowsState.UnlockSmartflowForUser(
                                                UserSession.User
                                                , _Selected_VmClientSmartflowRecord.ClientSmartflowRecord.Id
                                            );

            await _SelectHome.InvokeAsync();

            Disposed = true;
        }


    

#endregion

#region Chapter Details

        private async Task SelectSmartflow()
        {
            try
            {

                //Single Smartflow selected, display library details
                P4WCaseTypeGroups = await PartnerAccessService.GetPartnerCaseTypeGroups();
                

                if (!(_Selected_VmClientSmartflowRecord.ClientSmartflowRecord.SmartflowData is null))
                {
                    SelectedSmartflow = JsonConvert.DeserializeObject<SmartflowV2>(_Selected_VmClientSmartflowRecord.ClientSmartflowRecord.SmartflowData);

                    //It may be that some Smartflows have been copied/moved from other Case Types
                    //We need to perform a quick validation chaeck to make sure that the Smartflow 
                    //object refers to the correct Case Type Group and Case Type. i.e. the one it is listed against
                    if(SelectedSmartflow.CaseTypeGroup != _SelectedCaseTypeGroup || SelectedSmartflow.CaseType != _SelectedCaseType)
                    {
                        SelectedSmartflow.CaseTypeGroup = _SelectedCaseTypeGroup;
                        SelectedSmartflow.CaseType = _SelectedCaseType;

                        try
                        {
                            _Selected_VmClientSmartflowRecord.ClientSmartflowRecord.SmartflowData = JsonConvert.SerializeObject(SelectedSmartflow);
                            await ClientApiManagementService.Update(_Selected_VmClientSmartflowRecord.ClientSmartflowRecord);
                        }
                        catch (Exception e)
                        {
                            GenericErrorLog(true,e, "SelectSmartflow", $"Updating Smartflow Case Type details: {e.Message}");
                        }


                    }

                }
                else
                {

                    //Initialise the VmSmartflow in case of null Json
                    SelectedSmartflow = new SmartflowV2
                    {
                        Agendas = new List<SmartflowAgenda>()
                        , Status = new List<SmartflowStatus>()
                        , Documents = new List<SmartflowDocument>()                     
                        , DataViews = new List<SmartflowDataView>()                     
                        , Messages = new List<SmartflowMessage>()                             
                        , Fees = new List<SmartflowFee>()
                    };
                    SelectedSmartflow.CaseTypeGroup = _SelectedCaseTypeGroup;
                    SelectedSmartflow.CaseType = _SelectedCaseType;
                    SelectedSmartflow.Name = _Selected_VmClientSmartflowRecord.ClientSmartflowRecord.SmartflowName;

                }

                SelectedSmartflow.StepName = $"SF {_Selected_VmClientSmartflowRecord.ClientSmartflowRecord.SmartflowName} Smartflow";
                SelectedSmartflow.SelectedStep = SelectedSmartflow.SelectedStep is null || SelectedSmartflow.SelectedStep == "Create New" ? "" : SelectedSmartflow.SelectedStep;
                SelectedSmartflowId = _Selected_VmClientSmartflowRecord.ClientSmartflowRecord.Id;
                CompareSystems = false;
                RowChanged = 0;
                NavDisplay = "Chapter";
                ShowJSON = false;


                try
                {
                    await RefreshLibraryDocumentsAndSteps();
                    
                    //LibraryDocumentsAndSteps = LibraryDocumentsAndSteps.Where(D => !(D.Name is null)).ToList();
                    TableDates = await ClientApiManagementService.GetDatabaseTableDateFields();
                    ListP4WViews = await PartnerAccessService.GetPartnerViews();
                }
                catch (Exception e)
                {
                    await NotificationManager.ShowNotification("Danger", $"Oops! Something went wrong accessing the Smartflow details");

                    GenericErrorLog(false,e, "SelectSmartflow", $"Getting document list after Smartflow selected for edit: {e.Message}");

                    DisplaySpinner = false;

                    //return user to the full list of Smartflows. Do not present the user with the details of the Smartflow as none of the features will work.
                    SelectHome();

                    return;
                }


                RefreshSmartflowItems("All"); //required as some lists are passed down to componetnts


                if (!string.IsNullOrEmpty(SelectedSmartflow.BackgroundImage))
                {
                    UserSession.SetTempBackground(SelectedSmartflow.BackgroundImage.Replace("/wwwroot", ""), NavigationManager.Uri);
                    UserSession.RefreshHome?.Invoke();
                }

                // await SetSmartflowFilePath();
                // GetSeletedChapterFileList();


                

                await JSRuntime.InvokeVoidAsync("moveToPosition", 0);

            }
            catch (Exception ex)
            {

                GenericErrorLog(true,ex, "SelectSmartflow", $"Loading selected Smartflow: {ex.Message}");

            }
            finally
            {
                
                StateHasChanged();


            }


        }

        private async Task RefreshLibraryDocumentsAndSteps()
        {
            try
            {
                LibraryDocumentsAndSteps = await ClientApiManagementService.GetDocumentList(SelectedSmartflow.CaseType);
            }
            catch (Exception ex)
            {

                GenericErrorLog(true,ex, "RefreshLibraryDocumentsAndSteps", $"Refreshing LibraryDocumentsAndSteps: {ex.Message}");

            }
        }
        


        private async Task SmartflowUpdated(SmartflowV2 _selectedSmartflow)
        {

            SelectedSmartflow = _selectedSmartflow;

            _Selected_VmClientSmartflowRecord.ClientSmartflowRecord.SmartflowData = JsonConvert.SerializeObject(SelectedSmartflow);
            await ClientApiManagementService.Update(_Selected_VmClientSmartflowRecord.ClientSmartflowRecord);

            if(PreviewSmartflowImage)
            {
                UpdatePreviewImage(true);
            }

            
        }



        private void RefreshSmartflowItems(string listType)
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
                    LstAgendas = SelectedSmartflow.Agendas
                                        .Select(L => new VmSmartflowAgenda { SmartflowObject = L })
                                        .OrderBy(L => L.SmartflowObject.Name)
                                        .ToList();

                }
                if (listType == "Docs" | listType == "All")
                {

                    Dictionary<int?, string> docTypes = new Dictionary<int?, string> { { 1, "Doc" }, { 4, "Form" }, { 6, "Step" }, { 8, "Date" }, { 9, "Email" }, { 11, "Doc" }, { 12, "Email" }, { 13, "Csv" } };
                    LstDocs = SelectedSmartflow.Documents
                                        .Select(L => new VmSmartflowDocument{ SmartflowObject = L })
                                        .OrderBy(L => L.SmartflowObject.SeqNo)
                                        .Select(A => {
                                        //Make sure all Items have the DocType set by comparing against dm_documents for matches
                                        A.DocType = LibraryDocumentsAndSteps.Where(D => D.Name.ToUpper() == A.SmartflowObject.Name.ToUpper())
                                                                                    .Select(D => docTypes.ContainsKey(D.DocumentType) ? docTypes[D.DocumentType] : "Doc")
                                                                                    .FirstOrDefault();
                                            A.SmartflowObject.RescheduleDays = !string.IsNullOrEmpty(A.SmartflowObject.AsName) && A.SmartflowObject.RescheduleDays is null ? 0 : A.SmartflowObject.RescheduleDays;
                                            A.SmartflowObject.Action = (A.SmartflowObject.Action == "" ? "INSERT" : A.SmartflowObject.Action);
                                        //Make sure all Linked Items have the DocType set by comparing against dm_documents for matches
                                        A.SmartflowObject.LinkedItems = A.SmartflowObject.LinkedItems == null
                                                                                    ? new List<LinkedItem>()
                                                                                    : A.SmartflowObject.LinkedItems
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

                    if(LstDocs.Where(C => C.SmartflowObject.SeqNo != LstDocs.IndexOf(C) + 1).Count() > 0) //If any SeqNos are out of sequence
                    { 
                        LstDocs.Select(C => { C.SmartflowObject.SeqNo = LstDocs.IndexOf(C) + 1; return C; }).ToList();
                    }



                }
                if (listType == "Fees" | listType == "All")
                {
                    LstFees = SelectedSmartflow.Fees.Select(F => new VmSmartflowFee { FeeObject = F })
                                                            .OrderBy(F => F.FeeObject.SeqNo)
                                                            .ToList();

                    if(LstFees.Where(C => C.FeeObject.SeqNo != (LstFees.IndexOf(C) + 1)).Count() > 0) //If any SeqNos are out of sequence
                    { 
                        LstFees.Select(C => { C.FeeObject.SeqNo = LstFees.IndexOf(C) + 1; return C; }).ToList();
                    }


                }
                if (listType == "Status" | listType == "All")
                {
                    LstStatus = SelectedSmartflow.Status
                                        .Select(L => new VmSmartflowStatus { SmartflowObject = L })
                                        .OrderBy(L => L.SmartflowObject.SeqNo)
                                        .ToList();
                    
                    if(LstStatus.Where(C => C.SmartflowObject.SeqNo != LstStatus.IndexOf(C) + 1).Count() > 0) //If any SeqNos are out of sequence
                    { 
                        LstStatus.Select(C => { C.SmartflowObject.SeqNo = LstStatus.IndexOf(C) + 1; return C; }).ToList();
                    }
                }
                if (listType == "DataViews" | listType == "All")
                {
                    LstDataViews = (SelectedSmartflow.DataViews is null)
                                                    ? new List<VmSmartflowDataView>()
                                                    : SelectedSmartflow
                                                            .DataViews
                                                            .Select(D => new VmSmartflowDataView { DataView = D })
                                                            .OrderBy(D => D.DataView.SeqNo)
                                                            .ToList();

                    if(LstDataViews.Where(C => C.DataView.SeqNo != LstDataViews.IndexOf(C) + 1).Count() > 0) //If any SeqNos are out of sequence
                    { 
                        LstDataViews.Select(C => { C.DataView.SeqNo = LstDataViews.IndexOf(C) + 1; return C; }).ToList(); //update all SeqNo by setting to the Index (+1)
                    }

                }
                if (listType == "TickerMessages" | listType == "All")
                {
                    LstMessages = (SelectedSmartflow.Messages is null)
                                                    ? new List<VmSmartflowMessage>()
                                                    : SelectedSmartflow
                                                            .Messages
                                                            .Select(D => new VmSmartflowMessage { Message = D })
                                                            .OrderBy(D => D.Message.SeqNo)
                                                            .ToList();
                    
                    if(LstMessages.Where(C => C.Message.SeqNo != LstMessages.IndexOf(C) + 1).Count() > 0) //If any SeqNos are out of sequence
                    { 
                        LstMessages.Select(C => { C.Message.SeqNo = LstMessages.IndexOf(C) + 1; return C; }).ToList(); //update all SeqNo by setting to the Index (+1)
                    }

                    TickerValidation();

                }
            
                SeqMoving = false;

                InvokeAsync(() => StateHasChanged()); //maybe called from a Timer event and not direct from UI thread so change state safely
                
    
            }
            catch (Exception e)
            {
                GenericErrorLog(true,e, "RefreshSmartflowItems", $"Refreshing Smartflow tab items: {e.Message}");

            }
            
        }

        
        protected void ShowNav(string displayChange)
        {
            CompareSystems = false;
            RowChanged = 0;

            RefreshSmartflowItems(displayChange);

            NavDisplay = displayChange;
        }

        
        private void TickerValidation()
        {
            var currentDate = DateTime.Now.Date;

            ValidTicketMessageCount = 1;
            foreach (VmSmartflowMessage msg in LstMessages)
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
            PreviewSmartflowImage = preview;
        }

        private bool PreviewSmartflowImage
        {
            get 
            {
                return UserSession.User.DisplaySmartflowPreviewImage; 
            }
            set
            {
                if (value)
                {
                    if(!string.IsNullOrEmpty(SelectedSmartflow.BackgroundImageName))
                    {
                        UserSession.SetTempBackground(SelectedSmartflow.BackgroundImage.Replace("/wwwroot",""), NavigationManager.Uri);
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
                SavePreviewSmartflowImage();
            }

        }

        private async void SavePreviewSmartflowImage()
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
                GenericErrorLog(true,e, "SavePreviewSmartflowImage", e.Message);
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
            using (LogContext.PushProperty("SourceContext", nameof(SmartflowDetail)))
            {
                Logger.LogError(e,"Error - Method: {0}, Message: {1}",_method, _message);
            }

        }


#endregion

    }
}
