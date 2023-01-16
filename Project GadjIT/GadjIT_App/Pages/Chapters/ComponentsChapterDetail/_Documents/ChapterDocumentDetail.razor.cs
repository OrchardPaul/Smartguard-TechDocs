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

namespace GadjIT_App.Pages.Chapters.ComponentsChapterDetail._Documents
{
    public partial class ChapterDocumentDetail
    {


        [Parameter]
        public List<VmGenSmartflowItem> _LstDocs { get; set; } 

        [Parameter]
        public List<VmGenSmartflowItem> _LstStatus { get; set; } 

        [Parameter]
        public UsrOrsfSmartflows _SelectedChapterObject { get; set; }

        [Parameter]
        public VmChapter _SelectedChapter { get; set; }


        [Parameter]
        public LinkedItems _AttachObject {get; set;}


        [Parameter]
        public EventCallback<string> _RefreshChapterItems {get; set;}

        [Parameter]
        public bool _SeqMoving {get; set;}

        [Parameter]
        public List<CaseTypeGroups> _P4WCaseTypeGroups {get; set;}


        [Parameter]
        public List<DmDocuments> _LibraryDocumentsAndSteps {get; set;}
        
        [Parameter]
        public List<VmGenSmartflowItem> _LstAgendas { get; set; }

        [Parameter]
        public List<TableDate> _TableDates {get; set;}

                


        [Inject]
        private ILogger<ChapterDocumentDetail> Logger { get; set; }

        [Inject]
        public INotificationManager NotificationManager {get; set;}

        [Inject]
        private IChapterManagementService ChapterManagementService { get; set; }

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

        public List<VmGenSmartflowItem> LstAltSystemItems { get; set; } 

        private VmGenSmartflowItem EditObject {get; set;}
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
        
        private string SelectedList = "";

        
        protected override void OnParametersSet()
        {
            ReSequence();
        }

        
        protected void PrepareForInsert() 
        {
            try
            {
                
                EditObject = new VmGenSmartflowItem { ChapterObject = new GenSmartflowItem() };
                EditObject.ChapterObject.Type = "Doc";
                EditObject.ChapterObject.Action = "INSERT"; //default item to INSERT, user can opt for a TAKE on the form if required
                
                if(_LstDocs != null && _LstDocs.Count > 0)
                {
                    EditObject.ChapterObject.SeqNo = _LstDocs
                                                            .OrderByDescending(A => A.ChapterObject.SeqNo)
                                                            .Select(A => A.ChapterObject.SeqNo)
                                                            .FirstOrDefault() + 1;
                }
                else
                {
                    EditObject.ChapterObject.SeqNo = 1;
                }

                
                ShowChapterDetailModal("Insert");
                
            }
            catch(Exception e)
            {
                GenericErrorLog(false, e, "PrepareForInsert", e.Message);
            }
            
        }


        protected void PrepareForEdit(VmGenSmartflowItem _selectedObject)
        {
            EditObject = _selectedObject;

            ShowChapterDetailModal("Edit");

        }

        protected void ShowChapterDetailModal(string _option) //moved partial
        {
            try
            {

                Action refreshSelectedList = HandleUpdate;
                Action refreshLibraryDocumentsAndSteps = RefreshLibraryDocumentsAndSteps;

                var copyObject = new GenSmartflowItem
                {
                    Type = EditObject.ChapterObject.Type,
                    Name = EditObject.ChapterObject.Name,
                    EntityType = EditObject.ChapterObject.EntityType,
                    SeqNo = EditObject.ChapterObject.SeqNo,
                    SuppressStep = EditObject.ChapterObject.SuppressStep,
                    CompleteName = EditObject.ChapterObject.CompleteName,
                    AsName = EditObject.ChapterObject.AsName,
                    RescheduleDays = EditObject.ChapterObject.RescheduleDays,
                    AltDisplayName = EditObject.ChapterObject.AltDisplayName,
                    UserMessage = EditObject.ChapterObject.UserMessage,
                    PopupAlert = EditObject.ChapterObject.PopupAlert,
                    NextStatus = EditObject.ChapterObject.NextStatus,
                    Action = EditObject.ChapterObject.Action,
                    TrackingMethod = EditObject.ChapterObject.TrackingMethod,
                    ChaserDesc = EditObject.ChapterObject.ChaserDesc,
                    RescheduleDataItem = EditObject.ChapterObject.RescheduleDataItem,
                    MilestoneStatus = EditObject.ChapterObject.MilestoneStatus,
                    OptionalDocument = EditObject.ChapterObject.OptionalDocument,
                    Agenda = EditObject.ChapterObject.Agenda,
                    CustomItem = EditObject.ChapterObject.CustomItem
            };

                var parameters = new ModalParameters();
                parameters.Add("_Option", _option);
                parameters.Add("_SelectedChapter", _SelectedChapter);
                parameters.Add("_SelectedChapterObject", _SelectedChapterObject);
                parameters.Add("_TaskObject", EditObject.ChapterObject);
                parameters.Add("_CopyObject", copyObject);
                parameters.Add("_DataChanged", refreshSelectedList);
                parameters.Add("_LibraryDocumentsAndSteps", _LibraryDocumentsAndSteps);
                parameters.Add("_TableDates", _TableDates);
                parameters.Add("_P4WCaseTypeGroups", _P4WCaseTypeGroups);
                parameters.Add("_ListOfStatus", _LstStatus);
                parameters.Add("_ListOfAgenda", _LstAgendas);

                var options = new ModalOptions()
                {
                    Class = "blazored-custom-modal modal-chapter-doc" 
                };

                Modal.Show<ModalChapterDetail>("Steps and Documents", parameters, options);
            }
            catch(Exception e)
            {
                GenericErrorLog(false,e, "ShowChapterDetailModal", e.Message);
            }

        }

        private async void HandleUpdate()
        {
            await RefreshSelectedList();
        }

        protected void ShowChapterDetailViewModal(VmGenSmartflowItem _selectedObject)//moved partial
        {
            try
            {
                var parameters = new ModalParameters();
                parameters.Add("_Object", _selectedObject);
                parameters.Add("_SelectedList", "Steps and Documents"); 

                var options = new ModalOptions()
                {
                    Class = "blazored-custom-modal modal-chapter-comparison"
                };

                Modal.Show<ModalChapterDetailView>("Steps and Documents", parameters, options);
            }
            catch(Exception e)
            {
                GenericErrorLog(false,e, "ShowChapterDetailViewModal", e.Message);
            }
        }

        protected void ShowChapterDetailDelete(VmGenSmartflowItem _selectedChapterItem) 
        {
            EditObject = _selectedChapterItem;

            string itemName = (string.IsNullOrEmpty(_selectedChapterItem.ChapterObject.AltDisplayName) ? _selectedChapterItem.ChapterObject.Name : _selectedChapterItem.ChapterObject.AltDisplayName);
            string itemType = _selectedChapterItem.ChapterObject.Type;

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
            //<ModalDelete> simply invokes this method when user cicks OK. No need for the modal to handle this action as we do not require any details from the Modal. 
            _SelectedChapter.Items.Remove(EditObject.ChapterObject);
            _SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(_SelectedChapter);
            await ChapterManagementService.Update(_SelectedChapterObject);

            //keep track of time last updated ready for comparison by other sessions checking for updates
            AppChapterState.SetLastUpdated(UserSession, _SelectedChapter);

            await RefreshSelectedList();

        }

        private async Task RefreshSelectedList()
        {
            await _RefreshChapterItems.InvokeAsync("Docs");

        }

        

        /// ##################################################################################
        /// Attachments:
        /// 
        /// ##################################################################################
    
        private void PrepareAttachmentForAdd(VmGenSmartflowItem _item)
        {
            SelectedList = "New Attachement";
            EditObject = _item;
            _AttachObject = null;

            ShowChapterAttachmentModal();
        }

        private void PrepareAttachmentForEdit(VmGenSmartflowItem _item, LinkedItems _linkedItems)
        {
            SelectedList = "Edit Attachement";
            EditObject = _item;
            _AttachObject = _linkedItems;

            ShowChapterAttachmentModal();
        }

        private void PrepareAttachmentForView(VmGenSmartflowItem _item, LinkedItems _linkedItems)
        {
            SelectedList = "Edit Attachement";
            EditObject = _item;
            _AttachObject = _linkedItems;

            ShowChapterAttachmentViewModal();
        }
       

        protected void ShowChapterAttachmentModal()
        {
            Action refreshSelectedList = HandleUpdate;
            Action refreshLibraryDocumentsAndSteps = RefreshLibraryDocumentsAndSteps;

            var copyObject = new GenSmartflowItem
            {
                Type = EditObject.ChapterObject.Type,
                Name = EditObject.ChapterObject.Name,
                EntityType = EditObject.ChapterObject.EntityType,
                SeqNo = EditObject.ChapterObject.SeqNo,
                SuppressStep = EditObject.ChapterObject.SuppressStep,
                CompleteName = EditObject.ChapterObject.CompleteName,
                AsName = EditObject.ChapterObject.AsName,
                RescheduleDays = EditObject.ChapterObject.RescheduleDays,
                AltDisplayName = EditObject.ChapterObject.AltDisplayName,
                UserMessage = EditObject.ChapterObject.UserMessage,
                PopupAlert = EditObject.ChapterObject.PopupAlert,
                NextStatus = EditObject.ChapterObject.NextStatus,
                LinkedItems = EditObject.ChapterObject.LinkedItems is null ? new List<LinkedItems>() : EditObject.ChapterObject.LinkedItems
            };

            
            var attachment = _AttachObject is null 
                ? new LinkedItems 
                {
                    Action = "INSERT",
                    ChaserDesc = "",
                    DocAsName = "",
                    DocName = "",
                    ScheduleDataItem = "",
                    TrackingMethod = "",
                    CustomItem = "N"
                } 
                : copyObject.LinkedItems.Where(F => F.DocName == _AttachObject.DocName).FirstOrDefault(); 


            var parameters = new ModalParameters();
            parameters.Add("_TaskObject", EditObject.ChapterObject);
            parameters.Add("_CopyObject", copyObject);
            parameters.Add("_DataChanged", refreshSelectedList);
            parameters.Add("_SelectedList", SelectedList);
            parameters.Add("_LibraryDocumentsAndSteps", _LibraryDocumentsAndSteps);
            parameters.Add("_P4WCaseTypeGroups", _P4WCaseTypeGroups);
            parameters.Add("_ListOfStatus", _LstStatus);
            parameters.Add("_ListOfAgenda", _LstAgendas);
            parameters.Add("_SelectedChapter", _SelectedChapter);
            parameters.Add("_SelectedChapterObject", _SelectedChapterObject);
            parameters.Add("_Attachment", attachment);
            parameters.Add("_TableDates", _TableDates);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-chapter-doc"
            };

            Modal.Show<ModalChapterAttachments>("Linked Item", parameters, options);
        }

        protected void ShowChapterAttachmentViewModal()
        {
            var attachment = _AttachObject is null ? new LinkedItems { Action = "INSERT" } : _AttachObject;

            var parameters = new ModalParameters();
            parameters.Add("_Attachment", attachment);
            parameters.Add("_SelectedChapter", _SelectedChapter);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-chapter-comparison"
            };

            Modal.Show<ModalChapterAttachmentsView>("Linked Item", parameters, options);
        }

        private async void RefreshLibraryDocumentsAndSteps()
        {
            await RefreshLibraryDocumentsAndStepsTask();
        }

        private async Task RefreshLibraryDocumentsAndStepsTask()
        {
            try
            {
                _LibraryDocumentsAndSteps = await ChapterManagementService.GetDocumentList(_SelectedChapter.CaseType);
                _LibraryDocumentsAndSteps = _LibraryDocumentsAndSteps.Where(D => !(D.Name is null)).ToList();
                //StateHasChanged();
            }
            catch (Exception ex)
            {
                GenericErrorLog(true,ex, "DisplaySmartflowLoadError", $"Refreshing the document list: {ex.Message}");

            }
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

                    var cItems = AltChapter.Items;

                    LstAltSystemItems = cItems.Select(T => new VmGenSmartflowItem { ChapterObject = T, Compared = false }).ToList();
                    
                    foreach (var item in _LstDocs)
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

            infoText = $"Make the {UserSession.AltSystem} system the same as {UserSession.SelectedSystem} for all items.";

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
                foreach (var item in AltChapter.Items.Where(I => I.Type == "Doc").ToList())
                {
                    AltChapter.Items.Remove(item);
                }

                AltChapter.Items.AddRange(_SelectedChapter.Items.Where(I => I.Type == "Doc").ToList());

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

        private void ShowChapterComparisonModal(VmGenSmartflowItem _selectedItem) 
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

                Modal.Show<ChapterDocumentComparison>("Synchronise Smartflow Item", parameters, options);
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


        protected void ShowChapterDeleteAlt(VmGenSmartflowItem _selectedItem)
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

                Modal.Show<ModalChapterDetailDelete>($"Delete Status", parameters, options);
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

            AltChapter.Items.Remove(EditObject.ChapterObject);
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
        /// <param name="listType">: Docs or Fees</param>
        /// <param name="direction">: Up or Down</param>
        /// <returns>No return</returns>
        protected async void MoveSeq(GenSmartflowItem _selectobject, string _direction)
        {
            await MoveSeqTask(_selectobject, _direction);
        }

        protected async Task MoveSeqTask(GenSmartflowItem _selectobject, string _direction)
        {
            try
            {
                _SeqMoving = true; //prevents changes to the form whilst process of changing seq is carried out

                int incrementBy;

                incrementBy = (_direction.ToLower() == "up" ? -1 : 1);

                RowChanged = (int)(_selectobject.SeqNo + incrementBy);

                
                var swapItem = _LstDocs.Where(D => D.ChapterObject.SeqNo == (_selectobject.SeqNo + incrementBy)).SingleOrDefault();
                if (!(swapItem is null))
                {
                    _selectobject.SeqNo += incrementBy;
                    swapItem.ChapterObject.SeqNo = swapItem.ChapterObject.SeqNo + (incrementBy * -1);

                    _SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(_SelectedChapter);
                    await ChapterManagementService.Update(_SelectedChapterObject);


                    //keep track of time last updated ready for comparison by other sessions checking for updates
                    AppChapterState.SetLastUpdated(UserSession, _SelectedChapter);

                }

                await _RefreshChapterItems.InvokeAsync("Docs");
                await InvokeAsync(() =>
                {
                    StateHasChanged();
                });
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
                if(_LstDocs.Select(C => C.ChapterObject.SeqNo != _LstDocs.IndexOf(C) + 1).Count() > 0) //If any SeqNos are out of sequence
                { 
                    _LstDocs.Select(C => { C.ChapterObject.SeqNo = _LstDocs.IndexOf(C) + 1; return C; }).ToList();

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
        private void GenericErrorLog(bool _showNotificationMsg, Exception e, string _method, string _message)
        {
            using (LogContext.PushProperty("SourceSystem", UserSession.SelectedSystem))
            using (LogContext.PushProperty("SourceCompanyId", UserSession.Company.Id))
            using (LogContext.PushProperty("SourceUserId", UserSession.User.Id))
            using (LogContext.PushProperty("SourceContext", nameof(ChapterDocumentDetail)))
            {
                Logger.LogError(e,"Error - Method: {0}, Message: {1}",_method, _message);
            }

            if(_showNotificationMsg)
            {
                NotificationManager.ShowNotification("Danger", $"Oops! Something went wrong.");
            }
        }

        
    }
}