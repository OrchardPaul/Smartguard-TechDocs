using AutoMapper;
using Blazored.Modal;
using Blazored.Modal.Services;
using GadjIT_App.Services;
using GadjIT_App.Services.SessionState;
using GadjIT_App.Pages._Shared.Modals;
using GadjIT_App.Data.Admin;
using GadjIT_App.Services.AppState;
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
using System.Threading;
using System.Threading.Tasks;
using GadjIT_App.Pages.Smartflows.ComponentsSmartflowList;
using GadjIT_ClientContext.Models.Smartflow;
using GadjIT_ClientContext.Models.Smartflow.Client;
using GadjIT_ClientContext.Models.P4W;

namespace GadjIT_App.Pages.Smartflows
{
    public partial class SmartflowList
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
        private IClientApiManagementService ClientApiManagementService { get; set; }

        [Inject]
        public IUserSessionState UserSession { get; set; }

        [Inject]
        public IAppSmartflowsState AppSmartflowsState { get; set; }

        [Inject]
        private ICompanyDbAccess CompanyDbAccess { get; set; }

        [Inject]
        private ILogger<SmartflowList> Logger { get; set; }


        [Inject]
        private IPageAuthorisationState pageAuthorisationState { get; set; }

        [Inject]
        public IConfiguration Configuration { get; set;}

        private List<Client_VmSmartflowRecord> LstAll_VmClientSmartflowRecord { get; set; } = new List<Client_VmSmartflowRecord>();

        private List<Client_VmSmartflowRecord> LstAlt_VmClientSmartflowRecord { get; set; } = new List<Client_VmSmartflowRecord>();


        public List<Client_VmSmartflowRecord> LstSelected_VmClientSmartflowRecord { get; set; } = new List<Client_VmSmartflowRecord>();

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
                    CompareSelectedSmartflows();
                }
                else{
                    StateHasChanged();
                }
            }
        }



        public String UserGuideURL => Configuration["AppSettings:UserGuideURL"];
        

        public string IsCaseTypeOrGroup { get; set; } = "";

        public Client_SmartflowRecord Edit_ClientSmartflowRecord = new Client_SmartflowRecord ();



        [Parameter]
        public Client_VmSmartflowRecord Selected_VmClientSmartflowRecord { get; set; } = new Client_VmSmartflowRecord(); //full object
        public Client_VmSmartflowRecord Alt_VmClientSmartflowRecord { get; set; } = new Client_VmSmartflowRecord();


        public Smartflow SelectedSmartflow { get; set; } = new Smartflow { Items = new List<GenSmartflowItem>() }; //SmartflowData

        protected string SelectedCaseTypeGroup { get; set;} = "";
        protected string SelectedCaseType { get; set;} = "";
        protected string SelectedCaseTypeGroupPrev { get; set;} = "";
        protected string SelectedCaseTypePrev { get; set;} = "";


        int RowChanged { get; set; } = 0; 

        private bool SeqMoving = false;
        private string RowChangedClass { get; set; } = "row-changed-nav3";

        protected bool CompareSystems = false;

       
        public bool ListSmartflowIsLoaded = false;

        

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
                    await RefreshSmartflowsTask();

                    //P4WCaseTypeGroups = await PartnerAccessService.GetPartnerCaseTypeGroups();
                    //ListP4WViews = await PartnerAccessService.GetPartnerViews();
                    UserSession.HomeActionSmartflow = SelectHome;

                }
                catch (Exception e)
                {
                    //Note: do not show notification as JsRuntime is not available until After Render
                    GenericErrorLog(false,e, "OnInitializedAsync", $"Loading initial Smartflow list: {e.Message}");
                    
                }
                
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

        


        public bool IsSmartflowLocked(Client_VmSmartflowRecord _vmClientSmartflowRecord)
        {
            try
            {
                // return AppSmartflowsState.IsSmartflowLocked(
                //                                 UserSession.Company.Id
                //                                 , UserSession.SelectedSystem
                //                                 , _vmClientSmartflowRecord.ClientSmartflowRecord.CaseTypeGroup
                //                                 , _vmClientSmartflowRecord.ClientSmartflowRecord.CaseType
                //                                 , _vmClientSmartflowRecord.ClientSmartflowRecord.SmartflowName
                //                              );
                return AppSmartflowsState.IsSmartflowLocked(_vmClientSmartflowRecord.ClientSmartflowRecord.Id);

            }
            catch(Exception)
            {
                return false;
            }

        }

#endregion

#region Navigation and Drag Drop and Resequence List


        public async void SelectHome()
        {
            try
            {
                //NavigationManager.NavigateTo($"Smartflow/{SelectedSmartflow.CaseTypeGroup}/{SelectedSmartflow.CaseType}",true);
                if (!string.IsNullOrEmpty(UserSession.TempBackGroundImage))
                {
                    UserSession.TempBackGroundImage = "";
                    UserSession.RefreshHome?.Invoke();
                }
                CompareSystems = false;
                Selected_VmClientSmartflowRecord.ClientSmartflowRecord = null;
                SelectedSmartflow.Name = "";
                if(SelectedCaseTypeGroup == "" && SelectedCaseTypeGroupPrev != "")
                {
                    SelectedCaseTypeGroup = SelectedCaseTypeGroupPrev;
                    SelectedCaseType = SelectedCaseTypePrev;
                }
                else
                {
                    SelectedCaseTypeGroup = "";
                    SelectedCaseType = "";
                }
                

                ResetRowChanged();

                StateHasChanged();


                await MovePos().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                GenericErrorLog(false,e, "SelectHome", e.Message);
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
                GenericErrorLog(false,e, "MovePos", e.Message);
            }
        }       
        
        public void ResetRowChanged() 
        {
            RowChanged = 0;
        }

        // Timer timerRowChanged;

        // public void ResetRowChangedHandler(object sender, ElapsedEventArgs eventArgs) 
        // {
        //     RowChanged = 0;
        // }

        // private async void ResetRowChangedDelayed() 
        // {
        //     timerRowChanged = new Timer();
        //     timerRowChanged.Interval = 1300; //1.3 seconds
        //     timerRowChanged.Elapsed += ResetRowChangedHandler;
        //     timerRowChanged.AutoReset = false;
        //     // Start the timer
        //     timerRowChanged.Enabled = true;
        // }
        
    
        public async Task ReSequenceSmartFlows(int seq)
        {
            RowChanged = seq;
            await ReSequenceSmartFlows();

            // ResetRowChangedDelayed();
        }

        public async Task ReSequenceSmartFlows()
        {
            try
            {
                if(LstSelected_VmClientSmartflowRecord.Select(C => C.ClientSmartflowRecord.SeqNo != LstSelected_VmClientSmartflowRecord.IndexOf(C) + 1).Count() > 0) //If any SeqNos are out of sequence
                { 
                    LstSelected_VmClientSmartflowRecord.Select(C => { C.ClientSmartflowRecord.SeqNo = LstSelected_VmClientSmartflowRecord.IndexOf(C) + 1; return C; }).ToList(); //update all SeqNo by setting to the Index (+1)

                    foreach (var smartflowToChange in LstSelected_VmClientSmartflowRecord)
                    {
                        await ClientApiManagementService.UpdateMainItem(smartflowToChange.ClientSmartflowRecord).ConfigureAwait(false);

                    }

                    await InvokeAsync(() =>
                    {
                        StateHasChanged();
                    });
                }

                
            }
            catch
            {
                
            }

        }

        protected async void MoveSmartFlowSeq(Client_SmartflowRecord selectobject, string listType, string direction)
        {
            await MoveSmartFlowSeqTask(selectobject, listType, direction);
        }

        protected async Task MoveSmartFlowSeqTask(Client_SmartflowRecord selectobject, string listType, string direction)
        {
            try
            {
                SeqMoving = true; //prevents changes to the form whilst process of changing seq is carried out

                var lstItems = new List<Client_VmSmartflowRecord>();
                int incrementBy;

                incrementBy = (direction.ToLower() == "up" ? -1 : 1);

                RowChanged = (int)(selectobject.SeqNo + incrementBy);

                lstItems = LstSelected_VmClientSmartflowRecord
                            .OrderBy(A => A.ClientSmartflowRecord.SeqNo)
                            .ToList();


                var swapItem = lstItems.Where(D => D.ClientSmartflowRecord.SeqNo == (selectobject.SeqNo + incrementBy)).SingleOrDefault();
                if (!(swapItem is null))
                {
                    selectobject.SeqNo += incrementBy;
                    swapItem.ClientSmartflowRecord.SeqNo = swapItem.ClientSmartflowRecord.SeqNo + (incrementBy * -1);

                    await ClientApiManagementService.UpdateMainItem(selectobject);
                    await ClientApiManagementService.UpdateMainItem(swapItem.ClientSmartflowRecord);
                }

                await RefreshSelectedSmartflows();

                
                await InvokeAsync(() =>
                {
                    StateHasChanged();
                });
                
                //ResetRowChangedDelayed();
                

            }
            catch (Exception e)
            {
                GenericErrorLog(true,e, "MoveSmartFlowSeq", $"Moving smartflow: {e.Message}");

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

#region Smartflow Listing


        private async void RefreshSmartflows()
        {
            /// <summary>
            /// Must be a standard void method so it can be assigned as an Action to Modals
            /// </summary>
            
            await RefreshSmartflowsTask();
            
        }

        public async Task RefreshSmartflowsTask()
        {
                       
            ListSmartflowIsLoaded = false;

            bool gotLock = CompanyDbAccess.Lock;
            while (gotLock)
            {
                await Task.Yield();
                gotLock = CompanyDbAccess.Lock;
            }

            try
            {
                var lstAppSmartflowRecords = await CompanyDbAccess.GetAllAppSmartflowRecords(UserSession);
                //generate a copy of the Smartflow Records but converted to Client_VmSmartflowRecord 
                LstAll_VmClientSmartflowRecord = lstAppSmartflowRecords.Select(A => new Client_VmSmartflowRecord { ClientSmartflowRecord = Mapper.Map(A, new Client_SmartflowRecord()) }).ToList();
                

                LstSelected_VmClientSmartflowRecord = LstAll_VmClientSmartflowRecord
                                        .Where(C => C.ClientSmartflowRecord.CaseTypeGroup == SelectedSmartflow.CaseTypeGroup)
                                        .Where(C => C.ClientSmartflowRecord.CaseType == SelectedSmartflow.CaseType)
                                        .OrderBy(C => C.ClientSmartflowRecord.SeqNo)
                                        .ToList();

                await ReSequenceSmartFlows(); //makes sure the sequence numbers are all sequential, corrects any issues
                await RefreshSmartflowIssues();

            }
            catch(Exception e)
            {
                LstAll_VmClientSmartflowRecord = new List<Client_VmSmartflowRecord>();
                LstSelected_VmClientSmartflowRecord = new List<Client_VmSmartflowRecord>();
                // P4WCaseTypeGroups = new List<CaseTypeGroups>();
                // ListP4WViews = new List<MpSysViews>();

                GenericErrorLog(false,e, "RefreshSmartflows", $"Refreshing Smartflow list: {e.Message}");
            }

            ListSmartflowIsLoaded = true;

            //may have been called from a void (not direct on a UI thread) following a modal so will need to call a StateHasChanged
            await InvokeAsync(() =>
            {
                StateHasChanged();
            });

            
        }

        private async Task RefreshSelectedSmartflows()
        {
                       
            ListSmartflowIsLoaded = false;

            bool gotLock = CompanyDbAccess.Lock;
            while (gotLock)
            {
                await Task.Yield();
                gotLock = CompanyDbAccess.Lock;
            }

            try
            {
                LstSelected_VmClientSmartflowRecord = LstAll_VmClientSmartflowRecord.Where(C => C.ClientSmartflowRecord.CaseType == SelectedSmartflow.CaseType)
                                        .Where(C => C.ClientSmartflowRecord.CaseTypeGroup == SelectedSmartflow.CaseTypeGroup)
                                        .OrderBy(C => C.ClientSmartflowRecord.SeqNo)
                                        .ToList();

                await ReSequenceSmartFlows(); //makes sure the sequence numbers are all sequential, corrects any issues
                await RefreshSmartflowIssues();

            }
            catch(Exception e)
            {
                GenericErrorLog(true,e, "RefreshSmartflows", $"Refreshing Smartflow list: {e.Message}");
            }

            ListSmartflowIsLoaded = true;

            // await InvokeAsync(() =>
            // {
            //     StateHasChanged();
            // });
        }

        private async Task<bool> RefreshSmartflowIssues()
        {
            try
            {
                
                if(!(LstSelected_VmClientSmartflowRecord is null) && LstSelected_VmClientSmartflowRecord.Count > 0)
                {
                    foreach (var smartflow in LstSelected_VmClientSmartflowRecord)
                    {
                        smartflow.ComparisonList.Clear();

                        //check for duplicate Smartflow names
                        var numDuplicates = LstSelected_VmClientSmartflowRecord
                                            .Where(A => A.ClientSmartflowRecord.SmartflowName == smartflow.ClientSmartflowRecord.SmartflowName)
                                            .Where(A => A.ClientSmartflowRecord.CaseType == smartflow.ClientSmartflowRecord.CaseType)
                                            .Where(A => A.ClientSmartflowRecord.CaseTypeGroup == smartflow.ClientSmartflowRecord.CaseTypeGroup)
                                            .Where(A => A.ClientSmartflowRecord.SeqNo < smartflow.ClientSmartflowRecord.SeqNo)
                                            .Count();
                                            
                        if (numDuplicates > 0)
                        {
                            smartflow.ComparisonList.Add("Duplicate name");
                            smartflow.ComparisonResult = "Duplicate name";
                            smartflow.ComparisonIcon = "exclamation";
                        }
                            
                            
                        
                    }
                }
                
            }
            catch(Exception e)
            {
                GenericErrorLog(true,e, "RefreshSmartflowIssues", $"Checking Smartflows for basic issues from current listing: {e.Message}");

                return false;
            }

            return true;
        }


        public void SelectCaseTypeGroup(string caseTypeGroup)
        {
            try
            {
                SelectedSmartflow.CaseTypeGroup = (SelectedSmartflow.CaseTypeGroup == caseTypeGroup) ? "" : caseTypeGroup;
                SelectedSmartflow.CaseType = "";
                SelectedSmartflow.Name = "";
            }
            catch (Exception e)
            {
                GenericErrorLog(true,e, "SelectCaseTypeGroup", $"Selecting Case Type Group: {e.Message}");
                
            }
        }

        public async Task SelectCaseType(string caseType)
        {
            try
            {
                SelectedSmartflow.CaseType = (SelectedSmartflow.CaseType == caseType) ? "" : caseType; 
                
                LstSelected_VmClientSmartflowRecord = LstAll_VmClientSmartflowRecord.Where(C => C.ClientSmartflowRecord.CaseType == SelectedSmartflow.CaseType)
                                        .Where(C => C.ClientSmartflowRecord.CaseTypeGroup == SelectedSmartflow.CaseTypeGroup)
                                        .OrderBy(C => C.ClientSmartflowRecord.SeqNo)
                                        .ToList(); 
                

                SelectedSmartflow.Name = "";

                if(LstSelected_VmClientSmartflowRecord.Count > 0)
                {
                    await ReSequenceSmartFlows();
                    
                    if(SmartflowComparison)
                    {
                        CompareSelectedSmartflows();
                    }
                    else
                    {
                        await RefreshSmartflowIssues();
                    }
                }
            }
            catch (Exception e)
            {
                GenericErrorLog(false,e, "SelectCaseType", e.Message);
            }

        }


        private async Task SelectSmartflowFromHome(Client_SmartflowRecord clientSmartflowRecord)
        {
            
            SelectedCaseTypeGroupPrev = "";
            SelectedCaseTypePrev = "";

            await SelectSmartflow(clientSmartflowRecord);

        }

        private async Task SelectSmartflow(Client_SmartflowRecord clientSmartflowRecord)
        {
            try
            {

                ScrollPosition = await JSRuntime.InvokeAsync<float>("getElementPosition");

                //Check if current user is already editing/viewing another Smartflow in another window/tab/browser
                IAppStateSmartflow openSmartflow = AppSmartflowsState.GetSmartflowOpenedBy(UserSession.User, UserSession.Company.Id, UserSession.SelectedSystem);

                if(openSmartflow != null)
                {
                    await NotificationManager.ShowNotification("Warning", $"You already have open the Smartflow: {openSmartflow.SmartflowName}");
                }
                else
                {
                    Selected_VmClientSmartflowRecord.ClientSmartflowRecord = clientSmartflowRecord;

                    SelectedSmartflow.Name = clientSmartflowRecord.SmartflowName;

                    SelectedCaseTypeGroup = "";
                    SelectedCaseType = "";

                }



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

        

        private void PrepareSmartflowForInsert()
        {
            try
            {

                Edit_ClientSmartflowRecord = new Client_SmartflowRecord();

                if (!(SelectedSmartflow.CaseTypeGroup == ""))
                {
                    Edit_ClientSmartflowRecord.CaseTypeGroup = SelectedSmartflow.CaseTypeGroup;
                }
                else
                {
                    Edit_ClientSmartflowRecord.CaseTypeGroup = "";
                }

                if (!(SelectedSmartflow.CaseType == ""))
                {
                    Edit_ClientSmartflowRecord.CaseType = SelectedSmartflow.CaseType;
                }
                else
                {
                    Edit_ClientSmartflowRecord.CaseType = "";
                }

                if (!string.IsNullOrWhiteSpace(SelectedSmartflow.CaseTypeGroup) & !string.IsNullOrWhiteSpace(SelectedSmartflow.CaseType))
                {
                    Edit_ClientSmartflowRecord.SeqNo = LstSelected_VmClientSmartflowRecord
                                                    .OrderByDescending(C => C.ClientSmartflowRecord.SeqNo)
                                                    .Select(C => C.ClientSmartflowRecord.SeqNo)
                                                    .FirstOrDefault() + 1;
                }
                else
                {
                    Edit_ClientSmartflowRecord.SeqNo = 1;
                }

                ShowSmartflowAddOrEditModel();

            }
            catch(Exception e)
            {
                GenericErrorLog(false, e, "PrepareSmartflowForInsert", e.Message);
            }
        }

        private void PrepareCaseTypeForEdit(string caseType, string option)
        {
            EditCaseType = caseType;
            IsCaseTypeOrGroup = option;
            

            ShowCaseTypeEditModal();
        }

        private void PrepareSmartflowForEdit(Client_VmSmartflowRecord _vmClientSmartflowRecord)
        {
            if(IsSmartflowLocked(_vmClientSmartflowRecord))
            {
                NotificationManager.ShowNotification("Warning", $"This Smartflow is Locked by another user");
            }
            else
            {
                Edit_ClientSmartflowRecord = _vmClientSmartflowRecord.ClientSmartflowRecord;
                IsCaseTypeOrGroup = "Smartflow";

                ShowCaseTypeEditModal();
            }
        }

        protected async void ShowCaseTypeDetail(string caseTypeGroup, string caseType)
        {
            try
            {

                ScrollPosition = await JSRuntime.InvokeAsync<float>("getElementPosition");

                SelectedCaseTypeGroup = caseTypeGroup;
                SelectedCaseType = caseType;
                SelectedCaseTypeGroupPrev = caseTypeGroup;
                SelectedCaseTypePrev = caseType;

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


#endregion




#region Modals

        protected void ShowCaseTypeEditModal()
        {
            try
            {
                Action action = RefreshSmartflows;

                var parameters = new ModalParameters();
                parameters.Add("TaskObject", (IsCaseTypeOrGroup == "Smartflow") ? Edit_ClientSmartflowRecord.SmartflowData : EditCaseType);
                parameters.Add("originalName", (IsCaseTypeOrGroup == "Smartflow") ? Edit_ClientSmartflowRecord.SmartflowData : EditCaseType);
                if (IsCaseTypeOrGroup == "Smartflow")
                {
                    parameters.Add("Smartflow", Edit_ClientSmartflowRecord);
                }
                parameters.Add("DataChanged", action);
                parameters.Add("IsCaseTypeOrGroup", IsCaseTypeOrGroup);
                parameters.Add("caseTypeGroupName", SelectedSmartflow.CaseTypeGroup);
                parameters.Add("ListSmartflows", LstAll_VmClientSmartflowRecord);
                parameters.Add("CompanyDbAccess", CompanyDbAccess);
                parameters.Add("UserSession", UserSession);

                var options = new ModalOptions()
                {
                    Class = "blazored-custom-modal modal-smartflow-casetype"
                };

                Modal.Show<ModalSmartflowCaseTypeEdit>("Smartflow", parameters, options);
            }
            catch(Exception e)
            {
                GenericErrorLog(false,e, "ShowCaseTypeEditModal", e.Message);
            }
        }

        protected void ShowCaseTypeDeleteModal(string caseTypeGroup, string caseType)
        {
            EditCaseType = caseType;

            Action SelectedDeleteAction = HandleCaseTypeDelete;
            var parameters = new ModalParameters();
            parameters.Add("ItemName", caseType);
            parameters.Add("ModalHeight", "300px");
            parameters.Add("ModalWidth", "500px");
            parameters.Add("DeleteAction", SelectedDeleteAction);
            parameters.Add("InfoText", $"Are you sure you wish to delete the '{caseType}' case type? Cancel to create a backup first.");


            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal"
            };

            Modal.Show<ModalDelete>("Delete Case Type", parameters, options);
            
            
        }

        private async void HandleCaseTypeDelete()
        {
            await HandleCaseTypeDeleteTask();
        }

        private async Task HandleCaseTypeDeleteTask()
        {
            try
            {
                var lstRecordsToDelete = LstAll_VmClientSmartflowRecord.Where(R => R.ClientSmartflowRecord.CaseTypeGroup == SelectedSmartflow.CaseTypeGroup
                                                                                    && R.ClientSmartflowRecord.CaseType == EditCaseType)
                                                                                    .ToList();

                foreach(Client_VmSmartflowRecord recordToDelete in lstRecordsToDelete)
                {
                    await ClientApiManagementService.Delete(recordToDelete.ClientSmartflowRecord.Id);

                    LstSelected_VmClientSmartflowRecord.Remove(recordToDelete);
                    LstAll_VmClientSmartflowRecord.Remove(recordToDelete);
                    
                }
                
                await NotificationManager.ShowNotification("Success", $"{lstRecordsToDelete.Count()} Smartflows successfully deleted.");
                    
                RefreshSmartflows();    

            }
            catch(Exception e)
            {
                GenericErrorLog(true,e, "HandleCaseTypeDeleteTask", $"Deleting the Case Type {SelectedSmartflow.CaseTypeGroup}\\{EditCaseType}: {e.Message}");
            }
            
        }

        protected void ShowSmartflowAddOrEditModel()
        {
            Action action = RefreshSmartflows;

            var parameters = new ModalParameters();
            parameters.Add("TaskObject", Edit_ClientSmartflowRecord);
            parameters.Add("DataChanged", action);
            parameters.Add("AllObjects", LstAll_VmClientSmartflowRecord);
            parameters.Add("UserSession", UserSession);
            parameters.Add("CompanyDbAccess", CompanyDbAccess);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-smartflow-smartflow"
            };

            Modal.Show<ModalSmartflowAddOrEdit>("Smartflow", parameters, options);
        }



        protected async void ShowSmartflowDeleteModal(Client_VmSmartflowRecord _vmClientSmartflowRecord)
        {
            if(IsSmartflowLocked(_vmClientSmartflowRecord))
            {
                await NotificationManager.ShowNotification("Warning", $"This Smartflow is Locked by another user");
            }
            else
            {
                Edit_ClientSmartflowRecord = _vmClientSmartflowRecord.ClientSmartflowRecord;

                string itemName = _vmClientSmartflowRecord.ClientSmartflowRecord.SmartflowName;

                Action SelectedDeleteAction = HandleSmartflowDelete;
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
            
        }

        private async void HandleSmartflowDelete()
        {
            await HandleSmartflowDeleteTask();
        }

        private async Task HandleSmartflowDeleteTask()
        {
            try
            {
                await ClientApiManagementService.Delete(Edit_ClientSmartflowRecord.Id);

                var recordToRemove = LstSelected_VmClientSmartflowRecord.Where(S => S.ClientSmartflowRecord.Id == Edit_ClientSmartflowRecord.Id).First();

                LstSelected_VmClientSmartflowRecord.Remove(recordToRemove);
                LstAll_VmClientSmartflowRecord.Remove(recordToRemove);
                
                await NotificationManager.ShowNotification("Success", $"Smartflow successfully deleted.");
                    
                RefreshSmartflows();    

            }
            catch(Exception e)
            {
                GenericErrorLog(true,e, "HandleSmartflowDeleteTask", $"Deleting the Case Type {SelectedSmartflow.CaseTypeGroup}\\{EditCaseType}: {e.Message}");
            }
        }


#endregion

#region Smartflow Comparrisons


        /// <summary>
        /// Triggered from the checkbox at top of the screen to Sync Smartflows via the bound property SmartflowComparison
        /// </summary>
        /// <returns></returns>
        private async void CompareSelectedSmartflows()
        {
            try
            {
                LstSelected_VmClientSmartflowRecord.Select(C => { C.ComparisonIcon = null; C.ComparisonResult = null; return C; }).ToList();

                LstAlt_VmClientSmartflowRecord = new List<Client_VmSmartflowRecord>();
                Alt_VmClientSmartflowRecord.ClientSmartflowRecord = new Client_SmartflowRecord();
                await RefreshCompararisonSelectedSmartflows();
            }
            catch (Exception e)
            {
                GenericErrorLog(false,e, "CompareSelectedSmartflows", $"{e.Message}");
            }

            await InvokeAsync(() =>
            {
                StateHasChanged();
            });

        }

        private async Task<bool> RefreshCompararisonSelectedSmartflows()
        {
            try
            {
                if (CompareSystems)
                {
                    await RefreshAltSystemSmartflowList();

                    /*
                    * for every Smartflow get list of Smartflow items from both current system and alt system
                    * if any result returns false
                    * 
                    * 
                    */
                    if(!(LstAlt_VmClientSmartflowRecord is null) && LstAlt_VmClientSmartflowRecord.Count > 0)
                    {
                        foreach (var clientSmartflowRecord in LstSelected_VmClientSmartflowRecord)
                        {
                            //var smartflowItems = JsonConvert.DeserializeObject<Smartflow>(clientSmartflowRecord.ClientSmartflowRecord.SmartflowData);

                            Alt_VmClientSmartflowRecord.ClientSmartflowRecord = LstAlt_VmClientSmartflowRecord
                                                .Where(A => A.ClientSmartflowRecord.SmartflowName == clientSmartflowRecord.ClientSmartflowRecord.SmartflowName)
                                                .Where(A => A.ClientSmartflowRecord.CaseType == clientSmartflowRecord.ClientSmartflowRecord.CaseType)
                                                .Where(A => A.ClientSmartflowRecord.CaseTypeGroup == clientSmartflowRecord.ClientSmartflowRecord.CaseTypeGroup)
                                                .Select(C => C.ClientSmartflowRecord)
                                                .FirstOrDefault(); //get the first just in case there are 2 Smartflows with same name

                            if (Alt_VmClientSmartflowRecord.ClientSmartflowRecord is null)
                            {
                                //No corresponding Smartflow on the Alt system
                                clientSmartflowRecord.ComparisonResult = "No match";
                                clientSmartflowRecord.ComparisonIcon = "times";
                            }
                            else
                            {

                                if(Alt_VmClientSmartflowRecord.ClientSmartflowRecord.SmartflowData == clientSmartflowRecord.ClientSmartflowRecord.SmartflowData)
                                {
                                    clientSmartflowRecord.ComparisonResult = "Exact match";
                                    clientSmartflowRecord.ComparisonIcon = "check";
                                }
                                else
                                {
                                    clientSmartflowRecord.ComparisonResult = "Partial match";
                                    clientSmartflowRecord.ComparisonIcon = "exclamation";
                                }
                                
                                

                            }
                        }
                    }
                    else
                    {
                        LstSelected_VmClientSmartflowRecord.Select(T => { T.ComparisonIcon = "times"; T.ComparisonResult = "No match"; return T; }).ToList();
                    }

                
                }
            }
            catch(Exception e)
            {
                GenericErrorLog(true,e, "RefreshCompararisonSelectedSmartflows", $"Comparing all Smartflows from current listing: {e.Message}");

                return false;
            }

            return true;
        }



        protected async Task ShowSmartflowSyncOnAltModal(Client_SmartflowRecord clientSmartflowRecord)
        {
            Selected_VmClientSmartflowRecord.ClientSmartflowRecord = clientSmartflowRecord;

            string infoText = $"Do you wish to sync this smartflow to {(UserSession.SelectedSystem == "Live" ? "Dev" : "Live")}.";

            Action SelectedAction = SyncSelectedSmartflowOnAlt;
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

        private async void SyncSelectedSmartflowOnAlt()
        {
            try
            {
                await UserSession.SwitchSelectedSystem();

                Alt_VmClientSmartflowRecord.ClientSmartflowRecord = LstAlt_VmClientSmartflowRecord
                                            .Where(A => A.ClientSmartflowRecord.SmartflowName == Selected_VmClientSmartflowRecord.ClientSmartflowRecord.SmartflowName)
                                            .Where(A => A.ClientSmartflowRecord.CaseType == Selected_VmClientSmartflowRecord.ClientSmartflowRecord.CaseType)
                                            .Where(A => A.ClientSmartflowRecord.CaseTypeGroup == Selected_VmClientSmartflowRecord.ClientSmartflowRecord.CaseTypeGroup)
                                            .Select(C => C.ClientSmartflowRecord)
                                            .SingleOrDefault();

                if (Alt_VmClientSmartflowRecord.ClientSmartflowRecord is null)
                {
                    var newAlt_VmClientSmartflowRecord = new Client_SmartflowRecord
                    {
                        SeqNo = Selected_VmClientSmartflowRecord.ClientSmartflowRecord.SeqNo,
                        CaseTypeGroup = Selected_VmClientSmartflowRecord.ClientSmartflowRecord.CaseTypeGroup,
                        CaseType = Selected_VmClientSmartflowRecord.ClientSmartflowRecord.CaseType,
                        SmartflowName = Selected_VmClientSmartflowRecord.ClientSmartflowRecord.SmartflowName,
                        SmartflowData = Selected_VmClientSmartflowRecord.ClientSmartflowRecord.SmartflowData,
                        VariantName = Selected_VmClientSmartflowRecord.ClientSmartflowRecord.VariantName,
                        VariantNo = Selected_VmClientSmartflowRecord.ClientSmartflowRecord.VariantNo
                    };

                    var returnObject = await ClientApiManagementService.Add(newAlt_VmClientSmartflowRecord);
                    newAlt_VmClientSmartflowRecord.Id = returnObject.Id;

                    bool gotLock = CompanyDbAccess.Lock;
                    while (gotLock)
                    {
                        await Task.Yield();
                        gotLock = CompanyDbAccess.Lock;
                    }

                    await CompanyDbAccess.SaveSmartFlowRecord(newAlt_VmClientSmartflowRecord, UserSession);
                }
                else
                {
                    Alt_VmClientSmartflowRecord.ClientSmartflowRecord.SmartflowData = Selected_VmClientSmartflowRecord.ClientSmartflowRecord.SmartflowData;

                    await ClientApiManagementService.Update(Alt_VmClientSmartflowRecord.ClientSmartflowRecord);
                }


                
                await UserSession.ResetSelectedSystem();

                await RefreshCompararisonSelectedSmartflows();

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
                GenericErrorLog(false,e, "CreateSelectedSmartflowOnAlt", e.Message);
            }

        }


        private async Task<bool> RefreshAltSystemSmartflowList()
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

                var lstAppRecords = await CompanyDbAccess.GetAllAppSmartflowRecords(UserSession);

                if(!(lstAppRecords is null))
                {
                    LstAlt_VmClientSmartflowRecord = lstAppRecords.Select(A => new Client_VmSmartflowRecord { ClientSmartflowRecord = Mapper.Map(A, new Client_SmartflowRecord()) }).ToList();
                }
                
                await UserSession.ResetSelectedSystem();

                return true;
            }
            catch (Exception e)
            {
                GenericErrorLog(false,e, "RefreshAltSystemSmartflowList", $"{e.Message}");

                return false;
            }
        }



        

#endregion



#region Sync Systems
        
        public async void UpdateToSmartflowV2()
        {
            try
            {
                var lstAppRecords = await CompanyDbAccess.GetAllAppSmartflowRecords(UserSession);
                List<Client_VmSmartflowRecord> lst_VmClientSmartflowRecord = new List<Client_VmSmartflowRecord>();

                if(!(lstAppRecords is null))
                {
                    lst_VmClientSmartflowRecord = lstAppRecords.Select(A => new Client_VmSmartflowRecord { ClientSmartflowRecord = Mapper.Map(A, new Client_SmartflowRecord()) }).ToList();
                }

                var intRecords = 0;

                foreach(Client_VmSmartflowRecord vmClientSmartflowRecord in lst_VmClientSmartflowRecord)
                {
                    try
                    {
                        Smartflow smartflow = JsonConvert.DeserializeObject<Smartflow>(vmClientSmartflowRecord.ClientSmartflowRecord.SmartflowData);

                        Smartflow2 smarflow2 = new Smartflow2{
                                                        CaseTypeGroup = smartflow.CaseTypeGroup
                                                        , CaseType  = smartflow.CaseType
                                                        , Name  = smartflow.Name
                                                        , SeqNo  = smartflow.SeqNo
                                                        , P4WCaseTypeGroup  = smartflow.P4WCaseTypeGroup
                                                        , StepName  = smartflow.StepName
                                                        , BackgroundColour  = smartflow.BackgroundColour
                                                        , BackgroundColourName  = smartflow.BackgroundColourName
                                                        , BackgroundImage  = smartflow.BackgroundImage
                                                        , BackgroundImageName  = smartflow.BackgroundImageName
                                                        , ShowPartnerNotes  = smartflow.ShowPartnerNotes
                                                        , ShowDocumentTracking  = smartflow.ShowDocumentTracking
                                                        , GeneralNotes  = smartflow.GeneralNotes
                                                        , DeveloperNotes  = smartflow.DeveloperNotes
                                                        , SelectedView  = smartflow.SelectedView
                                                        , SelectedStep  = smartflow.SelectedStep
                                                        , Agendas  = new List<SmartflowAgenda>()
                                                        , Status  = new List<SmartflowStatus>()
                                                        , Documents  = new List<SmartflowDocument>()
                                                        , DataViews  = new List<SmartflowDataView>()
                                                        , Fees  = new List<SmartflowFee>()
                                                        , Messages  = new List<SmartflowMessage>()
                        };
                        
                        if(smartflow.Items != null)
                        {

                            foreach(GenSmartflowItem smartflowItem in smartflow.Items.Where(I => I.Type == "Agenda").ToList())
                            {
                                smarflow2.Agendas.Add(new SmartflowAgenda{ Name = smartflowItem.Name});
                            }

                            foreach(GenSmartflowItem smartflowItem in smartflow.Items.Where(I => I.Type == "Status").ToList())
                            {
                                smarflow2.Status.Add(new SmartflowStatus{ Name = smartflowItem.Name
                                                                            , SuppressStep = smartflowItem.SuppressStep
                                                                            , MilestoneStatus = smartflowItem.MilestoneStatus
                                                                            , SeqNo = smartflowItem.SeqNo
                                                                        });
                            }
                            
                            foreach(GenSmartflowItem smartflowItem in smartflow.Items.Where(I => I.Type == "Doc").ToList())
                            {
                                SmartflowDocument smartflowDocument = new SmartflowDocument{ Name = smartflowItem.Name
                                                                            , SeqNo = smartflowItem.SeqNo
                                                                            , AsName = smartflowItem.AsName
                                                                            , CompleteName = smartflowItem.CompleteName
                                                                            , RescheduleDays = smartflowItem.RescheduleDays
                                                                            , RescheduleDataItem = smartflowItem.RescheduleDataItem
                                                                            , AltDisplayName = smartflowItem.AltDisplayName
                                                                            , NextStatus = smartflowItem.NextStatus
                                                                            , Action = smartflowItem.Action
                                                                            , UserMessage = smartflowItem.UserMessage
                                                                            , PopupAlert = smartflowItem.PopupAlert
                                                                            , DeveloperNotes = smartflowItem.DeveloperNotes
                                                                            , StoryNotes = smartflowItem.StoryNotes
                                                                            , TrackingMethod = smartflowItem.TrackingMethod
                                                                            , ChaserDesc = smartflowItem.ChaserDesc
                                                                            , OptionalDocument = smartflowItem.OptionalDocument
                                                                            , CustomItem = smartflowItem.CustomItem
                                                                            , Agenda = smartflowItem.Agenda
                                                                            , LinkedItems = new List<LinkedDocument>()
                                                                        };

                                if(smartflowItem.LinkedItems != null)
                                {
                                    foreach(LinkedItem linkedItem in smartflowItem.LinkedItems.ToList())
                                    {
                                        smartflowDocument.LinkedItems.Add(new LinkedDocument{
                                                                        DocName = linkedItem.DocName
                                                                        , DocAsName = linkedItem.DocAsName
                                                                        , DocType = linkedItem.DocType
                                                                        , Action = linkedItem.Action
                                                                        , TrackingMethod = linkedItem.TrackingMethod
                                                                        , ChaserDesc = linkedItem.ChaserDesc
                                                                        , ScheduleDays = linkedItem.ScheduleDays
                                                                        , OptionalDocument = linkedItem.OptionalDocument
                                                                        , CustomItem = linkedItem.CustomItem
                                                                        , Agenda = linkedItem.Agenda
                                                                        , ScheduleDataItem = linkedItem.ScheduleDataItem

                                        });
                                    }
                                }
                                
                                smarflow2.Documents.Add(smartflowDocument);
                            }
                        }

                        if(smartflow.Fees != null)
                        {
                            foreach(SmartflowFee smartflowItem in smartflow.Fees.ToList())
                            {
                                smarflow2.Fees.Add(new SmartflowFee{ 
                                                                FeeName = smartflowItem.FeeName
                                                                , SeqNo = smartflowItem.SeqNo
                                                                , FeeCategory = smartflowItem.FeeCategory
                                                                , Amount = smartflowItem.Amount
                                                                , VATable = smartflowItem.VATable
                                                                , PostingType = smartflowItem.PostingType
                                });
                            }
                        }

                        if(smartflow.DataViews != null)
                        {
                            foreach(SmartflowDataView smartflowItem in smartflow.DataViews.ToList())
                            {
                                smarflow2.DataViews.Add(new SmartflowDataView{ 
                                                                ViewName = smartflowItem.ViewName
                                                                , SeqNo = smartflowItem.BlockNo
                                                                , DisplayName = smartflowItem.DisplayName
                                                    });
                            }
                        }

                        if(smartflow.TickerMessages != null)
                        {
                            foreach(SmartflowMessage smartflowItem in smartflow.TickerMessages.ToList())
                            {
                                smarflow2.Messages.Add(new SmartflowMessage{ 
                                                                Message = smartflowItem.Message
                                                                , SeqNo = smartflowItem.SeqNo
                                                                , FromDate = smartflowItem.FromDate
                                                                , ToDate = smartflowItem.ToDate
                                                                
                                                            });
                            }
                        }

                        //save back
                        vmClientSmartflowRecord.ClientSmartflowRecord.SmartflowData = JsonConvert.SerializeObject(smarflow2);
                        await ClientApiManagementService.Update(vmClientSmartflowRecord.ClientSmartflowRecord);

                        intRecords += 1;
                    }
                    catch(Exception e)
                    {
                        //possibly already converted
                        GenericErrorLog(false,e, "UpdateToSmartflowV2", e.Message);
                    }
                }
                await NotificationManager.ShowNotification("Success", $"{intRecords} converted");
            }
            catch(Exception e)
            {
                GenericErrorLog(true,e, "UpdateToSmartflowV2", e.Message);
            }
        }
       
        public async void UploadSmartFlowsFromClient()
        {
            try
            {
                var lstClientRecords = await ClientApiManagementService.GetAllSmartflows();

                await CompanyDbAccess.SyncAdminSysToClient(lstClientRecords, UserSession);

                RefreshSmartflows();

                var parameters = new ModalParameters();
                parameters.Add("InfoText", "Systems synced successfully");

                var options = new ModalOptions()
                {
                    Class = "blazored-custom-modal modal-smartflow-smartflow"
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
                GenericErrorLog(true,e, "SyncSmartFlowSystems", e.Message);
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
                SmartflowP4WStepSchema SmartflowP4WStep;

                foreach (var clientSmartflowRecord in LstAll_VmClientSmartflowRecord)
                {

                    var decodedSmartflow = JsonConvert.DeserializeObject<Smartflow>(clientSmartflowRecord.ClientSmartflowRecord.SmartflowData);

                    if (!string.IsNullOrEmpty(decodedSmartflow.SelectedStep) 
                        && !string.IsNullOrEmpty(decodedSmartflow.SelectedView)
                        && !string.IsNullOrEmpty(decodedSmartflow.Name)
                        && !string.IsNullOrEmpty(decodedSmartflow.StepName)
                        && !string.IsNullOrEmpty(decodedSmartflow.P4WCaseTypeGroup)
                        && !string.IsNullOrEmpty(decodedSmartflow.CaseTypeGroup))
                    {
                        if (decodedSmartflow.P4WCaseTypeGroup == "Entity Documents")
                        {
                            SmartflowP4WStep = new SmartflowP4WStepSchema
                            {
                                StepName = decodedSmartflow.StepName,
                                P4WCaseTypeGroup = decodedSmartflow.P4WCaseTypeGroup,
                                GadjITCaseTypeGroup = decodedSmartflow.CaseTypeGroup,
                                GadjITCaseType = decodedSmartflow.CaseType,
                                Smartflow = decodedSmartflow.Name,
                                SFVersion = Configuration["AppSettings:Version"],
                                Questions = new List<SmartflowP4WStepQuestion>{
                                        new SmartflowP4WStepQuestion {QNo = 1, QText= "HQ - Set Current Smartflow Details" }
                                        ,new SmartflowP4WStepQuestion {QNo = 2, QText= "HQ - Show View" }
                                        ,new SmartflowP4WStepQuestion {QNo = 3, QText= "HQ - Run Required Steps" }
                                        ,new SmartflowP4WStepQuestion {QNo = 4, QText= "HQ - Update reschedule date" }
                                        ,new SmartflowP4WStepQuestion {QNo = 5, QText= "HQ - Reinsert this step with asname" }
                                        ,new SmartflowP4WStepQuestion {QNo = 6, QText= "HQ - Rename Step" }
                                        ,new SmartflowP4WStepQuestion {QNo = 7, QText= "HQ - Delete Previous Instances if Suppressed Step" }
                                        ,new SmartflowP4WStepQuestion {QNo = 8, QText= "HQ - Check if completion name exists" }
                                        ,new SmartflowP4WStepQuestion {QNo = 9, QText= "HQ - Delete Step" }
                                        },
                                Answers = new List<SmartflowP4WStepAnswer>{
                                        new SmartflowP4WStepAnswer {QNo = 1, GoToData= $"2 [SQL: EXEC Up_ORSF_MoveDocsToAgendas '[entity.code]', 0] [SQL: EXEC up_ORSF_CreateTableEntries '[entity.code]', 0] [SQL: UPDATE Usr_ORSF_ENT_Control SET Current_SF = '{decodedSmartflow.Name}', Current_Case_Type_Group = '{decodedSmartflow.CaseTypeGroup}', Current_Case_Type = '{decodedSmartflow.CaseType}', Default_Step = '{decodedSmartflow.StepName}', Date_Schedule_For = DATEADD(d, 7, getdate()), Steps_To_Run = '', Schedule_AsName = '{decodedSmartflow.StepName}|' + (SELECT CASE WHEN dbo.fn_ORSF_IsAllCap (Description) = 1 THEN '{decodedSmartflow.StepName}' + '|' + CONVERT(VARCHAR(20),ISNULL(Date_Schedule_For,DATEADD(d, 7, getdate())),103) ELSE Description + '|' + CONVERT(VARCHAR(20),ISNULL(s.DiaryDate, getdate()),103) END + '|' + '[CurrentUser.Code]' FROM Cm_CaseItems i INNER JOIN Cm_Steps s on s.ItemID = i.ItemID WHERE i.ItemID = [currentstep.stepid]), Complete_AsName = '' WHERE EntityRef = '[Entity.Code]'] [SQL: UPDATE Usr_ORSF_ENT_Control SET Screen_Opened_Via_Step = 'Y' WHERE EntityRef = '[Entity.Code]' ]" }
                                        ,new SmartflowP4WStepAnswer {QNo = 2, GoToData= $"7 [VIEW: '{decodedSmartflow.SelectedView}' UPDATE=Yes]" }
                                        ,new SmartflowP4WStepAnswer {QNo = 3, GoToData= $"4 [TAKE: 'SF-Admin Save Items for Agenda Management' INSERT=END]  [SQL: EXEC up_ORSF_GetStepsFromList '[entity.code]', 0] [SQL: UPDATE Usr_ORSF_ENT_Control SET Screen_Opened_Via_Step = null WHERE EntityRef='[entity.code]']" }
                                        ,new SmartflowP4WStepAnswer {QNo = 4, GoToData= $"5 [SQL: UPDATE Usr_ORSF_ENT_Control SET Date_Schedule_For = isnull(Date_Schedule_For, Cast(getdate() as Date)) WHERE EntityRef = '[Entity.Code]']" }
                                        ,new SmartflowP4WStepAnswer {QNo = 5, GoToData= $"8 [SQL: SELECT ScheduleCommand FROM fn_ORSF_GetScheduleItems('[Entity.Code]', 0)]" }
                                        ,new SmartflowP4WStepAnswer {QNo = 6, GoToData= $"[SQL: UPDATE cm_caseitems set CompletionDate = GETDATE(), description = UPPER('[!Usr_ORSF_ENT_Control.Complete_AsName]') where itemid = [currentstep.stepid]] [SQL: exec up_ORSF_CompleteStep [currentStep.stepid], '', 'Y']" }
                                        ,new SmartflowP4WStepAnswer {QNo = 7, GoToData= $"3 [[SQL: SELECT CASE WHEN ISNULL(Do_Not_Reschedule,'N') = 'Y' THEN 'SQL: exec up_ORSF_DeleteDueStep '''', [currentstep.stepid], ''{decodedSmartflow.Name}''' ELSE '' END FROM Usr_ORSF_MT_Control WHERE EntityRef = '[matters.entityref]' AND MatterNo = [matters.number]]]" }
                                        ,new SmartflowP4WStepAnswer {QNo = 8, GoToData= $"[SQL: SELECT CASE WHEN ISNULL('[!Usr_ORSF_ENT_Control.Complete_AsName]','') <> '' THEN 6 ELSE 9 END]" }
                                        ,new SmartflowP4WStepAnswer {QNo = 9, GoToData= $"[SQL: exec up_ORSF_DeleteStep [currentstep.stepid]]" }
                                        }
                            };
                        }
                        else
                        {
                            SmartflowP4WStep = new SmartflowP4WStepSchema
                            {
                                StepName = decodedSmartflow.StepName,
                                P4WCaseTypeGroup = decodedSmartflow.P4WCaseTypeGroup,
                                GadjITCaseTypeGroup = decodedSmartflow.CaseTypeGroup,
                                GadjITCaseType = decodedSmartflow.CaseType,
                                Smartflow = decodedSmartflow.Name,
                                SFVersion = Configuration["AppSettings:Version"],
                                Questions = new List<SmartflowP4WStepQuestion>{
                                        new SmartflowP4WStepQuestion {QNo = 1, QText= "HQ - Set Current Smartflow Details" }
                                        ,new SmartflowP4WStepQuestion {QNo = 2, QText= "HQ - Show View" }
                                        ,new SmartflowP4WStepQuestion {QNo = 3, QText= "HQ - Run Required Steps" }
                                        ,new SmartflowP4WStepQuestion {QNo = 4, QText= "HQ - Update reschedule date" }
                                        ,new SmartflowP4WStepQuestion {QNo = 5, QText= "HQ - Reinsert this step with asname" }
                                        ,new SmartflowP4WStepQuestion {QNo = 6, QText= "HQ - Rename Step" }
                                        ,new SmartflowP4WStepQuestion {QNo = 7, QText= "HQ - Delete Previous Instances if Suppressed Step" }
                                        ,new SmartflowP4WStepQuestion {QNo = 8, QText= "HQ - Check if completion name exists" }
                                        ,new SmartflowP4WStepQuestion {QNo = 9, QText= "HQ - Delete Step" }
                                        },
                                Answers = new List<SmartflowP4WStepAnswer>{
                                        new SmartflowP4WStepAnswer {QNo = 1, GoToData= $"2 [SQL: EXEC Up_ORSF_MoveDocsToAgendas '[matters.entityref]', [matters.number]] [SQL: EXEC up_ORSF_CreateTableEntries '[matters.entityref]', [matters.number]] [SQL: UPDATE Usr_ORSF_MT_Control SET Current_SF = '{decodedSmartflow.Name}', Current_Case_Type_Group = '{decodedSmartflow.CaseTypeGroup}', Current_Case_Type = '{decodedSmartflow.CaseType}', Default_Step = '{decodedSmartflow.StepName}', Date_Schedule_For = DATEADD(d, 7, getdate()), Steps_To_Run = '', Schedule_AsName = '{decodedSmartflow.StepName}|' + (SELECT CASE WHEN dbo.fn_ORSF_IsAllCap (Description) = 1 THEN '{decodedSmartflow.StepName}' + '|' + CONVERT(VARCHAR(20),ISNULL(Date_Schedule_For,DATEADD(d, 7, getdate())),103) ELSE Description + '|' + CONVERT(VARCHAR(20),ISNULL(s.DiaryDate, getdate()),103) END + '|' + '[matters.feeearnerref]' FROM Cm_CaseItems i INNER JOIN Cm_Steps s on s.ItemID = i.ItemID WHERE i.ItemID = [currentstep.stepid]), Complete_AsName = '' WHERE EntityRef = '[matters.entityref]' AND MatterNo = [matters.number]] [SQL: UPDATE Usr_ORSF_MT_Control SET Screen_Opened_Via_Step = 'Y' WHERE EntityRef = '[matters.entityref]' AND matterNo =[matters.number]]" }
                                        ,new SmartflowP4WStepAnswer {QNo = 2, GoToData= $"7 [VIEW: '{decodedSmartflow.SelectedView}' UPDATE=Yes]" }
                                        ,new SmartflowP4WStepAnswer {QNo = 3, GoToData= $"4 [TAKE: 'SF-Admin Save Items for Agenda Management' INSERT=END]  [SQL: EXEC up_ORSF_GetStepsFromList '[matters.entityref]', [matters.number]] [SQL: UPDATE Usr_ORSF_MT_Control SET Screen_Opened_Via_Step = null WHERE EntityRef='[matters.entityref]' AND matterNo=[matters.number]]" }
                                        ,new SmartflowP4WStepAnswer {QNo = 4, GoToData= $"5 [SQL: UPDATE Usr_ORSF_MT_Control SET Date_Schedule_For = isnull(Date_Schedule_For, Cast(getdate() as Date)) WHERE EntityRef = '[matters.entityref]' AND MatterNo = [matters.number]]" }
                                        ,new SmartflowP4WStepAnswer {QNo = 5, GoToData= $"8 [SQL: SELECT ScheduleCommand FROM fn_ORSF_GetScheduleItems('[matters.entityref]', [matters.number])]" }
                                        ,new SmartflowP4WStepAnswer {QNo = 6, GoToData= $"[SQL: UPDATE cm_caseitems set CompletionDate = GETDATE(), description = UPPER('[!Usr_ORSF_MT_Control.Complete_AsName]') where itemid = [currentstep.stepid]] [SQL: exec up_ORSF_CompleteStep [currentStep.stepid], '', 'Y']" }
                                        ,new SmartflowP4WStepAnswer {QNo = 7, GoToData= $"3 [[SQL: SELECT CASE WHEN ISNULL(Do_Not_Reschedule,'N') = 'Y' THEN 'SQL: exec up_ORSF_DeleteDueStep '''', [currentstep.stepid], ''{decodedSmartflow.Name}''' ELSE '' END FROM Usr_ORSF_MT_Control WHERE EntityRef = '[matters.entityref]' AND MatterNo = [matters.number]]]" }
                                        ,new SmartflowP4WStepAnswer {QNo = 8, GoToData= $"[SQL: SELECT CASE WHEN ISNULL('[!Usr_ORSF_MT_Control.Complete_AsName]','') <> '' THEN 6 ELSE 9 END]" }
                                        ,new SmartflowP4WStepAnswer {QNo = 9, GoToData= $"[SQL: exec up_ORSF_DeleteStep [currentstep.stepid]]" }
                                        }
                            };
                        }

                        stepJSON = JsonConvert.SerializeObject(SmartflowP4WStep);

                        

                        creationSuccess = await ClientApiManagementService.CreateStep(new P4W_SmartflowStepSchemaJSONObject { StepSchemaJSON = stepJSON });

                        if (!creationSuccess)
                        {
                            overAllCreationSuccess = false;
                        }

                        creationCount += 1;

                    }

                }

                //Refresh the global step that saves "Completed" docs into the relevant Agendas
                //This step will be called by each Smartflow
                SmartflowP4WStep = new SmartflowP4WStepSchema
                {
                    StepName = "SF-Admin Save Items for Agenda Management",
                    P4WCaseTypeGroup = "Global Documents",
                    GadjITCaseTypeGroup = "Global",
                    GadjITCaseType = "Global Documents",
                    Smartflow = "Admin Save Items for Agenda Management",
                    SFVersion = Configuration["AppSettings:Version"],
                    Questions = new List<SmartflowP4WStepQuestion>{
                            new SmartflowP4WStepQuestion {QNo = 1, QText= "HQ - Save Items for Agenda Management" }
                            ,new SmartflowP4WStepQuestion {QNo = 2, QText= "HQ - Delete Me" }
                            },
                    Answers = new List<SmartflowP4WStepAnswer>{
                            new SmartflowP4WStepAnswer {QNo = 1, GoToData= $"2 [SQL: exec up_ORSF_Agenda_Control '[matters.entityref]', [matters.number], [currentstep.stepid] ]" }
                            ,new SmartflowP4WStepAnswer {QNo = 2, GoToData= $"[SQL: EXEC dbo.up_ORSF_DeleteStep [currentstep.stepid]]" }
                            }
                };

                stepJSON = JsonConvert.SerializeObject(SmartflowP4WStep);

                creationSuccess = await ClientApiManagementService.CreateStep(new P4W_SmartflowStepSchemaJSONObject { StepSchemaJSON = stepJSON });

                if (!creationSuccess)
                {
                    overAllCreationSuccess = false;
                }

                var parameters = new ModalParameters();
                var message = overAllCreationSuccess ? "Creation Successfull" : "Creation Unsuccessfull";
                parameters.Add("InfoText", message);

                var options = new ModalOptions()
                {
                    Class = "blazored-custom-modal modal-smartflow-smartflow"
                };
                string title = $"Updated {creationCount} Steps";
                Modal.Show<ModalInfo>(title, parameters, options);
            }
            catch (Exception e)
            {
                GenericErrorLog(true,e, "UpdateSteps", $"Creating/Updating all P4W Smartflow steps: {e.Message}");

            }

        }


#endregion



#region Error Handling


        /****************************************/
        /* ERROR HANDLING */
        /****************************************/
        private async void GenericErrorLog(bool showNotificationMsg, Exception e, string _method, string _message)
        {
            if(showNotificationMsg)
            {
                await NotificationManager.ShowNotification("Danger", $"Oops! Something went wrong.");
            }

            using (LogContext.PushProperty("SourceSystem", UserSession.SelectedSystem))
            using (LogContext.PushProperty("SourceCompanyId", UserSession.Company.Id))
            using (LogContext.PushProperty("SourceUserId", UserSession.User.Id))
            using (LogContext.PushProperty("SourceContext", nameof(SmartflowList)))
            {
                Logger.LogError(e,"Error - Method: {0}, Message: {1}",_method, _message);
            }

        }


#endregion

    }
}
