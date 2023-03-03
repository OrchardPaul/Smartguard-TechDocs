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
using GadjIT_App.Services.AppState;
using GadjIT_App.Services.SessionState;
using GadjIT_AppContext.GadjIT_App;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog.Context;
using GadjIT_ClientContext.Models.Smartflow;
using GadjIT_ClientContext.Models.P4W;
using GadjIT_ClientContext.Models.Smartflow.Client;

namespace GadjIT_App.Pages.Smartflows.ComponentsSmartflowDetail._DataView
{
    public partial class SmartflowDataViewDetail
    {

        [Parameter]
        public List<VmSmartflowDataView> _LstDataViews { get; set; } 

        [Parameter]
        public Client_SmartflowRecord _Selected_ClientSmartflowRecord { get; set; }

        [Parameter]
        public Smartflow _SelectedSmartflow { get; set; }

        [Parameter]
        public EventCallback<Smartflow> _SmartflowUpdated {get; set;}

        [Parameter]
        public EventCallback<string> _RefreshSmartflowItems {get; set;}


        [Parameter]
        public List<P4W_MpSysViews> _ListP4WViews {get; set;}

        [Parameter]
        public bool _SmartflowLockedForEdit {get; set;}
        



        [Inject]
        private ILogger<SmartflowDataViewDetail> Logger { get; set; }

        [Inject]
        public INotificationManager NotificationManager {get; set;}

        [Inject]
        private IClientApiManagementService ClientApiManagementService { get; set; }

        [Inject]
        private IPartnerAccessService PartnerAccessService { get; set; }

        [Inject]
        private ICompanyDbAccess CompanyDbAccess { get; set; }

        [Inject]
        public IAppSmartflowsState AppSmartflowsState { get; set; }

        [Inject]
        public IUserSessionState UserSession { get; set; }

        [Inject]
        IModalService Modal { get; set; }


        private App_SmartflowRecord Alt_AppSmartflowRecord {get; set;} //as saved on Company

        private Client_SmartflowRecord Alt_ClientSmartflowRecord { get; set; } = new Client_SmartflowRecord(); //as saved on client site with serialised VmSmartflow

        private Smartflow AltSmartflow {get; set;} //Smartflow Schema

        public List<VmSmartflowDataView> LstAltSystemItems { get; set; } 


        public VmSmartflowDataView EditObject = new VmSmartflowDataView { DataView = new SmartflowDataView() };

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


        // protected override async Task OnInitializedAsync()
        // {
        //     ListP4WViews = await PartnerAccessService.GetPartnerViews();
        // }

        
        protected void PrepareForInsert ()
        {
            try
            {

                EditObject = new VmSmartflowDataView { DataView = new SmartflowDataView() } ;

                if(_LstDataViews != null && _LstDataViews.Count > 0)
                {

                    EditObject.DataView.SeqNo = _LstDataViews
                                                        .OrderByDescending(D => D.DataView.SeqNo)
                                                        .Select(D => D.DataView.SeqNo)
                                                        .FirstOrDefault() + 1;
                }
                else
                {
                    EditObject.DataView.SeqNo = 1;
                }
                ShowSmartflowDataViewModal("Insert");
            }
            catch(Exception e)
            {
                GenericErrorLog(false, e, "PrepareForInsert", e.Message);
            }
        }


        protected void PrepareForEdit(VmSmartflowDataView _selectedObject)
        {
            EditObject = _selectedObject;

            ShowSmartflowDataViewModal("Edit");

        }

       

        protected void ShowSmartflowDataViewModal(string _option) 
        {
            try
            {
                Action dataChanged = HandleUpdate;

                SmartflowDataView copyObject = new SmartflowDataView
                {
                    SeqNo = EditObject.DataView.SeqNo,
                    DisplayName = EditObject.DataView.DisplayName,
                    ViewName = EditObject.DataView.ViewName
                };

                var parameters = new ModalParameters();
                parameters.Add("_Option", _option);
                parameters.Add("_TaskObject", EditObject.DataView);
                parameters.Add("_CopyObject", copyObject);
                parameters.Add("_SelectedSmartflow", _SelectedSmartflow);
                parameters.Add("_Selected_ClientSmartflowRecord", _Selected_ClientSmartflowRecord);
                parameters.Add("_DataChanged", dataChanged);
                parameters.Add("_ListP4WViews", _ListP4WViews);

                var options = new ModalOptions()
                {
                    Class = "blazored-custom-modal modal-smartflow-data"
                };

                Modal.Show<ModalSmartflowDataViewDetail>("Data View", parameters, options);
            }
            catch(Exception e)
            {
                GenericErrorLog(false,e, "ShowSmartflowDataViewModal", e.Message);
            }


        }

        private async void HandleUpdate()
        {
            await ChapterItemsUpdated();
        }

        protected void ShowSmartflowDataViewDisplayModal(VmSmartflowDataView _selectedObject)//moved partial
        {
            try
            {
                var parameters = new ModalParameters();
                parameters.Add("_Object", _selectedObject);

                var options = new ModalOptions()
                {
                    Class = "blazored-custom-modal modal-smartflow-fees"
                };

                Modal.Show<ModalSmartflowDataViewDisplay>("Data View", parameters, options);
            }
            catch(Exception e)
            {
                GenericErrorLog(false,e, "ShowSmartflowDataViewDisplayModal", e.Message);
            }
        }

        protected void ShowSmartflowDataViewDelete(VmSmartflowDataView _SelectedSmartflowItem) 
        {
            EditObject = _SelectedSmartflowItem;

            string itemName = _SelectedSmartflowItem.DataView.ViewName;
            string itemType = "Data View";

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
            _SelectedSmartflow.DataViews.Remove(EditObject.DataView);
           
            await ChapterItemsUpdated();

        }
        
        private async Task ChapterItemsUpdated()
        {
            await _SmartflowUpdated.InvokeAsync(_SelectedSmartflow);
            await _RefreshSmartflowItems.InvokeAsync("DataViews");
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
                        
                        var cItems = AltSmartflow.DataViews;

                        LstAltSystemItems = cItems.Select(T => new VmSmartflowDataView { DataView = T, Compared = false }).ToList();
                        
                        foreach (var item in _LstDataViews)
                        {
                            var altObject = LstAltSystemItems
                                        .Where(A => A.DataView.DisplayNameOrViewName == item.DataView.DisplayNameOrViewName)
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

            infoText = $"Make the {UserSession.AltSystem} system the same as {UserSession.SelectedSystem} for all Data Views.";

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
                AltSmartflow.DataViews.Clear();
                
                AltSmartflow.DataViews.AddRange(_SelectedSmartflow.DataViews.ToList());

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

        protected void ShowSmartflowComparisonModal(VmSmartflowDataView _selectedItem) 
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

                Modal.Show<SmartflowDataViewComparison>("Synchronise Smartflow Item", parameters, options);
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

        protected void ShowSmartflowDeleteAlt(VmSmartflowDataView _selectedItem)
        {
            try
            {
                EditObject = _selectedItem;
                
                Action SelectedDeleteAction = HandleAltDelete;
                var parameters = new ModalParameters();
                parameters.Add("_ItemName", _selectedItem.DataView.DisplayName);
                parameters.Add("_DeleteAction", SelectedDeleteAction);
                parameters.Add("_InfoText", $"Are you sure you wish to delete the '{_selectedItem.DataView.DisplayName}' Data View from {UserSession.AltSystem} system?");

                var options = new ModalOptions()
                {
                    Class = "blazored-custom-modal"
                };

                Modal.Show<ModalSmartflowDetailDelete>($"Delete Data View", parameters, options);
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

            AltSmartflow.DataViews.Remove(EditObject.DataView);
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
        protected async Task MoveSeq(SmartflowDataView _selectobject, string _direction)
        {
            try
            {
                SeqMoving = true; //prevents changes to the form whilst process of changing seq is carried out

                int incrementBy;

                incrementBy = (_direction.ToLower() == "up" ? -1 : 1);

                RowChanged = (int)(_selectobject.SeqNo + incrementBy);

                
                var swapItem = _LstDataViews.Where(D => D.DataView.SeqNo == (_selectobject.SeqNo + incrementBy)).SingleOrDefault();
                if (!(swapItem is null))
                {
                    _selectobject.SeqNo += incrementBy;
                    swapItem.DataView.SeqNo = swapItem.DataView.SeqNo + (incrementBy * -1);

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

            _LstDataViews.Select(C => { C.DataView.SeqNo = _LstDataViews.IndexOf(C) + 1; return C; }).ToList(); //update all SeqNo by setting to the Index (+1)

            await ChapterItemsUpdated();
        }
   

        public void ResetRowChanged() 
        {
            RowChanged = 0;
            SeqMoving = false;

        }  


        /****************************************/
        /* ERROR HANDLING */
        /****************************************/
        private async void GenericErrorLog(bool _showNotificationMsg, Exception e, string _method, string _message)
        {
            using (LogContext.PushProperty("SourceSystem", UserSession.SelectedSystem))
            using (LogContext.PushProperty("SourceCompanyId", UserSession.Company.Id))
            using (LogContext.PushProperty("SourceUserId", UserSession.User.Id))
            using (LogContext.PushProperty("SourceContext", nameof(SmartflowDataViewDetail)))
            {
                Logger.LogError(e,"Error - Method: {0}, Message: {1}",_method, _message);
            }

            if(_showNotificationMsg)
            {
                await NotificationManager.ShowNotification("Danger", $"Oops! Something went wrong.").ConfigureAwait(false);
            }
        }

        
    }
}