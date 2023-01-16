using AutoMapper;
using Blazored.Modal;
using Blazored.Modal.Services;
using GadjIT_App.Services;
using GadjIT_App.Services.SessionState;
using GadjIT_App.Pages._Shared.Modals;
using GadjIT_ClientContext.P4W.Custom;
using GadjIT_App.Data.Admin;
using GadjIT_App.Services.AppState;
using GadjIT_ClientContext.P4W;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;
using GadjIT_App.Pages.Chapters.ComponentsChapterList;

namespace GadjIT_App.Pages.Chapters
{
    public partial class ChapterList
    {
        
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
        public IUserSessionState UserSession { get; set; }

        [Inject]
        public IAppChapterState AppChapterState { get; set; }

        [Inject]
        private ICompanyDbAccess CompanyDbAccess { get; set; }

        [Inject]
        private ILogger<ChapterList> Logger { get; set; }


        [Inject]
        private IPageAuthorisationState pageAuthorisationState { get; set; }

        [Inject]
        public IConfiguration Configuration { get; set;}

        private List<VmUsrOrsfSmartflows> LstChapters { get; set; } = new List<VmUsrOrsfSmartflows>();

        private List<VmUsrOrsfSmartflows> LstAltSystemChapters { get; set; } = new List<VmUsrOrsfSmartflows>();


        public List<VmUsrOrsfSmartflows> LstSelectedChapters { get; set; } = new List<VmUsrOrsfSmartflows>();

        public string EditCaseType { get; set; } = "";
        public bool CreateNewSmartflow { get; set; }

        public float ScrollPosition { get; set; }

        public bool SmartflowComparison 
        {
            get 
            {
                return CompareSystems;
            }
            set
            {
                CompareSystems = value;
                if(CompareSystems)
                {
                    CompareSelectedChapters();
                }
                else{
                    StateHasChanged();
                }
            }
        }



        public String UserGuideURL => Configuration["AppSettings:UserGuideURL"];
        

        public UsrOrsfSmartflows EditChapter { get; set; }
        public string IsCaseTypeOrGroup { get; set; } = "";

        public UsrOrsfSmartflows EditChapterObject = new UsrOrsfSmartflows ();



        [Parameter]
        public VmUsrOrsfSmartflows SelectedChapterObject { get; set; } = new VmUsrOrsfSmartflows(); //full object
        public VmUsrOrsfSmartflows AltChapterObject { get; set; } = new VmUsrOrsfSmartflows();


        public VmChapter SelectedChapter { get; set; } = new VmChapter { Items = new List<GenSmartflowItem>() }; //SmartflowData


        int RowChanged { get; set; } = 0; //moved partial


        private bool SeqMoving = false;

        protected bool CompareSystems = false;

        private string RowChangedClass { get; set; } = "row-changed-nav3";
       
        public bool ListChapterLoaded = false;

        

#region Page Events
        protected override async Task OnInitializedAsync()
        {
            var authenticationState = await pageAuthorisationState.IsSignedIn();

 
            if (authenticationState)
            {
                bool gotLock = UserSession.Lock;
                while (gotLock)
                {
                    await Task.Yield();
                    gotLock = UserSession.Lock;
                }


                try
                {
                    RefreshChapters();
                    //P4WCaseTypeGroups = await PartnerAccessService.GetPartnerCaseTypeGroups();
                    //ListP4WViews = await PartnerAccessService.GetPartnerViews();
                    UserSession.HomeActionSmartflow = SelectHome;

                }
                catch (Exception e)
                {
                    //Note: do not show notification as JsRuntime is not available until After Render
                    await GenericErrorLog(false,e, "OnInitializedAsync", $"Loading initial Smartflow list: {e.Message}");
                    
                }
            }

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
                timer = new Timer();
                timer.Interval = 10000; //10 seconds
                timer.Elapsed += OnTimerInterval;
                timer.AutoReset = true;
                // Start the timer
                timer.Enabled = true;
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

#region Navigation and Drag Drop and Resequence List


        public async void SelectHome()
        {
            try
            {
                //NavigationManager.NavigateTo($"Smartflow/{SelectedChapter.CaseTypeGroup}/{SelectedChapter.CaseType}",true);
                if (!string.IsNullOrEmpty(UserSession.TempBackGroundImage))
                {
                    UserSession.TempBackGroundImage = "";
                    UserSession.RefreshHome?.Invoke();
                }
                CompareSystems = false;
                SelectedChapterObject.SmartflowObject = null;
                SelectedChapter.Name = "";
                ResetRowChanged();

                StateHasChanged();


                await MovePos().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                await GenericErrorLog(false,e, "SelectHome", e.Message);
            }
          
        }

        public async Task MovePos()
        {
            try
            {
                await Task.Delay(1);
                await JSRuntime.InvokeVoidAsync("moveToPosition", ScrollPosition);
            }
            catch (Exception e)
            {
                await GenericErrorLog(false,e, "MovePos", e.Message);
            }
        }       
        
        public void ResetRowChanged() 
        {
            RowChanged = 0;
        }

        Timer timerRowChanged;

        public void ResetRowChangedHandler(object sender, ElapsedEventArgs eventArgs) 
        {
            RowChanged = 0;
        }

        private async void ResetRowChangedDelayed() 
        {
            timerRowChanged = new Timer();
            timerRowChanged.Interval = 1300; //1.3 seconds
            timerRowChanged.Elapsed += ResetRowChangedHandler;
            timerRowChanged.AutoReset = false;
            // Start the timer
            timerRowChanged.Enabled = true;
        }
        
    
        public async Task ReSequenceSmartFlows(int seq)
        {
            RowChanged = seq;
            await ReSequenceSmartFlows();

            ResetRowChangedDelayed();
        }

        public async Task ReSequenceSmartFlows()
        {
            try
            {
                if(LstSelectedChapters.Select(C => C.SmartflowObject.SeqNo != LstSelectedChapters.IndexOf(C) + 1).Count() > 0) //If any SeqNos are out of sequence
                { 
                    LstSelectedChapters.Select(C => { C.SmartflowObject.SeqNo = LstSelectedChapters.IndexOf(C) + 1; return C; }).ToList(); //update all SeqNo by setting to the Index (+1)

                    foreach (var smartflowToChange in LstSelectedChapters)
                    {
                        await ChapterManagementService.UpdateMainItem(smartflowToChange.SmartflowObject).ConfigureAwait(false);

                    }

                    StateHasChanged();
                }

                
            }
            catch
            {
                
            }

        }

        protected async void MoveSmartFlowSeq(UsrOrsfSmartflows selectobject, string listType, string direction)
        {
            try
            {
                SeqMoving = true; //prevents changes to the form whilst process of changing seq is carried out

                var lstItems = new List<VmUsrOrsfSmartflows>();
                int incrementBy;

                incrementBy = (direction.ToLower() == "up" ? -1 : 1);

                RowChanged = (int)(selectobject.SeqNo + incrementBy);

                lstItems = LstSelectedChapters
                            .OrderBy(A => A.SmartflowObject.SeqNo)
                            .ToList();


                var swapItem = lstItems.Where(D => D.SmartflowObject.SeqNo == (selectobject.SeqNo + incrementBy)).SingleOrDefault();
                if (!(swapItem is null))
                {
                    selectobject.SeqNo += incrementBy;
                    swapItem.SmartflowObject.SeqNo = swapItem.SmartflowObject.SeqNo + (incrementBy * -1);

                    await ChapterManagementService.UpdateMainItem(selectobject);
                    await ChapterManagementService.UpdateMainItem(swapItem.SmartflowObject);
                }

                await RefreshSelectedChapters();

                
                await InvokeAsync(() =>
                {
                    StateHasChanged();
                });
                
                ResetRowChangedDelayed();
                

            }
            catch (Exception e)
            {
                await GenericErrorLog(true,e, "MoveSmartFlowSeq", $"Moving smartflow: {e.Message}");

            }
            finally
            {
                SeqMoving = false;
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


       

        

#endregion

#region Chapter Listing


        private async void RefreshChapters()
        {
            /// <summary>
            /// Must be a standard void method so it can be assigned as an Action to Modals
            /// </summary>
                       
            ListChapterLoaded = false;

            bool gotLock = CompanyDbAccess.Lock;
            while (gotLock)
            {
                await Task.Yield();
                gotLock = CompanyDbAccess.Lock;
            }

            try
            {
                var LstAllChapterItemsChapters = await CompanyDbAccess.GetAllSmartflowRecords(UserSession);
                //generate a copy of the Smartflow Records but converted to VmUsrOrsfSmartflows 
                LstChapters = LstAllChapterItemsChapters.Select(A => new VmUsrOrsfSmartflows { SmartflowObject = Mapper.Map(A, new UsrOrsfSmartflows()) }).ToList();
                

                LstSelectedChapters = LstChapters.Where(C => C.SmartflowObject.CaseType == SelectedChapter.CaseType)
                                        .Where(C => C.SmartflowObject.CaseTypeGroup == SelectedChapter.CaseTypeGroup)
                                        .OrderBy(C => C.SmartflowObject.SeqNo)
                                        .ToList();

                await ReSequenceSmartFlows(); //makes sure the sequence numbers are all sequential, corrects any issues
                await RefreshChapterIssues();

            }
            catch(Exception e)
            {
                LstChapters = new List<VmUsrOrsfSmartflows>();
                LstSelectedChapters = new List<VmUsrOrsfSmartflows>();
                // P4WCaseTypeGroups = new List<CaseTypeGroups>();
                // ListP4WViews = new List<MpSysViews>();

                await GenericErrorLog(true,e, "RefreshChapters", $"Refreshing Smartflow list: {e.Message}");
            }

            ListChapterLoaded = true;

            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }

        private async Task RefreshSelectedChapters()
        {
                       
            ListChapterLoaded = false;

            bool gotLock = CompanyDbAccess.Lock;
            while (gotLock)
            {
                await Task.Yield();
                gotLock = CompanyDbAccess.Lock;
            }

            try
            {
                LstSelectedChapters = LstChapters.Where(C => C.SmartflowObject.CaseType == SelectedChapter.CaseType)
                                        .Where(C => C.SmartflowObject.CaseTypeGroup == SelectedChapter.CaseTypeGroup)
                                        .OrderBy(C => C.SmartflowObject.SeqNo)
                                        .ToList();

                await ReSequenceSmartFlows(); //makes sure the sequence numbers are all sequential, corrects any issues
                await RefreshChapterIssues();

            }
            catch(Exception e)
            {
                await GenericErrorLog(true,e, "RefreshChapters", $"Refreshing Smartflow list: {e.Message}");
            }

            ListChapterLoaded = true;

            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }

        private async Task<bool> RefreshChapterIssues()
        {
            try
            {
                
                if(!(LstSelectedChapters is null) && LstSelectedChapters.Count > 0)
                {
                    foreach (var chapter in LstSelectedChapters)
                    {
                        chapter.ComparisonList.Clear();

                        //check for duplicate Smartflow names
                        var numDuplicates = LstSelectedChapters
                                            .Where(A => A.SmartflowObject.SmartflowName == chapter.SmartflowObject.SmartflowName)
                                            .Where(A => A.SmartflowObject.CaseType == chapter.SmartflowObject.CaseType)
                                            .Where(A => A.SmartflowObject.CaseTypeGroup == chapter.SmartflowObject.CaseTypeGroup)
                                            .Where(A => A.SmartflowObject.SeqNo < chapter.SmartflowObject.SeqNo)
                                            .Count();
                                            
                        if (numDuplicates > 0)
                        {
                            chapter.ComparisonList.Add("Duplicate name");
                            chapter.ComparisonResult = "Duplicate name";
                            chapter.ComparisonIcon = "exclamation";
                        }
                            
                            
                        
                    }
                }
                
            }
            catch(Exception e)
            {
                await GenericErrorLog(true,e, "RefreshChapterIssues", $"Checking Smartflows for basic issues from current listing: {e.Message}");

                return false;
            }

            return true;
        }


        public async Task SelectCaseTypeGroup(string caseTypeGroup)
        {
            try
            {
                SelectedChapter.CaseTypeGroup = (SelectedChapter.CaseTypeGroup == caseTypeGroup) ? "" : caseTypeGroup;
                SelectedChapter.CaseType = "";
                SelectedChapter.Name = "";
            }
            catch (Exception e)
            {
                await GenericErrorLog(true,e, "SelectCaseTypeGroup", $"Selecting Case Type Group: {e.Message}");
                
            }
        }

        public async Task SelectCaseType(string caseType)
        {
            try
            {
                SelectedChapter.CaseType = (SelectedChapter.CaseType == caseType) ? "" : caseType; 
                
                LstSelectedChapters = LstChapters.Where(C => C.SmartflowObject.CaseType == SelectedChapter.CaseType)
                                        .Where(C => C.SmartflowObject.CaseTypeGroup == SelectedChapter.CaseTypeGroup)
                                        .OrderBy(C => C.SmartflowObject.SeqNo)
                                        .ToList(); 
                

                SelectedChapter.Name = "";

                if(LstSelectedChapters.Count > 0)
                {
                    await ReSequenceSmartFlows();
                    
                    if(SmartflowComparison)
                    {
                        CompareSelectedChapters();
                    }
                    else
                    {
                        await RefreshChapterIssues();
                    }
                }
            }
            catch (Exception e)
            {
                await GenericErrorLog(false,e, "SelectCaseType", e.Message);
            }

        }


        private async void SelectChapter(UsrOrsfSmartflows chapter)
        {
            try
            {

                ScrollPosition = await JSRuntime.InvokeAsync<float>("getElementPosition");

                SelectedChapterObject.SmartflowObject = chapter;

                SelectedChapter.Name = chapter.SmartflowName;

            }
            catch (Exception ex)
            {

                await GenericErrorLog(true,ex, "SelectChapter", $"Loading selected Smartflow: {ex.Message}");

            }
            finally
            {
                StateHasChanged();
            }


        }

        

        private void PrepareChapterForInsert()
        {
            try
            {

                EditChapterObject = new UsrOrsfSmartflows();

                if (!(SelectedChapter.CaseTypeGroup == ""))
                {
                    EditChapterObject.CaseTypeGroup = SelectedChapter.CaseTypeGroup;
                }
                else
                {
                    EditChapterObject.CaseTypeGroup = "";
                }

                if (!(SelectedChapter.CaseType == ""))
                {
                    EditChapterObject.CaseType = SelectedChapter.CaseType;
                }
                else
                {
                    EditChapterObject.CaseType = "";
                }

                if (!string.IsNullOrWhiteSpace(SelectedChapter.CaseTypeGroup) & !string.IsNullOrWhiteSpace(SelectedChapter.CaseType))
                {
                    EditChapterObject.SeqNo = LstSelectedChapters
                                                    .OrderByDescending(C => C.SmartflowObject.SeqNo)
                                                    .Select(C => C.SmartflowObject.SeqNo)
                                                    .FirstOrDefault() + 1;
                }
                else
                {
                    EditChapterObject.SeqNo = 1;
                }

                ShowChapterAddOrEditModel();

            }
            catch(Exception e)
            {
                GenericErrorLog(false, e, "PrepareChapterForInsert", e.Message);
            }
        }

        private void PrepareCaseTypeForEdit(string caseType, string option)
        {
            EditCaseType = caseType;
            IsCaseTypeOrGroup = option;

            ShowCaseTypeEditModal();
        }

        private void PrepareChapterForEdit(UsrOrsfSmartflows chapter, string option)
        {
            EditChapter = chapter;
            IsCaseTypeOrGroup = option;

            ShowCaseTypeEditModal();
        }



#endregion




#region Modals

        protected async Task ShowCaseTypeEditModal()
        {
            try
            {
                Action action = RefreshChapters;

                var parameters = new ModalParameters();
                parameters.Add("TaskObject", (IsCaseTypeOrGroup == "Chapter") ? EditChapter.SmartflowData : EditCaseType);
                parameters.Add("originalName", (IsCaseTypeOrGroup == "Chapter") ? EditChapter.SmartflowData : EditCaseType);
                if (IsCaseTypeOrGroup == "Chapter")
                {
                    parameters.Add("Chapter", EditChapter);
                }
                parameters.Add("DataChanged", action);
                parameters.Add("IsCaseTypeOrGroup", IsCaseTypeOrGroup);
                parameters.Add("caseTypeGroupName", SelectedChapter.CaseTypeGroup);
                parameters.Add("ListChapters", LstChapters);
                parameters.Add("CompanyDbAccess", CompanyDbAccess);
                parameters.Add("UserSession", UserSession);

                var options = new ModalOptions()
                {
                    Class = "blazored-custom-modal modal-chapter-casetype"
                };

                Modal.Show<ModalChapterCaseTypeEdit>("Smartflow", parameters, options);
            }
            catch(Exception e)
            {
                await GenericErrorLog(false,e, "ShowCaseTypeEditModal", e.Message);
            }
        }

        protected void ShowChapterAddOrEditModel()
        {
            Action action = RefreshChapters;

            var parameters = new ModalParameters();
            parameters.Add("TaskObject", EditChapterObject);
            parameters.Add("DataChanged", action);
            parameters.Add("AllObjects", LstChapters);
            parameters.Add("UserSession", UserSession);
            parameters.Add("CompanyDbAccess", CompanyDbAccess);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-chapter-chapter"
            };

            Modal.Show<ModalChapterAddOrEdit>("Smartflow", parameters, options);
        }



        protected void ShowChapterDeleteModal(VmUsrOrsfSmartflows SelectedChapterItem)
        {
            EditChapter = SelectedChapterItem.SmartflowObject;

            string itemName = SelectedChapterItem.SmartflowObject.SmartflowName;

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

        private async void HandleChapterDelete()
        {
            await ChapterManagementService.Delete(EditChapter.Id);

            var chapterToRemove = LstSelectedChapters.Where(S => S.SmartflowObject.Id == EditChapter.Id).First();

            LstSelectedChapters.Remove(chapterToRemove);
            LstChapters.Remove(chapterToRemove);
            
            StateHasChanged();

            await ReSequenceSmartFlows();
        }


#endregion

#region Chapter Comparrisons


        /// <summary>
        /// Triggered from the checkbox at top of the screen to Sync Smartflows via the bound property SmartflowComparison
        /// </summary>
        /// <returns></returns>
        private async void CompareSelectedChapters()
        {
            try
            {
                LstSelectedChapters.Select(C => { C.ComparisonIcon = null; C.ComparisonResult = null; return C; }).ToList();

                LstAltSystemChapters = new List<VmUsrOrsfSmartflows>();
                AltChapterObject.SmartflowObject = new UsrOrsfSmartflows();
                await RefreshCompararisonSelectedChapters();
            }
            catch (Exception e)
            {
                await GenericErrorLog(false,e, "CompareSelectedChapters", $"{e.Message}");
            }

            await InvokeAsync(() =>
            {
                StateHasChanged();
            });

        }

        private async Task<bool> RefreshCompararisonSelectedChapters()
        {
            try
            {
                if (CompareSystems)
                {
                    await RefreshAltSystemChaptersList();

                    /*
                    * for every chapter get list of chapter items from both current system and alt system
                    * if any result returns false
                    * 
                    * 
                    */
                    if(!(LstAltSystemChapters is null) && LstAltSystemChapters.Count > 0)
                    {
                        foreach (var chapter in LstSelectedChapters)
                        {
                            var chapterItems = JsonConvert.DeserializeObject<VmChapter>(chapter.SmartflowObject.SmartflowData);

                            AltChapterObject.SmartflowObject = LstAltSystemChapters
                                                .Where(A => A.SmartflowObject.SmartflowName == chapter.SmartflowObject.SmartflowName)
                                                .Where(A => A.SmartflowObject.CaseType == chapter.SmartflowObject.CaseType)
                                                .Where(A => A.SmartflowObject.CaseTypeGroup == chapter.SmartflowObject.CaseTypeGroup)
                                                .Select(C => C.SmartflowObject)
                                                .FirstOrDefault(); //get the first just in case there are 2 Smartflows with same name

                            if (AltChapterObject.SmartflowObject is null)
                            {
                                //No corresponding Smartflow on the Alt system
                                chapter.ComparisonResult = "No match";
                                chapter.ComparisonIcon = "times";
                            }
                            else
                            {

                                if(AltChapterObject.SmartflowObject.SmartflowData == chapter.SmartflowObject.SmartflowData)
                                {
                                    chapter.ComparisonResult = "Exact match";
                                    chapter.ComparisonIcon = "check";
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
                        LstSelectedChapters.Select(T => { T.ComparisonIcon = "times"; T.ComparisonResult = "No match"; return T; }).ToList();
                    }

                
                }
            }
            catch(Exception e)
            {
                await GenericErrorLog(true,e, "RefreshCompararisonSelectedChapters", $"Comparing all Smartflows from current listing: {e.Message}");

                return false;
            }

            return true;
        }



        protected async Task ShowChapterSyncOnAltModal(UsrOrsfSmartflows SelectedChapter)
        {
            SelectedChapterObject.SmartflowObject = SelectedChapter;

            string infoText = $"Do you wish to sync this smartflow to {(UserSession.SelectedSystem == "Live" ? "Dev" : "Live")}.";

            Action SelectedAction = SyncSelectedChapterOnAlt;
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

        private async void SyncSelectedChapterOnAlt()
        {
            try
            {
                await UserSession.SwitchSelectedSystem();

                AltChapterObject.SmartflowObject = LstAltSystemChapters
                                            .Where(A => A.SmartflowObject.SmartflowName == SelectedChapterObject.SmartflowObject.SmartflowName)
                                            .Where(A => A.SmartflowObject.CaseType == SelectedChapterObject.SmartflowObject.CaseType)
                                            .Where(A => A.SmartflowObject.CaseTypeGroup == SelectedChapterObject.SmartflowObject.CaseTypeGroup)
                                            .Select(C => C.SmartflowObject)
                                            .SingleOrDefault();

                if (AltChapterObject.SmartflowObject is null)
                {
                    var newAltChapterObject = new UsrOrsfSmartflows
                    {
                        SeqNo = SelectedChapterObject.SmartflowObject.SeqNo,
                        CaseTypeGroup = SelectedChapterObject.SmartflowObject.CaseTypeGroup,
                        CaseType = SelectedChapterObject.SmartflowObject.CaseType,
                        SmartflowName = SelectedChapterObject.SmartflowObject.SmartflowName,
                        SmartflowData = SelectedChapterObject.SmartflowObject.SmartflowData,
                        VariantName = SelectedChapterObject.SmartflowObject.VariantName,
                        VariantNo = SelectedChapterObject.SmartflowObject.VariantNo
                    };

                    var returnObject = await ChapterManagementService.Add(newAltChapterObject);
                    newAltChapterObject.Id = returnObject.Id;

                    bool gotLock = CompanyDbAccess.Lock;
                    while (gotLock)
                    {
                        await Task.Yield();
                        gotLock = CompanyDbAccess.Lock;
                    }

                    await CompanyDbAccess.SaveSmartFlowRecord(newAltChapterObject, UserSession);
                }
                else
                {
                    AltChapterObject.SmartflowObject.SmartflowData = SelectedChapterObject.SmartflowObject.SmartflowData;

                    await ChapterManagementService.Update(AltChapterObject.SmartflowObject);
                }


                
                await UserSession.ResetSelectedSystem();

                await RefreshCompararisonSelectedChapters();

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
                await GenericErrorLog(false,e, "CreateSelectedChapterOnAlt", e.Message);
            }

        }


        private async Task<bool> RefreshAltSystemChaptersList()
        {
            try
            {
                await UserSession.SwitchSelectedSystem();

                bool gotLock = CompanyDbAccess.Lock;
                while (gotLock)
                {
                    await Task.Yield();
                    gotLock = CompanyDbAccess.Lock;
                }

                var LstAllChapterItemsChapters = await CompanyDbAccess.GetAllSmartflowRecords(UserSession);

                if(!(LstAllChapterItemsChapters is null))
                {
                    LstAltSystemChapters = LstAllChapterItemsChapters.Select(A => new VmUsrOrsfSmartflows { SmartflowObject = Mapper.Map(A, new UsrOrsfSmartflows()) }).ToList();
                }
                
                await UserSession.ResetSelectedSystem();

                return true;
            }
            catch (Exception e)
            {
                await GenericErrorLog(false,e, "RefreshAltSystemChaptersList", $"{e.Message}");

                return false;
            }
        }



        

#endregion



#region Sync Systems
        
       
        public async void UploadSmartFlowsFromClient()
        {
            try
            {
                var lstC = await ChapterManagementService.GetAllChapters();

                await CompanyDbAccess.SyncAdminSysToClient(lstC, UserSession);

                RefreshChapters();

                var parameters = new ModalParameters();
                parameters.Add("InfoText", "Systems synced successfully");

                var options = new ModalOptions()
                {
                    Class = "blazored-custom-modal modal-chapter-chapter"
                };
                string title = $"Systems Synced";
                Modal.Show<ModalInfo>(title, parameters, options);
            }
            catch(HttpRequestException)
            {
                //do nothing, already dealt with
            }
            catch(Exception e)
            {
                await GenericErrorLog(true,e, "SyncSmartFlowSystems", e.Message);
            }
                
            
        }

        /// <summary>
        /// Update all P4W steps for the current system
        /// </summary>
        /// <returns></returns>
        private async Task UpdateSteps()
        {
            bool overAllCreationSuccess = true;
            bool creationSuccess = true;
            int creationCount = 0;
            string stepJSON = "";

            try
            {
                ChapterP4WStepSchema chapterP4WStep;

                foreach (var chapter in LstChapters)
                {

                    var decodedChapter = JsonConvert.DeserializeObject<VmChapter>(chapter.SmartflowObject.SmartflowData);

                    if (!string.IsNullOrEmpty(decodedChapter.SelectedStep) 
                        && !string.IsNullOrEmpty(decodedChapter.SelectedView)
                        && !string.IsNullOrEmpty(decodedChapter.Name)
                        && !string.IsNullOrEmpty(decodedChapter.StepName)
                        && !string.IsNullOrEmpty(decodedChapter.P4WCaseTypeGroup)
                        && !string.IsNullOrEmpty(decodedChapter.CaseTypeGroup))
                    {
                        if (decodedChapter.P4WCaseTypeGroup == "Entity Documents")
                        {
                            chapterP4WStep = new ChapterP4WStepSchema
                            {
                                StepName = decodedChapter.StepName,
                                P4WCaseTypeGroup = decodedChapter.P4WCaseTypeGroup,
                                GadjITCaseTypeGroup = decodedChapter.CaseTypeGroup,
                                GadjITCaseType = decodedChapter.CaseType,
                                Smartflow = decodedChapter.Name,
                                SFVersion = Configuration["AppSettings:Version"],
                                Questions = new List<ChapterP4WStepQuestion>{
                                        new ChapterP4WStepQuestion {QNo = 1, QText= "HQ - Set Current Chapter Details" }
                                        ,new ChapterP4WStepQuestion {QNo = 2, QText= "HQ - Show View" }
                                        ,new ChapterP4WStepQuestion {QNo = 3, QText= "HQ - Run Required Steps" }
                                        ,new ChapterP4WStepQuestion {QNo = 4, QText= "HQ - Update reschedule date" }
                                        ,new ChapterP4WStepQuestion {QNo = 5, QText= "HQ - Reinsert this step with asname" }
                                        ,new ChapterP4WStepQuestion {QNo = 6, QText= "HQ - Rename Step" }
                                        ,new ChapterP4WStepQuestion {QNo = 7, QText= "HQ - Delete Previous Instances if Suppressed Step" }
                                        ,new ChapterP4WStepQuestion {QNo = 8, QText= "HQ - Check if completion name exists" }
                                        ,new ChapterP4WStepQuestion {QNo = 9, QText= "HQ - Delete Step" }
                                        },
                                Answers = new List<ChapterP4WStepAnswer>{
                                        new ChapterP4WStepAnswer {QNo = 1, GoToData= $"2 [SQL: EXEC Up_ORSF_MoveDocsToAgendas '[entity.code]', 0] [SQL: EXEC up_ORSF_CreateTableEntries '[entity.code]', 0] [SQL: UPDATE Usr_ORSF_ENT_Control SET Current_SF = '{decodedChapter.Name}', Current_Case_Type_Group = '{decodedChapter.CaseTypeGroup}', Current_Case_Type = '{decodedChapter.CaseType}', Default_Step = '{decodedChapter.StepName}', Date_Schedule_For = DATEADD(d, 7, getdate()), Steps_To_Run = '', Schedule_AsName = '{decodedChapter.StepName}|' + (SELECT CASE WHEN dbo.fn_ORSF_IsAllCap (Description) = 1 THEN '{decodedChapter.StepName}' + '|' + CONVERT(VARCHAR(20),ISNULL(Date_Schedule_For,DATEADD(d, 7, getdate())),103) ELSE Description + '|' + CONVERT(VARCHAR(20),ISNULL(s.DiaryDate, getdate()),103) END + '|' + '[CurrentUser.Code]' FROM Cm_CaseItems i INNER JOIN Cm_Steps s on s.ItemID = i.ItemID WHERE i.ItemID = [currentstep.stepid]), Complete_AsName = '' WHERE EntityRef = '[Entity.Code]'] [SQL: UPDATE Usr_ORSF_ENT_Control SET Screen_Opened_Via_Step = 'Y' WHERE EntityRef = '[Entity.Code]' ]" }
                                        ,new ChapterP4WStepAnswer {QNo = 2, GoToData= $"7 [VIEW: '{decodedChapter.SelectedView}' UPDATE=Yes]" }
                                        ,new ChapterP4WStepAnswer {QNo = 3, GoToData= $"4 [TAKE: 'SF-Admin Save Items for Agenda Management' INSERT=END]  [SQL: EXEC up_ORSF_GetStepsFromList '[entity.code]', 0] [SQL: UPDATE Usr_ORSF_ENT_Control SET Screen_Opened_Via_Step = null WHERE EntityRef='[entity.code]']" }
                                        ,new ChapterP4WStepAnswer {QNo = 4, GoToData= $"5 [SQL: UPDATE Usr_ORSF_ENT_Control SET Date_Schedule_For = isnull(Date_Schedule_For, Cast(getdate() as Date)) WHERE EntityRef = '[Entity.Code]']" }
                                        ,new ChapterP4WStepAnswer {QNo = 5, GoToData= $"8 [SQL: SELECT ScheduleCommand FROM fn_ORSF_GetScheduleItems('[Entity.Code]', 0)]" }
                                        ,new ChapterP4WStepAnswer {QNo = 6, GoToData= $"[SQL: UPDATE cm_caseitems set CompletionDate = GETDATE(), description = UPPER('[!Usr_ORSF_ENT_Control.Complete_AsName]') where itemid = [currentstep.stepid]] [SQL: exec up_ORSF_CompleteStep [currentStep.stepid], '', 'Y']" }
                                        ,new ChapterP4WStepAnswer {QNo = 7, GoToData= $"3 [[SQL: SELECT CASE WHEN ISNULL(Do_Not_Reschedule,'N') = 'Y' THEN 'SQL: exec up_ORSF_DeleteDueStep '''', [currentstep.stepid], ''{decodedChapter.Name}''' ELSE '' END FROM Usr_ORSF_MT_Control WHERE EntityRef = '[matters.entityref]' AND MatterNo = [matters.number]]]" }
                                        ,new ChapterP4WStepAnswer {QNo = 8, GoToData= $"[SQL: SELECT CASE WHEN ISNULL('[!Usr_ORSF_ENT_Control.Complete_AsName]','') <> '' THEN 6 ELSE 9 END]" }
                                        ,new ChapterP4WStepAnswer {QNo = 9, GoToData= $"[SQL: exec up_ORSF_DeleteStep [currentstep.stepid]]" }
                                        }
                            };
                        }
                        else
                        {
                            chapterP4WStep = new ChapterP4WStepSchema
                            {
                                StepName = decodedChapter.StepName,
                                P4WCaseTypeGroup = decodedChapter.P4WCaseTypeGroup,
                                GadjITCaseTypeGroup = decodedChapter.CaseTypeGroup,
                                GadjITCaseType = decodedChapter.CaseType,
                                Smartflow = decodedChapter.Name,
                                SFVersion = Configuration["AppSettings:Version"],
                                Questions = new List<ChapterP4WStepQuestion>{
                                        new ChapterP4WStepQuestion {QNo = 1, QText= "HQ - Set Current Chapter Details" }
                                        ,new ChapterP4WStepQuestion {QNo = 2, QText= "HQ - Show View" }
                                        ,new ChapterP4WStepQuestion {QNo = 3, QText= "HQ - Run Required Steps" }
                                        ,new ChapterP4WStepQuestion {QNo = 4, QText= "HQ - Update reschedule date" }
                                        ,new ChapterP4WStepQuestion {QNo = 5, QText= "HQ - Reinsert this step with asname" }
                                        ,new ChapterP4WStepQuestion {QNo = 6, QText= "HQ - Rename Step" }
                                        ,new ChapterP4WStepQuestion {QNo = 7, QText= "HQ - Delete Previous Instances if Suppressed Step" }
                                        ,new ChapterP4WStepQuestion {QNo = 8, QText= "HQ - Check if completion name exists" }
                                        ,new ChapterP4WStepQuestion {QNo = 9, QText= "HQ - Delete Step" }
                                        },
                                Answers = new List<ChapterP4WStepAnswer>{
                                        new ChapterP4WStepAnswer {QNo = 1, GoToData= $"2 [SQL: EXEC Up_ORSF_MoveDocsToAgendas '[matters.entityref]', [matters.number]] [SQL: EXEC up_ORSF_CreateTableEntries '[matters.entityref]', [matters.number]] [SQL: UPDATE Usr_ORSF_MT_Control SET Current_SF = '{decodedChapter.Name}', Current_Case_Type_Group = '{decodedChapter.CaseTypeGroup}', Current_Case_Type = '{decodedChapter.CaseType}', Default_Step = '{decodedChapter.StepName}', Date_Schedule_For = DATEADD(d, 7, getdate()), Steps_To_Run = '', Schedule_AsName = '{decodedChapter.StepName}|' + (SELECT CASE WHEN dbo.fn_ORSF_IsAllCap (Description) = 1 THEN '{decodedChapter.StepName}' + '|' + CONVERT(VARCHAR(20),ISNULL(Date_Schedule_For,DATEADD(d, 7, getdate())),103) ELSE Description + '|' + CONVERT(VARCHAR(20),ISNULL(s.DiaryDate, getdate()),103) END + '|' + '[matters.feeearnerref]' FROM Cm_CaseItems i INNER JOIN Cm_Steps s on s.ItemID = i.ItemID WHERE i.ItemID = [currentstep.stepid]), Complete_AsName = '' WHERE EntityRef = '[matters.entityref]' AND MatterNo = [matters.number]] [SQL: UPDATE Usr_ORSF_MT_Control SET Screen_Opened_Via_Step = 'Y' WHERE EntityRef = '[matters.entityref]' AND matterNo =[matters.number]]" }
                                        ,new ChapterP4WStepAnswer {QNo = 2, GoToData= $"7 [VIEW: '{decodedChapter.SelectedView}' UPDATE=Yes]" }
                                        ,new ChapterP4WStepAnswer {QNo = 3, GoToData= $"4 [TAKE: 'SF-Admin Save Items for Agenda Management' INSERT=END]  [SQL: EXEC up_ORSF_GetStepsFromList '[matters.entityref]', [matters.number]] [SQL: UPDATE Usr_ORSF_MT_Control SET Screen_Opened_Via_Step = null WHERE EntityRef='[matters.entityref]' AND matterNo=[matters.number]]" }
                                        ,new ChapterP4WStepAnswer {QNo = 4, GoToData= $"5 [SQL: UPDATE Usr_ORSF_MT_Control SET Date_Schedule_For = isnull(Date_Schedule_For, Cast(getdate() as Date)) WHERE EntityRef = '[matters.entityref]' AND MatterNo = [matters.number]]" }
                                        ,new ChapterP4WStepAnswer {QNo = 5, GoToData= $"8 [SQL: SELECT ScheduleCommand FROM fn_ORSF_GetScheduleItems('[matters.entityref]', [matters.number])]" }
                                        ,new ChapterP4WStepAnswer {QNo = 6, GoToData= $"[SQL: UPDATE cm_caseitems set CompletionDate = GETDATE(), description = UPPER('[!Usr_ORSF_MT_Control.Complete_AsName]') where itemid = [currentstep.stepid]] [SQL: exec up_ORSF_CompleteStep [currentStep.stepid], '', 'Y']" }
                                        ,new ChapterP4WStepAnswer {QNo = 7, GoToData= $"3 [[SQL: SELECT CASE WHEN ISNULL(Do_Not_Reschedule,'N') = 'Y' THEN 'SQL: exec up_ORSF_DeleteDueStep '''', [currentstep.stepid], ''{decodedChapter.Name}''' ELSE '' END FROM Usr_ORSF_MT_Control WHERE EntityRef = '[matters.entityref]' AND MatterNo = [matters.number]]]" }
                                        ,new ChapterP4WStepAnswer {QNo = 8, GoToData= $"[SQL: SELECT CASE WHEN ISNULL('[!Usr_ORSF_MT_Control.Complete_AsName]','') <> '' THEN 6 ELSE 9 END]" }
                                        ,new ChapterP4WStepAnswer {QNo = 9, GoToData= $"[SQL: exec up_ORSF_DeleteStep [currentstep.stepid]]" }
                                        }
                            };
                        }

                        stepJSON = JsonConvert.SerializeObject(chapterP4WStep);

                        

                        creationSuccess = await ChapterManagementService.CreateStep(new VmChapterP4WStepSchemaJSONObject { StepSchemaJSON = stepJSON });

                        if (!creationSuccess)
                        {
                            overAllCreationSuccess = false;
                        }

                        creationCount += 1;

                    }

                }

                //Refresh the global step that saves "Completed" docs into the relevant Agendas
                //This step will be called by each Smartflow
                chapterP4WStep = new ChapterP4WStepSchema
                {
                    StepName = "SF-Admin Save Items for Agenda Management",
                    P4WCaseTypeGroup = "Global Documents",
                    GadjITCaseTypeGroup = "Global",
                    GadjITCaseType = "Global Documents",
                    Smartflow = "Admin Save Items for Agenda Management",
                    SFVersion = Configuration["AppSettings:Version"],
                    Questions = new List<ChapterP4WStepQuestion>{
                            new ChapterP4WStepQuestion {QNo = 1, QText= "HQ - Save Items for Agenda Management" }
                            ,new ChapterP4WStepQuestion {QNo = 2, QText= "HQ - Delete Me" }
                            },
                    Answers = new List<ChapterP4WStepAnswer>{
                            new ChapterP4WStepAnswer {QNo = 1, GoToData= $"2 [SQL: exec up_ORSF_Agenda_Control '[matters.entityref]', [matters.number], [currentstep.stepid] ]" }
                            ,new ChapterP4WStepAnswer {QNo = 2, GoToData= $"[SQL: EXEC dbo.up_ORSF_DeleteStep [currentstep.stepid]]" }
                            }
                };

                stepJSON = JsonConvert.SerializeObject(chapterP4WStep);

                creationSuccess = await ChapterManagementService.CreateStep(new VmChapterP4WStepSchemaJSONObject { StepSchemaJSON = stepJSON });

                if (!creationSuccess)
                {
                    overAllCreationSuccess = false;
                }

                var parameters = new ModalParameters();
                var message = overAllCreationSuccess ? "Creation Successfull" : "Creation Unsuccessfull";
                parameters.Add("InfoText", message);

                var options = new ModalOptions()
                {
                    Class = "blazored-custom-modal modal-chapter-chapter"
                };
                string title = $"Updated {creationCount} Steps";
                Modal.Show<ModalInfo>(title, parameters, options);
            }
            catch (Exception e)
            {
                await GenericErrorLog(true,e, "UpdateSteps", $"Creating/Updating all P4W Smartflow steps: {e.Message}");

            }

        }


#endregion



#region Error Handling


        /****************************************/
        /* ERROR HANDLING */
        /****************************************/
        private async Task GenericErrorLog(bool showNotificationMsg, Exception e, string _method, string _message)
        {
            using (LogContext.PushProperty("SourceSystem", UserSession.SelectedSystem))
            using (LogContext.PushProperty("SourceCompanyId", UserSession.Company.Id))
            using (LogContext.PushProperty("SourceUserId", UserSession.User.Id))
            using (LogContext.PushProperty("SourceContext", nameof(ChapterList)))
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
