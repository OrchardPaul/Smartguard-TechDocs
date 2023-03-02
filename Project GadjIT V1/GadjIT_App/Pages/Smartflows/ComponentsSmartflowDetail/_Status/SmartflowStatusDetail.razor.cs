using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Blazored.Modal;
using Blazored.Modal.Services;
using GadjIT_App.Data.Admin;
using GadjIT_App.Pages.Smartflows.ComponentsSmartflowDetail._SharedItems;
using GadjIT_App.Pages._Shared.Modals;
using GadjIT_App.Services;
using GadjIT_App.Services.SessionState;
using GadjIT_AppContext.GadjIT_App;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog.Context;
using GadjIT_ClientContext.Models.Smartflow;
using GadjIT_ClientContext.Models.Smartflow.Client;

namespace GadjIT_App.Pages.Smartflows.ComponentsSmartflowDetail._Status
{
    public partial class SmartflowStatusDetail
    {

        [Parameter]
        public List<VmGenSmartflowItem> _LstStatus { get; set; } 


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
        private ILogger<SmartflowStatusDetail> Logger { get; set; }

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

        public List<VmGenSmartflowItem> LstAltSystemItems { get; set; } 

        public VmGenSmartflowItem EditObject = new VmGenSmartflowItem { ChapterObject = new GenSmartflowItem() };

        public bool SeqMoving {get; set;}

        private int RowChanged = 0;
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


        protected void PrepareForInsert() 
        {
            try
            {
                
                EditObject = new VmGenSmartflowItem { ChapterObject = new GenSmartflowItem() };
                EditObject.ChapterObject.Type = "Status";

                
                if(_LstStatus != null && _LstStatus.Count > 0)
                {
                    EditObject.ChapterObject.SeqNo = _LstStatus
                                                            .OrderByDescending(A => A.ChapterObject.SeqNo)
                                                            .Select(A => A.ChapterObject.SeqNo)
                                                            .FirstOrDefault() + 1;
                }
                else
                {
                    EditObject.ChapterObject.SeqNo = 1;
                }
                

                ShowSmartflowDetailModal("Insert");
                
            }
            catch(Exception e)
            {
                GenericErrorLog(false, e, "PrepareForInsert", e.Message);
            }
            
        }


        protected void PrepareForEdit(VmGenSmartflowItem _selectedObject)
        {
            EditObject = _selectedObject;

            ShowSmartflowDetailModal("Edit");

        }


        protected void ShowSmartflowDetailModal(string _option) 
        {
            try
            {

                Action refreshSelectedList = HandleUpdate;
                
                var copyObject = new GenSmartflowItem
                {
                    Type = EditObject.ChapterObject.Type,
                    Name = EditObject.ChapterObject.Name,
                    SeqNo = EditObject.ChapterObject.SeqNo,
                    SuppressStep = EditObject.ChapterObject.SuppressStep,
                    MilestoneStatus = EditObject.ChapterObject.MilestoneStatus,
                };

                var parameters = new ModalParameters();
                parameters.Add("_Option", _option);
                parameters.Add("_Selected_ClientSmartflowRecord", _Selected_ClientSmartflowRecord);
                parameters.Add("_SelectedSmartflow", _SelectedSmartflow);
                parameters.Add("_TaskObject", EditObject.ChapterObject);
                parameters.Add("_CopyObject", copyObject);
                parameters.Add("_DataChanged", refreshSelectedList);
                parameters.Add("_ListOfStatus", _LstStatus);

                var options = new ModalOptions()
                {
                    Class = "blazored-custom-modal modal-smartflow-status" 
                };

                Modal.Show<ModalSmartflowDetail>("Status", parameters, options);
            }
            catch(Exception e)
            {
                GenericErrorLog(false,e, "ShowSmartflowDetailModal", e.Message);
            }

        }

        private async void HandleUpdate()
        {
            
            await ChapterItemsUpdated();

        }


        protected void ShowSmartflowDetailViewModal(VmGenSmartflowItem _selectedObject)//moved partial
        {
            try
            {
                var parameters = new ModalParameters();
                parameters.Add("_Object", _selectedObject);
                parameters.Add("_SelectedList", "Status"); 

                var options = new ModalOptions()
                {
                    Class = "blazored-custom-modal modal-smartflow-status"
                };

                Modal.Show<ModalSmartflowDetailView>("Status", parameters, options);
            }
            catch(Exception e)
            {
                GenericErrorLog(false,e, "ShowSmartflowDetailViewModal", e.Message);
            }
        }

        protected void ShowSmartflowDetailDelete(VmGenSmartflowItem _SelectedSmartflowItem) 
        {
            EditObject = _SelectedSmartflowItem;

            string itemName = _SelectedSmartflowItem.ChapterObject.Name;
            string itemType = _SelectedSmartflowItem.ChapterObject.Type;

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
            _SelectedSmartflow.Items.Remove(EditObject.ChapterObject);

            await ChapterItemsUpdated();
            
        }


        
        private async Task ChapterItemsUpdated()
        {
            await _SmartflowUpdated.InvokeAsync(_SelectedSmartflow);
            await _RefreshSmartflowItems.InvokeAsync("Status");

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

                        var cItems = AltSmartflow.Items;

                        LstAltSystemItems = cItems.Select(T => new VmGenSmartflowItem { ChapterObject = T, Compared = false }).ToList();
                        
                        foreach (var item in _LstStatus)
                        {
                            var altObject = LstAltSystemItems
                                        .Where(A => A.ChapterObject.Type == item.ChapterObject.Type)
                                        .Where(A => A.ChapterObject.Name == item.ChapterObject.Name)
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

            infoText = $"Make the {UserSession.AltSystem} system the same as {UserSession.SelectedSystem} for all items.";

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
                foreach (var item in AltSmartflow.Items.Where(I => I.Type == "Status").ToList())
                {
                    AltSmartflow.Items.Remove(item);
                }

                AltSmartflow.Items.AddRange(_SelectedSmartflow.Items.Where(I => I.Type == "Status").ToList());

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


        private void ShowSmartflowComparisonModal(VmGenSmartflowItem _selectedItem) 
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

                Modal.Show<SmartflowStatusComparison>("Synchronise Smartflow Item", parameters, options);
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

        protected void ShowSmartflowDeleteAlt(VmGenSmartflowItem _selectedItem)
        {
            try
            {
                EditObject = _selectedItem;
                
                Action SelectedDeleteAction = HandleAltDelete;
                var parameters = new ModalParameters();
                parameters.Add("_ItemName", _selectedItem.ChapterObject.Name);
                parameters.Add("_DeleteAction", SelectedDeleteAction);
                parameters.Add("_InfoText", $"Are you sure you wish to delete the '{_selectedItem.ChapterObject.Name}' item from {UserSession.AltSystem} system?");

                var options = new ModalOptions()
                {
                    Class = "blazored-custom-modal"
                };

                Modal.Show<ModalSmartflowDetailDelete>($"Delete Status", parameters, options);
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

            AltSmartflow.Items.Remove(EditObject.ChapterObject);
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
        /// <param name="listType">: Docs or Fees</param>
        /// <param name="direction">: Up or Down</param>
        /// <returns>No return</returns>
        protected async Task MoveSeq(GenSmartflowItem _selectobject, string _direction)
        {
            try
            {
                SeqMoving = true; //prevents changes to the form whilst process of changing seq is carried out

                int incrementBy;

                incrementBy = (_direction.ToLower() == "up" ? -1 : 1);

                RowChanged = (int)(_selectobject.SeqNo + incrementBy);

                
                var swapItem = _LstStatus.Where(D => D.ChapterObject.SeqNo == (_selectobject.SeqNo + incrementBy)).SingleOrDefault();
                if (!(swapItem is null))
                {
                    _selectobject.SeqNo += incrementBy;
                    swapItem.ChapterObject.SeqNo = swapItem.ChapterObject.SeqNo + (incrementBy * -1);

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

            _LstStatus.Select(C => { C.ChapterObject.SeqNo = _LstStatus.IndexOf(C) + 1; return C; }).ToList();

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
            using (LogContext.PushProperty("SourceContext", nameof(SmartflowStatusDetail)))
            {
                Logger.LogError(e,"Error - Method: {0}, Message: {1}",_method, _message);
            }

        }

        
    }
}