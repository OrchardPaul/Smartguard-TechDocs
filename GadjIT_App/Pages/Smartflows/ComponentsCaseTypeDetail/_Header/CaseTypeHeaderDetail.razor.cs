using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Blazored.Modal.Services;
using GadjIT_App.Services;
using GadjIT_App.Services.AppState;
using GadjIT_App.Services.SessionState;
using GadjIT_ClientContext.Models.Smartflow.Client;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using Microsoft.JSInterop;
using Blazored.Modal;
using GadjIT_App.Pages._Shared.Modals;
using GadjIT_App.Data.Admin;
using System.Net.Http;
using AutoMapper;

namespace GadjIT_App.Pages.Smartflows.ComponentsCaseTypeDetail._Header
{
    public partial class CaseTypeHeaderDetail
    {
        [Parameter]
        public List<Client_VmSmartflowRecord> _LstVmClientSmartflowRecord { get; set; }

        [Parameter]
        public string _SelectedCaseTypeGroup { get; set; }

        [Parameter]
        public string _SelectedCaseType { get; set; }

        [Parameter]
        public EventCallback<string> _ShowNav {get; set;}

        [Parameter]
        public EventCallback<Client_SmartflowRecord> _SelectSmartflow {get; set;}

        [Parameter]
        public EventCallback _RefreshSmartflowsTask {get; set;}

            
        [Inject]
        IModalService Modal { get; set; }

        [Inject]
        public IUserSessionState UserSession { get; set; }

        [Inject]
        private ICompanyDbAccess CompanyDbAccess { get; set; }

        [Inject]
        public IMapper Mapper { get; set; }


        [Inject]
        private IClientApiManagementService ClientApiManagementService { get; set; }
       
        [Inject]
        public INotificationManager NotificationManager {get; set;}

        [Inject]
        private ILogger<SmartflowList> Logger { get; set; }

        [Inject]
        public IAppSmartflowsState AppSmartflowsState { get; set; }

        protected List<Client_VmSmartflowRecord> LstSmartflows {get; set;}

        private List<Client_VmSmartflowRecord> LstAlt_VmClientSmartflowRecord { get; set; } = new List<Client_VmSmartflowRecord>();

        public Client_VmSmartflowRecord Selected_VmClientSmartflowRecord { get; set; } = new Client_VmSmartflowRecord();
        public Client_VmSmartflowRecord Alt_VmClientSmartflowRecord { get; set; } = new Client_VmSmartflowRecord();
        public Client_SmartflowRecord Edit_ClientSmartflowRecord = new Client_SmartflowRecord ();


        
        [Inject]
        private IJSRuntime JSRuntime {get; set;}

        int RowChanged { get; set; } = 0; 

        private bool SeqMoving = false;

        protected bool CompareSystems = false;

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


                            
        protected override async Task OnInitializedAsync()
        {
            
            await RefreshSmartflowListTask(false);

            
        }

        private Timer TimerStateChanged;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                try
                {
                    StateHasChanged();
                    await JSRuntime.InvokeVoidAsync("showPageAfterFirstRender");
                }
                catch (Exception e)
                {
                    GenericErrorLog(false,e, "OnAfterRenderAsync", e.Message);  
                }
            }
            else
            {
                

                TimerStateChanged = new Timer(async _ =>
                {
                    await InvokeAsync(() =>
                    {
                        StateHasChanged();
                    });

                }, null, 20000, 2000);

            }
            base.OnAfterRender(firstRender);
        }

        public void Dispose()
        {
            TimerStateChanged?.Dispose();
        }

        protected async Task RefreshAllSmartflows()
        {
            await _RefreshSmartflowsTask.InvokeAsync();

            await RefreshSmartflowListTask(false);

            await NotificationManager.ShowNotification("Success", $"Smartflow list refreshed");

        }

        private async void RefreshSmartflowList()
        {
            await RefreshSmartflowListTask(true);
        }


        private async Task RefreshSmartflowListTask(bool forceChange)
        {
            //_LstVmClientSmartflowRecord may change if other users are making updates
            // these changes will be refreshed within SmartflowList during a timer event
            // and passed down via Parameter delegates

            LstSmartflows = _LstVmClientSmartflowRecord
                                        .Where(C => C.ClientSmartflowRecord.CaseTypeGroup == _SelectedCaseTypeGroup)
                                        .Where(C => C.ClientSmartflowRecord.CaseType == _SelectedCaseType)
                                        .OrderBy(C => C.ClientSmartflowRecord.SeqNo)
                                        .ToList();


            if(LstSmartflows.Count > 0)
            {
                             
                await ReSequenceSmartFlows();

                await RefreshSmartflowIssues();

                foreach(Client_VmSmartflowRecord vmSmartflow in LstSmartflows)
                {
                    vmSmartflow.SetSmartflowStatistics();
                }

            }

            if(forceChange)
            {
                await InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            }
            
        }

        protected async Task ShowNav(string navItem)
        {
            await _ShowNav.InvokeAsync(navItem);
        }


        private void ShowCreateSmartflowModal()
        {
            try
            {

                Edit_ClientSmartflowRecord = new Client_SmartflowRecord();

                Edit_ClientSmartflowRecord.CaseTypeGroup = _SelectedCaseTypeGroup;
                Edit_ClientSmartflowRecord.CaseType = _SelectedCaseType;
                

                Edit_ClientSmartflowRecord.SeqNo = LstSmartflows
                                                    .OrderByDescending(C => C.ClientSmartflowRecord.SeqNo)
                                                    .Select(C => C.ClientSmartflowRecord.SeqNo)
                                                    .FirstOrDefault() + 1;
                
                Action action = RefreshSmartflowList;

                var parameters = new ModalParameters();
                parameters.Add("_TaskObject", Edit_ClientSmartflowRecord.SmartflowData);
                parameters.Add("_Smartflow", Edit_ClientSmartflowRecord);
                parameters.Add("_LstVmClientSmartflowRecord", _LstVmClientSmartflowRecord);
                parameters.Add("_DataChanged", action);
                parameters.Add("_TaskType", "Add");

                
                var options = new ModalOptions()
                {
                    Class = "blazored-custom-modal modal-smartflow-smartflow"
                };

                Modal.Show<ModalSmartflowEdit>("Smartflow", parameters, options);

            }
            catch(Exception e)
            {
                GenericErrorLog(false, e, "PrepareSmartflowForInsert", e.Message);
            }
        }



        protected void ShowSmartflowEditModal(Client_VmSmartflowRecord _vmClientSmartflowRecord)
        {
            try
            {
                if(IsSmartflowLocked(_vmClientSmartflowRecord))
                {
                    NotificationManager.ShowNotification("Warning", $"This Smartflow is Locked by another user");
                }
                else
                {
                    Edit_ClientSmartflowRecord = _vmClientSmartflowRecord.ClientSmartflowRecord;
                    

                    Action action = RefreshSmartflowList;

                    var parameters = new ModalParameters();
                    parameters.Add("_TaskObject", Edit_ClientSmartflowRecord.SmartflowData);
                    parameters.Add("_Smartflow", Edit_ClientSmartflowRecord);
                    parameters.Add("_LstVmClientSmartflowRecord", _LstVmClientSmartflowRecord);
                    parameters.Add("_DataChanged", action);
                    parameters.Add("_TaskType", "Edit");

                    var options = new ModalOptions()
                    {
                        Class = "blazored-custom-modal modal-smartflow-casetype"
                    };

                    Modal.Show<ModalSmartflowEdit>("Smartflow", parameters, options);
                }
            }
            catch(Exception e)
            {
                GenericErrorLog(false,e, "ShowCaseTypeEditModal", e.Message);
            }
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

                var recordToRemove = _LstVmClientSmartflowRecord.Where(S => S.ClientSmartflowRecord.Id == Edit_ClientSmartflowRecord.Id).First();

                _LstVmClientSmartflowRecord.Remove(recordToRemove);
                
                await NotificationManager.ShowNotification("Success", $"Smartflow successfully deleted.");
                    
                await RefreshSmartflowListTask(false);    

            }
            catch(Exception e)
            {
                GenericErrorLog(true,e, "HandleSmartflowDeleteTask", $"Deleting the Smartflow {Edit_ClientSmartflowRecord.SmartflowName}: {e.Message}");
            }
        }



        private async Task<bool> RefreshSmartflowIssues()
        {
            try
            {
                
                if(!(LstSmartflows is null) && LstSmartflows.Count > 0)
                {
                    foreach (var smartflow in LstSmartflows)
                    {
                        smartflow.ComparisonList.Clear();

                        //check for duplicate Smartflow names
                        var numDuplicates = LstSmartflows
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



        public async Task ReSequenceSmartFlows(int seq)
        {
            //RowChanged = seq;
            await ReSequenceSmartFlows();
        }

         public async Task ReSequenceSmartFlows()
        {
            try
            {
                if(LstSmartflows.Select(C => C.ClientSmartflowRecord.SeqNo != LstSmartflows.IndexOf(C) + 1).Count() > 0) //If any SeqNos are out of sequence
                { 
                    SeqMoving = true;

                    LstSmartflows.Select(C => { C.ClientSmartflowRecord.SeqNo = LstSmartflows.IndexOf(C) + 1; return C; }).ToList(); //update all SeqNo by setting to the Index (+1)

                    foreach (var smartflowToChange in LstSmartflows)
                    {
                        await ClientApiManagementService.UpdateMainItem(smartflowToChange.ClientSmartflowRecord);

                    }

                    
                }

                
            }
            catch(Exception e)
            {
                GenericErrorLog(true,e, "ReSequenceSmartFlows", $"Resequencing smartflows: {e.Message}");
            }
            finally
            {
                SeqMoving = false;
            }

        }

        protected async Task MoveSmartFlowSeq(Client_SmartflowRecord _selectedSmartflow, string _direction)
        {
            try
            {
                var lstItems = new List<Client_VmSmartflowRecord>();
                int incrementBy;

                incrementBy = (_direction.ToLower() == "up" ? -1 : 1);

                RowChanged = (int)(_selectedSmartflow.SeqNo + incrementBy);

                var swapItem = LstSmartflows.Where(S => S.ClientSmartflowRecord.SeqNo == RowChanged).FirstOrDefault();
                if (!(swapItem is null))
                {
                    _selectedSmartflow.SeqNo += incrementBy;
                    swapItem.ClientSmartflowRecord.SeqNo = RowChanged + (incrementBy * -1);

                    //re-index the list
                    LstSmartflows = LstSmartflows
                                        .OrderBy(C => C.ClientSmartflowRecord.SeqNo)
                                        .ToList();

                    await ClientApiManagementService.UpdateMainItem(_selectedSmartflow);
                    await ClientApiManagementService.UpdateMainItem(swapItem.ClientSmartflowRecord);
                }
                

            }
            catch (Exception e)
            {
                GenericErrorLog(true,e, "MoveSmartFlowSeq", $"Moving smartflow: {e.Message}");

            }


        }


        public void ResetRowChanged() 
        {
            RowChanged = 0;
            SeqMoving = false;

            StateHasChanged();
        }

        

        public async Task SelectSmartflow(Client_SmartflowRecord smartflow)
        {
            await _SelectSmartflow.InvokeAsync(smartflow);
        }

        public bool IsSmartflowLockedByOtherUser(Client_VmSmartflowRecord _vmClientSmartflowRecord)
        {
            try
            {
                return AppSmartflowsState.IsSmartflowLockedByOtherUser(
                                                UserSession.User
                                                , _vmClientSmartflowRecord.ClientSmartflowRecord.Id
                                             );

            }
            catch(Exception e)
            {
                GenericErrorLog(true,e, "IsSmartflowLockedByOtherUser", $"Selecting smartflow: {e.Message}");

                return false;

            }

        }

        public bool IsSmartflowLocked(Client_VmSmartflowRecord _vmClientSmartflowRecord)
        {
            try
            {
                return AppSmartflowsState.IsSmartflowLocked(_vmClientSmartflowRecord.ClientSmartflowRecord.Id);

            }
            catch(Exception e)
            {
                GenericErrorLog(true,e, "IsSmartflowLocked", $"Selecting smartflow: {e.Message}");

                return false;

            }

        }

        public void UnlockSmartflow(Client_VmSmartflowRecord _vmClientSmartflowRecord)
        {
            try
            {
                // AppSmartflowsState.UnlockSmartflow(
                //                                 UserSession.Company.Id
                //                                 , UserSession.SelectedSystem
                //                                 , _vmClientSmartflowRecord.ClientSmartflowRecord.CaseTypeGroup
                //                                 , _vmClientSmartflowRecord.ClientSmartflowRecord.CaseType
                //                                 , _vmClientSmartflowRecord.ClientSmartflowRecord.SmartflowName
                //                              );
                AppSmartflowsState.UnlockSmartflow(_vmClientSmartflowRecord.ClientSmartflowRecord.Id);
                
                StateHasChanged();

            }
            catch(Exception e)
            {
                GenericErrorLog(true,e, "UnlockSmartflow", $"Unlocking smartflow: {e.Message}");

            }

        }
        
      
      #region Smartflow Comparrisons


        /// <summary>
        /// Triggered from the checkbox at top of the screen to Sync Smartflows via the bound property SmartflowComparison
        /// </summary>
        /// <returns></returns>
        private async void CompareSelectedSmartflows()
        {
            try
            {
                _LstVmClientSmartflowRecord.Select(C => { C.ComparisonIcon = null; C.ComparisonResult = null; return C; }).ToList();

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
                        foreach (var clientSmartflowRecord in _LstVmClientSmartflowRecord)
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
                        _LstVmClientSmartflowRecord.Select(T => { T.ComparisonIcon = "times"; T.ComparisonResult = "No match"; return T; }).ToList();
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



        
        /****************************************/
        /* ERROR HANDLING AND NOTIFICATIONS     */
        /****************************************/
        private async void GenericErrorLog(bool _showNotificationMsg, Exception e, string _method, string _message)
        {
            if(_showNotificationMsg)
            {
                await NotificationManager.ShowNotification("Danger", $"Oops! Something went wrong.");
            }

            using (LogContext.PushProperty("SourceSystem", UserSession.SelectedSystem))
            using (LogContext.PushProperty("SourceCompanyId", UserSession.Company.Id))
            using (LogContext.PushProperty("SourceUserId", UserSession.User.Id))
            using (LogContext.PushProperty("SourceContext", nameof(CaseTypeHeaderDetail)))
            {
                Logger.LogError(e,"Error - Method: {0}, Message: {1}",_method, _message);
            }

        }

        


    }

}