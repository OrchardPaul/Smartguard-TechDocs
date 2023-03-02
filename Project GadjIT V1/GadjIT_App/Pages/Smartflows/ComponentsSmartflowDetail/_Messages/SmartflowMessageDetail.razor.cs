using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.Modal;
using Blazored.Modal.Services;
using GadjIT_App.Data.Admin;
using GadjIT_App.Services;
using GadjIT_App.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog.Context;
using GadjIT_AppContext.GadjIT_App;
using GadjIT_App.Pages.Smartflows.ComponentsSmartflowDetail._SharedItems;
using GadjIT_App.Pages._Shared.Modals;
using System.Net.Http;
using GadjIT_ClientContext.Models.Smartflow;
using GadjIT_ClientContext.Models.Smartflow.Client;

namespace GadjIT_App.Pages.Smartflows.ComponentsSmartflowDetail._Messages
{
    public partial class SmartflowMessageDetail
    {

        [Parameter]
        public List<VmSmartflowMessage> _LstMessages { get; set; } 

        [Parameter]
        public Client_SmartflowRecord _Selected_ClientSmartflowRecord { get; set; }

        [Parameter]
        public Smartflow _SelectedSmartflow { get; set; }

        [Parameter]
        public EventCallback<Smartflow> _SmartflowUpdated {get; set;}

        [Parameter]
        public EventCallback<string> _RefreshSmartflowItems {get; set;}

        [Parameter]
        public bool _SmartflowLockedForEdit {get; set;}


        

        

        [Inject]
        private ILogger<SmartflowMessageDetail> Logger { get; set; }

        [Inject]
        public INotificationManager NotificationManager {get; set;}

        [Inject]
        private IClientApiManagementService ClientApiManagementService { get; set; }

        [Inject]
        private ICompanyDbAccess CompanyDbAccess { get; set; }

        [Inject]
        public IUserSessionState UserSession { get; set; }

        [Inject]
        IModalService Modal { get; set; }


        private App_SmartflowRecord Alt_AppSmartflowRecord {get; set;} //as saved on Company

        private Client_SmartflowRecord Alt_ClientSmartflowRecord { get; set; } = new Client_SmartflowRecord(); //as saved on client site with serialised VmSmartflow

        private Smartflow AltSmartflow {get; set;} //Smartflow Schema

        public List<VmSmartflowMessage> LstAltSystemItems { get; set; } 

        public VmSmartflowMessage EditObject = new VmSmartflowMessage { Message = new SmartflowMessage() };


        private bool CompareSystems_;
        protected bool CompareSystems 
                {
                    get
                    { 
                        return CompareSystems_;
                    } 
                    set
                    {
                            _ = CompareToAltSystem(value);
                    }
                }

        public bool SeqMoving {get; set;}

        private int RowChanged = 0;


        protected void PrepareForInsert()
        {
            try
            {

                EditObject = new VmSmartflowMessage { Message = new SmartflowMessage() };

                EditObject.Message.FromDate = DateTime.Now.ToString("yyyyMMdd");
                EditObject.Message.ToDate = DateTime.Now.ToString("yyyyMMdd");

                if(!(_LstMessages is null ) && _LstMessages.Count() > 0)
                {

                    EditObject.Message.SeqNo = _LstMessages
                                                        .OrderByDescending(D => D.Message.SeqNo)
                                                        .Select(D => D.Message.SeqNo)
                                                        .FirstOrDefault() + 1;
                }
                else
                {
                    EditObject.Message.SeqNo = 1;
                }

                ShowSmartflowMessageModal("Insert");
            }
            catch(Exception e)
            {
                GenericErrorLog(false, e, "PrepareForInsert", e.Message);
            }
        }

        private void PrepareForEdit(VmSmartflowMessage _selectedObject)
        {
            EditObject = _selectedObject;

            ShowSmartflowMessageModal("Edit");
        }

        protected void ShowSmartflowMessageModal(string _option)
        {
            try
            {
                Action  dataChanged = HandleUpdate;

                var copyObject = new SmartflowMessage
                {
                    SeqNo = EditObject.Message.SeqNo,
                    Message = EditObject.Message.Message,
                    FromDate = EditObject.Message.FromDate,
                    ToDate = EditObject.Message.ToDate
                };

                var parameters = new ModalParameters();
                parameters.Add("_Option", _option);
                parameters.Add("_TaskObject", EditObject.Message);
                parameters.Add("_CopyObject", copyObject);
                parameters.Add("_SelectedSmartflow", _SelectedSmartflow);
                parameters.Add("_Selected_ClientSmartflowRecord", _Selected_ClientSmartflowRecord);
                parameters.Add("_DataChanged", dataChanged);

                var options = new ModalOptions()
                {
                    Class = "blazored-custom-modal modal-smartflow-item"
                };

                Modal.Show<ModalSmartflowMessage>("Ticker Messages", parameters, options);
            }
            catch(Exception e)
            {
                GenericErrorLog(false,e, "ShowSmartflowMessageModal", e.Message);
            }
        }

        private async void HandleUpdate()
        {
            await ChapterItemsUpdated();

        }


        protected void ShowSmartflowMessageViewModal(VmSmartflowMessage _selectedObject)
        {
            try
            {
                var parameters = new ModalParameters();
                parameters.Add("_Object", _selectedObject);

                var options = new ModalOptions()
                {
                    Class = "blazored-custom-modal modal-smartflow-comparison"
                };

                Modal.Show<ModalSmartflowMessageView>("Ticker Message", parameters, options);
            }
            catch(Exception e)
            {
                GenericErrorLog(false,e, "ShowSmartflowMessageViewModal", e.Message);
            }
        }

        protected void ShowSmartflowMessageDelete(VmSmartflowMessage _SelectedSmartflowItem) 
        {
            EditObject = _SelectedSmartflowItem;

            string itemName = _SelectedSmartflowItem.Message.Message;
            string itemType = "Ticker Message";

            Action SelectedDeleteAction = HandleDelete;
            var parameters = new ModalParameters();
            parameters.Add("_ItemName", itemName);
            parameters.Add("_DeleteAction", SelectedDeleteAction);
            parameters.Add("_InfoText", $"Are you sure you wish to delete the '{itemName}' {itemType.ToLower()}? ");

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal"
            };

            Modal.Show<ModalSmartflowDetailDelete>($"Delete {itemType}", parameters, options);

        }

        private async void HandleDelete() 
        {
            
            //<ModalDelete> simply invokes this method when user clicks OK. No need for the modal to handle this action as we do not require any details from the Modal. 
            _SelectedSmartflow.TickerMessages.Remove(EditObject.Message);
           
            await ChapterItemsUpdated();

        }
        
        private async Task ChapterItemsUpdated()
        {
            await _SmartflowUpdated.InvokeAsync(_SelectedSmartflow);
            await _RefreshSmartflowItems.InvokeAsync("TickerMessages");
            
        }

        

        


        /// ##################################################################################
        /// Comparison:
        /// 
        /// Comparison methods are deatlt with in main Chapter pages as they 
        /// are used by multiple Detail Types (Docs, Agenda, Status, etc) and also required
        /// for the list to indicate which Smartflows have items that differ
        /// ##################################################################################


        private async Task CompareToAltSystem(bool _compareSystems)
        {

            try
            {
                if(_compareSystems == false)
                {
                    CompareSystems_ = false;    //CompareSystems is set within this method only
                                                //setting to false will not cause any display issues so no checks required
                                                //if setting to true then we must make sure that there is a corresponding 
                                                //Smartflow on the alt system before completing this, else the interface will temporarily
                                                //show items being synced (get spinning wheels) before this method switches it back to false 
                }
                else
                {
                    await UserSession.SwitchSelectedSystem();

                    bool gotLock = CompanyDbAccess.Lock;
                    while (gotLock)
                    {
                        await Task.Yield();
                        gotLock = CompanyDbAccess.Lock;
                    }

                    Alt_AppSmartflowRecord = await CompanyDbAccess.GetSmartflow(UserSession
                                                                ,_Selected_ClientSmartflowRecord.CaseTypeGroup
                                                                ,_Selected_ClientSmartflowRecord.CaseType
                                                                ,_Selected_ClientSmartflowRecord.SmartflowName
                                                                );
                    
                    await UserSession.ResetSelectedSystem();
                    
                    if(Alt_AppSmartflowRecord == null || Alt_AppSmartflowRecord.SmartflowData == null)
                    {
                        //Smartflow does not exist on Alt System 
                        await NotificationManager.ShowNotification("Warning", $"A corresponding Smartflow must exist on the {UserSession.AltSystem} system.").ConfigureAwait(false);
                        CompareSystems_ = false;
                    }
                    else
                    {
                        CompareSystems_ = true;
                        AltSmartflow = JsonConvert.DeserializeObject<Smartflow>(Alt_AppSmartflowRecord.SmartflowData);

                        Alt_AppSmartflowRecord.SmartflowData = JsonConvert.SerializeObject(AltSmartflow);
                        Alt_ClientSmartflowRecord = new Client_SmartflowRecord {
                            Id = Alt_AppSmartflowRecord.RowId
                            , SeqNo = Alt_AppSmartflowRecord.SeqNo
                            , CaseTypeGroup = Alt_AppSmartflowRecord.CaseTypeGroup
                            , CaseType = Alt_AppSmartflowRecord.CaseType
                            , SmartflowName = Alt_AppSmartflowRecord.SmartflowName
                            , SmartflowData = Alt_AppSmartflowRecord.SmartflowData
                        };
                        

                        Alt_AppSmartflowRecord.SmartflowData = JsonConvert.SerializeObject(AltSmartflow);
                        
                        var cItems = AltSmartflow.TickerMessages;

                        LstAltSystemItems = cItems.Select(T => new VmSmartflowMessage { Message = T, Compared = false }).ToList();
                        
                        foreach (var item in _LstMessages)
                        {
                            var altObject = LstAltSystemItems
                                        .Where(A => A.Message.Message == item.Message.Message)
                                        .FirstOrDefault();

                            if (altObject is null)
                            {
                                item.ComparisonResult = "No match";
                                item.ComparisonIcon = "times";
                            }
                            else
                            {
                                if (item.IsChapterItemMatch(altObject))
                                {
                                    item.ComparisonResult = "Exact match";
                                    item.ComparisonIcon = "check";

                                }
                                else
                                {
                                    item.ComparisonResult = "Partial match";
                                    item.ComparisonIcon = "exclamation";

                                }

                            }
                        }

                    }
                }
            }
            catch(Exception e)
            {
                GenericErrorLog(true,e, "CompareToAltSystem", $"Comparing systems: {e.Message}");
                
            }

            await InvokeAsync(() =>
            {
                StateHasChanged();
            });

        }

        protected void PrepareChapterSync()
        {
            string infoText;

            infoText = $"Make the {UserSession.AltSystem} system the same as {UserSession.SelectedSystem} for all Messages.";

            Action SelectedAction = HandleSmartflowSync;
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

        private async void HandleSmartflowSync()
        {
            await SyncChapter();
        }

        private async Task SyncChapter()
        {
            try
            {
                AltSmartflow.TickerMessages.Clear();
                
                AltSmartflow.TickerMessages.AddRange(_SelectedSmartflow.TickerMessages.ToList());

                Alt_AppSmartflowRecord.SmartflowData = JsonConvert.SerializeObject(AltSmartflow);
                Client_SmartflowRecord altSmartflow = new Client_SmartflowRecord {
                    Id = Alt_AppSmartflowRecord.RowId
                    , CaseTypeGroup = Alt_AppSmartflowRecord.CaseTypeGroup
                    , CaseType = Alt_AppSmartflowRecord.CaseType
                    , SmartflowName = Alt_AppSmartflowRecord.SmartflowName
                    , SmartflowData = Alt_AppSmartflowRecord.SmartflowData
                };

                await UserSession.SwitchSelectedSystem();

                await ClientApiManagementService.Update(altSmartflow);

                await UserSession.ResetSelectedSystem();

                await CompareToAltSystem(true);
            }
            catch(HttpRequestException)
            {
                //do nothing, already dealt with
            }
            catch(Exception e)
            {
                GenericErrorLog(false,e, "HandleSmartflowSync", e.Message);
            }
        }

        protected void ShowSmartflowComparisonModal(VmSmartflowMessage _selectedItem) 
        {
            try
            {

                EditObject = _selectedItem;

                Action action = HandleSmartflowComparison;

                var parameters = new ModalParameters();
                parameters.Add("_Object", EditObject);
                parameters.Add("_ComparisonRefresh", action);
                parameters.Add("_Alt_Smartflow", AltSmartflow);
                parameters.Add("_Alt_ClientSmartflowRecord", Alt_ClientSmartflowRecord);
                

                var options = new ModalOptions()
                {
                    Class = "blazored-custom-modal modal-smartflow-comparison"
                };

                Modal.Show<SmartflowMessageComparison>("Synchronise Smartflow Item", parameters, options);
            }
            catch(Exception e)
            {
                GenericErrorLog(false,e, "ShowSmartflowComparisonModal", e.Message);
            }

        }

        private async void HandleSmartflowComparison()
        {
            await CompareToAltSystem(true);
        }

        protected void ShowSmartflowDeleteAlt(VmSmartflowMessage _selectedItem)
        {
            try
            {
                EditObject = _selectedItem;
                
                Action SelectedDeleteAction = HandleAltDelete;
                var parameters = new ModalParameters();
                parameters.Add("_ItemName", _selectedItem.Message.Message);
                parameters.Add("_DeleteAction", SelectedDeleteAction);
                parameters.Add("_InfoText", $"Are you sure you wish to delete the Message from {UserSession.AltSystem} system?");

                var options = new ModalOptions()
                {
                    Class = "blazored-custom-modal"
                };

                Modal.Show<ModalSmartflowDetailDelete>($"Delete Message", parameters, options);
            }
            catch(Exception e)
            {
                GenericErrorLog(false,e, "ShowSmartflowDeleteAlt", e.Message);
            }

        }

        private async void HandleAltDelete() 
        {
            await DeleteAltItem();
        }

        private async Task DeleteAltItem()
        {
            await UserSession.SwitchSelectedSystem();

            AltSmartflow.TickerMessages.Remove(EditObject.Message);
            LstAltSystemItems.Remove(EditObject);

            Alt_AppSmartflowRecord.SmartflowData = JsonConvert.SerializeObject(AltSmartflow);
            Client_SmartflowRecord altSmartflow = new Client_SmartflowRecord {
                  Id = Alt_AppSmartflowRecord.RowId
                , CaseTypeGroup = Alt_AppSmartflowRecord.CaseTypeGroup
                , CaseType = Alt_AppSmartflowRecord.CaseType
                , SmartflowName = Alt_AppSmartflowRecord.SmartflowName
                , SmartflowData = Alt_AppSmartflowRecord.SmartflowData
            };
            await ClientApiManagementService.Update(altSmartflow); //saves to Company as well

            await UserSession.ResetSelectedSystem();

            await CompareToAltSystem(true);
        }

        /// ----------------------------------------------------------------------------------
        /// End Comparison:
        /// ----------------------------------------------------------------------------------
        

        /// <summary>
        /// Moves a sequecnce item up or down a list of type [GenSmartflowItem]
        /// </summary>
        /// <remarks>
        /// <para>Up: swaps the item with the preceding item in the lest by reducing sequence number by 1 </para>
        /// <para>Up: swaps the item with the following item in the lest by increasing sequence number by 1 </para>
        /// </remarks>
        /// <param name="selectobject">: current list item</param>
        /// <param name="direction">: Up or Down</param>
        /// <returns>No return</returns>
        protected async Task MoveSeq(SmartflowMessage _selectobject, string _direction)
        {
            try
            {
                SeqMoving = true; //prevents changes to the form whilst process of changing seq is carried out

                int incrementBy;

                incrementBy = (_direction.ToLower() == "up" ? -1 : 1);

                RowChanged = (int)(_selectobject.SeqNo + incrementBy);

                
                var swapItem = _LstMessages.Where(D => D.Message.SeqNo == (_selectobject.SeqNo + incrementBy)).SingleOrDefault();
                if (!(swapItem is null))
                {
                    _selectobject.SeqNo += incrementBy;
                    swapItem.Message.SeqNo = swapItem.Message.SeqNo + (incrementBy * -1);

                }

                SeqMoving = false;

                await ChapterItemsUpdated();

            }
            catch(Exception e)
            {
                GenericErrorLog(false, e, "MoveSeq", e.Message);
            }
            finally
            {
                SeqMoving = false;
            }

        }

        public async Task ReSequence(int _seq)
        {
            RowChanged = _seq;

            _LstMessages.Select(C => { C.Message.SeqNo = _LstMessages.IndexOf(C) + 1; return C; }).ToList(); //update all SeqNo by setting to the Index (+1)

            await ChapterItemsUpdated();
        }
   

        public void ResetRowChanged() 
        {
            RowChanged = 0;
            SeqMoving = false;

            StateHasChanged();

        }  


        /****************************************/
        /* ERROR HANDLING */
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
            using (LogContext.PushProperty("SourceContext", nameof(SmartflowMessageDetail)))
            {
                Logger.LogError(e,"Error - Method: {0}, Message: {1}",_method, _message);
            }

        }

        
    }
}