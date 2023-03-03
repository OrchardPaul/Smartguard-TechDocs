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
        private IClientApiManagementService ClientApiManagementService { get; set; }
       
        [Inject]
        public INotificationManager NotificationManager {get; set;}

        [Inject]
        private ILogger<SmartflowList> Logger { get; set; }

        [Inject]
        public IAppSmartflowsState AppSmartflowsState { get; set; }

        protected List<Client_VmSmartflowRecord> LstSmartflows {get; set;}

        
        [Inject]
        private IJSRuntime JSRuntime {get; set;}

        int RowChanged { get; set; } = 0; 

        private bool SeqMoving = false;

                            
        protected override void OnInitialized()
        {
            RefreshSmartflowList();

            foreach(Client_VmSmartflowRecord vmSmartflow in LstSmartflows)
            {
                vmSmartflow.SetSmartflowStatistics();
            }
        }

        private Timer TimerRefreshList;
        private Timer TimerStateChanged;

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
                TimerRefreshList = new Timer(async _ =>
                {
                    try
                    {
                        await InvokeAsync(() =>
                        {
                            _RefreshSmartflowsTask.InvokeAsync().ConfigureAwait(false);
                            //NotificationManager.ShowNotification("Danger", $"Refresh List");
                        });

                    }
                    catch(Exception e)
                    {
                        GenericErrorLog(false,e, "OnAfterRenderAsync", $"Attempting to Invoke _RefreshSmartflowsTask"); 
                    }

                }, null, 20000, 20000);

                TimerStateChanged = new Timer(async _ =>
                {
                    try
                    {
                        await InvokeAsync(() =>
                        {
                            StateHasChanged();
                        });

                    }
                    catch(Exception e)
                    {
                        GenericErrorLog(false,e, "OnAfterRenderAsync", $"Attempting to Invoke _RefreshSmartflowsTask"); 
                    }

                }, null, 25000, 20000);

            }
            base.OnAfterRender(firstRender);
        }

        public void Dispose()
        {
            TimerRefreshList?.Dispose();
            TimerStateChanged?.Dispose();
        }

    
        private void RefreshSmartflowList()
        {
            //_LstVmClientSmartflowRecord may change if other users are making updates
            // these changes will be refreshed within SmartflowList during a timer event
            // and passed down via Parameter delegates

            LstSmartflows = _LstVmClientSmartflowRecord
                                            .Where(C => C.ClientSmartflowRecord.CaseTypeGroup == _SelectedCaseTypeGroup)
                                            .Where(C => C.ClientSmartflowRecord.CaseType == _SelectedCaseType)
                                            .OrderBy(C => C.ClientSmartflowRecord.SeqNo)
                                            .ToList();

            LstSmartflows = LstSmartflows
                                        .OrderBy(C => C.ClientSmartflowRecord.SeqNo)
                                        .ToList();
        }

        protected async Task ShowNav(string navItem)
        {
            await _ShowNav.InvokeAsync(navItem);
        }

        public async Task ReSequenceSmartFlows(int seq)
        {
            RowChanged = seq;
            await ReSequenceSmartFlows();
        }

        public async Task ReSequenceSmartFlows()
        {
            try
            {
                if(LstSmartflows.Select(C => C.ClientSmartflowRecord.SeqNo != LstSmartflows.IndexOf(C) + 1).Count() > 0) //If any SeqNos are out of sequence
                { 
                    LstSmartflows.Select(C => { C.ClientSmartflowRecord.SeqNo = LstSmartflows.IndexOf(C) + 1; return C; }).ToList(); //update all SeqNo by setting to the Index (+1)

                    foreach (var smartflowToChange in LstSmartflows)
                    {
                        await ClientApiManagementService.UpdateMainItem(smartflowToChange.ClientSmartflowRecord).ConfigureAwait(false);

                    }

                    StateHasChanged();
                }

                
            }
            catch
            {
                
            }

        }

        protected async void MoveSmartFlowSeq(Client_SmartflowRecord selectobject, string listType, string direction)
        {
            try
            {
                ResetRowChanged();

                SeqMoving = true; //prevents changes to the form whilst process of changing seq is carried out

                var lstItems = new List<Client_VmSmartflowRecord>();
                int incrementBy;

                incrementBy = (direction.ToLower() == "up" ? -1 : 1);

                RowChanged = (int)(selectobject.SeqNo + incrementBy);

                lstItems = LstSmartflows
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

                SeqMoving = false;

                RefreshSmartflowList();


                await InvokeAsync(() =>
                {
                    StateHasChanged();
                });
                
                               

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