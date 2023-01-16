using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Blazored.Modal;
using Blazored.Modal.Services;
using GadjIT_App.Data.Admin;
using GadjIT_App.Pages.Chapters.ComponentsChapterDetail._SharedItems;
using GadjIT_App.Pages._Shared.Modals;
using GadjIT_App.Services;
using GadjIT_App.Services.AppState;
using GadjIT_App.Services.SessionState;
using GadjIT_AppContext.GadjIT_App;
using GadjIT_ClientContext.P4W;
using GadjIT_ClientContext.P4W.Custom;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog.Context;

namespace GadjIT_App.Pages.Chapters.ComponentsChapterDetail._DataView
{
    public partial class ChapterDataViewDetail
    {

        [Parameter]
        public List<VmDataView> _LstDataViews { get; set; } 

        [Parameter]
        public UsrOrsfSmartflows _SelectedChapterObject { get; set; }

        [Parameter]
        public VmChapter _SelectedChapter { get; set; }

        [Parameter]
        public EventCallback<string> _RefreshChapterItems {get; set;}

        [Parameter]
        public bool _SeqMoving {get; set;}

        [Parameter]
        public List<MpSysViews> _ListP4WViews {get; set;}



        [Inject]
        private ILogger<ChapterDataViewDetail> Logger { get; set; }

        [Inject]
        public INotificationManager NotificationManager {get; set;}

        [Inject]
        private IChapterManagementService ChapterManagementService { get; set; }

        [Inject]
        private IPartnerAccessService PartnerAccessService { get; set; }

        [Inject]
        private ICompanyDbAccess CompanyDbAccess { get; set; }

        [Inject]
        public IAppChapterState AppChapterState { get; set; }

        [Inject]
        public IUserSessionState UserSession { get; set; }

        [Inject]
        IModalService Modal { get; set; }


        private SmartflowRecords AltChapterRecord {get; set;} //as saved on Company

        private UsrOrsfSmartflows AltChapterObject { get; set; } = new UsrOrsfSmartflows(); //as saved on client site with serialised VmChapter

        private VmChapter AltChapter {get; set;} //Smartflow Schema

        public List<VmDataView> LstAltSystemItems { get; set; } 


        public VmDataView EditObject = new VmDataView { DataView = new DataView() };


        private int RowChanged = 0;
        private bool compareSystems;
        protected bool CompareSystems 
                {
                    get{ return compareSystems;} 
                    set
                    {
                        compareSystems = value;
                        _ = CompareToAltSystem();
                    }
                }


        // protected override async Task OnInitializedAsync()
        // {
        //     ListP4WViews = await PartnerAccessService.GetPartnerViews();
        // }

        protected override void OnParametersSet()
        {
            ReSequence();
        }

        protected void PrepareForInsert ()
        {
            try
            {

                EditObject = new VmDataView { DataView = new DataView() } ;

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
                ShowChapterDataViewModal("Insert");
            }
            catch(Exception e)
            {
                GenericErrorLog(false, e, "PrepareForInsert", e.Message);
            }
        }


        protected void PrepareForEdit(VmDataView _selectedObject)
        {
            EditObject = _selectedObject;

            ShowChapterDataViewModal("Edit");

        }

       

        protected void ShowChapterDataViewModal(string _option) 
        {
            try
            {
                Action dataChanged = HandleUpdate;

                DataView copyObject = new DataView
                {
                    SeqNo = EditObject.DataView.SeqNo,
                    DisplayName = EditObject.DataView.DisplayName,
                    ViewName = EditObject.DataView.ViewName
                };

                var parameters = new ModalParameters();
                parameters.Add("_Option", _option);
                parameters.Add("_TaskObject", EditObject.DataView);
                parameters.Add("_CopyObject", copyObject);
                parameters.Add("_SelectedChapter", _SelectedChapter);
                parameters.Add("_SelectedChapterObject", _SelectedChapterObject);
                parameters.Add("_DataChanged", dataChanged);
                parameters.Add("_ListP4WViews", _ListP4WViews);

                var options = new ModalOptions()
                {
                    Class = "blazored-custom-modal modal-chapter-data"
                };

                Modal.Show<ModalChapterDataViewDetail>("Data View", parameters, options);
            }
            catch(Exception e)
            {
                GenericErrorLog(false,e, "ShowChapterDataViewModal", e.Message);
            }


        }

        private async void HandleUpdate()
        {
            await RefreshSelectedList();
        }

        protected void ShowChapterDataViewDisplayModal(VmDataView _selectedObject)//moved partial
        {
            try
            {
                var parameters = new ModalParameters();
                parameters.Add("_Object", _selectedObject);

                var options = new ModalOptions()
                {
                    Class = "blazored-custom-modal modal-chapter-fees"
                };

                Modal.Show<ModalChapterDataViewDisplay>("Data View", parameters, options);
            }
            catch(Exception e)
            {
                GenericErrorLog(false,e, "ShowChapterDataViewDisplayModal", e.Message);
            }
        }

        protected void ShowChapterDataViewDelete(VmDataView _selectedChapterItem) 
        {
            EditObject = _selectedChapterItem;

            string itemName = _selectedChapterItem.DataView.ViewName;
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

            Modal.Show<ModalChapterDetailDelete>($"Delete {itemType}", parameters, options);

        }

        private async void HandleDelete()
        {
            await DeleteItem();
        }

        private async Task DeleteItem()
        {
            //<ModalDelete> simply invokes this method when user clicks OK. No need for the modal to handle this action as we do not require any details from the Modal. 
            _SelectedChapter.DataViews.Remove(EditObject.DataView);
            _SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(_SelectedChapter);
            await ChapterManagementService.Update(_SelectedChapterObject);

            //keep track of time last updated ready for comparison by other sessions checking for updates
            AppChapterState.SetLastUpdated(UserSession, _SelectedChapter);

            await RefreshSelectedList();

        }
        
        private async Task RefreshSelectedList()
        {
            await _RefreshChapterItems.InvokeAsync("DataViews");
        }

        

        


        /// ##################################################################################
        /// Comparison:
        /// 
        /// Comparison methods are deatlt with in main Chapter pages as they 
        /// are used by multiple Detail Types (Docs, Agenda, Status, etc) and also required
        /// for the list to indicate which Smartflows have items that differ
        /// ##################################################################################

        

        private async Task CompareToAltSystem()
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

                AltChapterRecord = await CompanyDbAccess.GetSmartflow(UserSession
                                                            ,_SelectedChapterObject.CaseTypeGroup
                                                            ,_SelectedChapterObject.CaseType
                                                            ,_SelectedChapterObject.SmartflowName
                                                            );
                
                await UserSession.ResetSelectedSystem();
                
                if(AltChapterRecord == null || AltChapterRecord.SmartflowData == null)
                {
                    //Smartflow does not exist on Alt System 
                    await NotificationManager.ShowNotification("Warning", $"A corresponding Smartflow must exist on the {UserSession.AltSystem} system.").ConfigureAwait(false);
                    CompareSystems = false;
                }
                else
                {
                    AltChapter = JsonConvert.DeserializeObject<VmChapter>(AltChapterRecord.SmartflowData);

                    AltChapterRecord.SmartflowData = JsonConvert.SerializeObject(AltChapter);
                    AltChapterObject = new UsrOrsfSmartflows {
                        Id = AltChapterRecord.RowId
                        , SeqNo = AltChapterRecord.SeqNo
                        , CaseTypeGroup = AltChapterRecord.CaseTypeGroup
                        , CaseType = AltChapterRecord.CaseType
                        , SmartflowName = AltChapterRecord.SmartflowName
                        , SmartflowData = AltChapterRecord.SmartflowData
                    };
                    
                    AltChapterRecord.SmartflowData = JsonConvert.SerializeObject(AltChapter);
                    
                    var cItems = AltChapter.DataViews;

                    LstAltSystemItems = cItems.Select(T => new VmDataView { DataView = T, Compared = false }).ToList();
                    
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
            catch(Exception e)
            {
                GenericErrorLog(true,e, "CompareToAltSystem", $"Comparing systems: {e.Message}");
                
                //return false;
            }

            await InvokeAsync(() =>
            {
                StateHasChanged();
            });


            //return true;
        }

        protected void PrepareChapterSync()
        {
            string infoText;

            infoText = $"Make the {UserSession.AltSystem} system the same as {UserSession.SelectedSystem} for all Data Views.";

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

        private async void HandleChapterSync()
        {
            await SyncChapter();
        }

        private async Task SyncChapter()
        {
            try
            {
                AltChapter.DataViews.Clear();
                
                AltChapter.DataViews.AddRange(_SelectedChapter.DataViews.ToList());

                AltChapterRecord.SmartflowData = JsonConvert.SerializeObject(AltChapter);
                UsrOrsfSmartflows altSmartflow = new UsrOrsfSmartflows {
                    Id = AltChapterRecord.RowId
                    , CaseTypeGroup = AltChapterRecord.CaseTypeGroup
                    , CaseType = AltChapterRecord.CaseType
                    , SmartflowName = AltChapterRecord.SmartflowName
                    , SmartflowData = AltChapterRecord.SmartflowData
                };

                await UserSession.SwitchSelectedSystem();

                await ChapterManagementService.Update(altSmartflow);

                await UserSession.ResetSelectedSystem();

                await CompareToAltSystem();
            }
            catch(HttpRequestException)
            {
                //do nothing, already dealt with
            }
            catch(Exception e)
            {
                GenericErrorLog(false,e, "HandleChapterSync", e.Message);
            }
        }

        protected void ShowChapterComparisonModal(VmDataView _selectedItem) 
        {
            try
            {

                EditObject = _selectedItem;

                Action action = HandleChapterComparison;

                var parameters = new ModalParameters();
                parameters.Add("_Object", EditObject);
                parameters.Add("_ComparisonRefresh", action);
                parameters.Add("_AltChapter", AltChapter);
                parameters.Add("_AltChapterRow", AltChapterObject);
                

                var options = new ModalOptions()
                {
                    Class = "blazored-custom-modal modal-chapter-comparison"
                };

                Modal.Show<ChapterDataViewComparison>("Synchronise Smartflow Item", parameters, options);
            }
            catch(Exception e)
            {
                GenericErrorLog(false,e, "ShowChapterComparisonModal", e.Message);
            }

        }

        private async void HandleChapterComparison()
        {
            await CompareToAltSystem();
        }

        protected void ShowChapterDeleteAlt(VmDataView _selectedItem)
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

                Modal.Show<ModalChapterDetailDelete>($"Delete Data View", parameters, options);
            }
            catch(Exception e)
            {
                GenericErrorLog(false,e, "ShowChapterDeleteAlt", e.Message);
            }

        }

        private async void HandleAltDelete() 
        {
            await DeleteAltItem();
        }

        private async Task DeleteAltItem()
        {
            await UserSession.SwitchSelectedSystem();

            AltChapter.DataViews.Remove(EditObject.DataView);
            LstAltSystemItems.Remove(EditObject);

            AltChapterRecord.SmartflowData = JsonConvert.SerializeObject(AltChapter);
            UsrOrsfSmartflows altSmartflow = new UsrOrsfSmartflows {
                  Id = AltChapterRecord.RowId
                , CaseTypeGroup = AltChapterRecord.CaseTypeGroup
                , CaseType = AltChapterRecord.CaseType
                , SmartflowName = AltChapterRecord.SmartflowName
                , SmartflowData = AltChapterRecord.SmartflowData
            };
            await ChapterManagementService.Update(altSmartflow); //saves to Company as well

            await UserSession.ResetSelectedSystem();

            await CompareToAltSystem();
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
        protected async void MoveSeq(DataView _selectobject, string _direction)
        {
            await MoveSeqTask(_selectobject, _direction);
        }

        protected async Task MoveSeqTask(DataView _selectobject, string _direction)
        {
            try
            {
                _SeqMoving = true; //prevents changes to the form whilst process of changing seq is carried out

                int incrementBy;

                incrementBy = (_direction.ToLower() == "up" ? -1 : 1);

                RowChanged = (int)(_selectobject.SeqNo + incrementBy);

                
                var swapItem = _LstDataViews.Where(D => D.DataView.SeqNo == (_selectobject.SeqNo + incrementBy)).SingleOrDefault();
                if (!(swapItem is null))
                {
                    _selectobject.SeqNo += incrementBy;
                    swapItem.DataView.SeqNo = swapItem.DataView.SeqNo + (incrementBy * -1);

                    _SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(_SelectedChapter);
                    await ChapterManagementService.Update(_SelectedChapterObject);


                    //keep track of time last updated ready for comparison by other sessions checking for updates
                    AppChapterState.SetLastUpdated(UserSession, _SelectedChapter);

                }

                await _RefreshChapterItems.InvokeAsync("DataViews");
                
            }
            catch(Exception e)
            {
                GenericErrorLog(false, e, "MoveSeq", e.Message);
            }
            finally
            {
                _SeqMoving = false;
            }

        }

        public void ReSequence(int _seq)
        {
            RowChanged = _seq;

            ReSequence();
        }

        public void ReSequence()
        {
            try
            {
                if(_LstDataViews.Select(C => C.DataView.SeqNo != _LstDataViews.IndexOf(C) + 1).Count() > 0) //If any SeqNos are out of sequence
                { 
                    _LstDataViews.Select(C => { C.DataView.SeqNo = _LstDataViews.IndexOf(C) + 1; return C; }).ToList(); //update all SeqNo by setting to the Index (+1)

                    _SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(_SelectedChapter);
                    ChapterManagementService.Update(_SelectedChapterObject).ConfigureAwait(false);

                    StateHasChanged();
                }
            }
            catch
            {
                
            }
        }      

        public void ResetRowChanged() 
        {
            RowChanged = 0;
        }  


        /****************************************/
        /* ERROR HANDLING */
        /****************************************/
        private async void GenericErrorLog(bool _showNotificationMsg, Exception e, string _method, string _message)
        {
            using (LogContext.PushProperty("SourceSystem", UserSession.SelectedSystem))
            using (LogContext.PushProperty("SourceCompanyId", UserSession.Company.Id))
            using (LogContext.PushProperty("SourceUserId", UserSession.User.Id))
            using (LogContext.PushProperty("SourceContext", nameof(ChapterDataViewDetail)))
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