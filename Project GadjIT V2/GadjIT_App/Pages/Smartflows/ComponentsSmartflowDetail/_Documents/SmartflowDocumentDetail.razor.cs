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

namespace GadjIT_App.Pages.Smartflows.ComponentsSmartflowDetail._Documents
{
    public partial class SmartflowDocumentDetail
    {


        [Parameter]
        public List<VmSmartflowDocument> _LstDocs { get; set; } 

        [Parameter]
        public List<VmSmartflowStatus> _LstStatus { get; set; } 
        
        [Parameter]
        public List<VmSmartflowAgenda> _LstAgendas { get; set; }

        [Parameter]
        public Client_SmartflowRecord _Selected_ClientSmartflowRecord { get; set; }

        [Parameter]
        public SmartflowV2 _SelectedSmartflow { get; set; }


        

        [Parameter]
        public EventCallback<SmartflowV2> _SmartflowUpdated {get; set;}

        [Parameter]
        public EventCallback<string> _RefreshSmartflowItems {get; set;}

       
        
        [Parameter]
        public List<P4W_CaseTypeGroups> _P4WCaseTypeGroups {get; set;}


        [Parameter]
        public List<P4W_DmDocuments> _LibraryDocumentsAndSteps {get; set;}

        [Parameter]
        public EventCallback _RefreshLibraryDocumentsAndSteps {get; set;}

        [Parameter]
        public List<P4W_TableDate> _TableDates {get; set;}

        [Parameter]
        public bool _SmartflowLockedForEdit {get; set;}
        

                


        [Inject]
        private ILogger<SmartflowDocumentDetail> Logger { get; set; }

        [Inject]
        public INotificationManager NotificationManager {get; set;}

        [Inject]
        private IClientApiManagementService ClientApiManagementService { get; set; }

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

        private SmartflowV2 AltSmartflow {get; set;} //Smartflow Schema

        public List<VmSmartflowDocument> LstAltSystemItems { get; set; } 

        private VmSmartflowDocument EditObject {get; set;}

        [Parameter]
        public LinkedDocument EditLinkedObject {get; set;}

        public bool SeqMoving {get; set;}

        private string RowChangedClass { get; set; } = "row-changed-nav3";

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
        
        
        private string SelectedList = "";


        protected override async Task OnInitializedAsync()
        {
            await _RefreshLibraryDocumentsAndSteps.InvokeAsync();
            await _RefreshSmartflowItems.InvokeAsync("Agenda");
            await _RefreshSmartflowItems.InvokeAsync("Status");
        }
       
        
        protected void PrepareForInsert() 
        {
            try
            {
                
                EditObject = new VmSmartflowDocument { SmartflowObject = new SmartflowDocument() };
                EditObject.SmartflowObject.Action = "INSERT"; //default item to INSERT, user can opt for a TAKE on the form if required
                
                if(_LstDocs != null && _LstDocs.Count > 0)
                {
                    EditObject.SmartflowObject.SeqNo = _LstDocs
                                                            .OrderByDescending(A => A.SmartflowObject.SeqNo)
                                                            .Select(A => A.SmartflowObject.SeqNo)
                                                            .FirstOrDefault() + 1;
                }
                else
                {
                    EditObject.SmartflowObject.SeqNo = 1;
                }

                
                ShowSmartflowDetailModal("Insert");
                
            }
            catch(Exception e)
            {
                GenericErrorLog(false, e, "PrepareForInsert", e.Message);
            }
            
        }


        protected void PrepareForEdit(VmSmartflowDocument _selectedObject)
        {
            EditObject = _selectedObject;

            ShowSmartflowDetailModal("Edit");

        }

        protected void ShowSmartflowDetailModal(string _option) //moved partial
        {
            try
            {

                Action refreshSelectedList = HandleUpdate;
                Action refreshLibraryDocumentsAndSteps = RefreshLibraryDocumentsAndSteps;

                var copyObject = new SmartflowDocument
                {
                    Name = EditObject.SmartflowObject.Name,
                    EntityType = EditObject.SmartflowObject.EntityType,
                    SeqNo = EditObject.SmartflowObject.SeqNo,
                    CompleteName = EditObject.SmartflowObject.CompleteName,
                    AsName = EditObject.SmartflowObject.AsName,
                    RescheduleDays = EditObject.SmartflowObject.RescheduleDays,
                    AltDisplayName = EditObject.SmartflowObject.AltDisplayName,
                    UserMessage = EditObject.SmartflowObject.UserMessage,
                    PopupAlert = EditObject.SmartflowObject.PopupAlert,
                    NextStatus = EditObject.SmartflowObject.NextStatus,
                    Action = EditObject.SmartflowObject.Action,
                    TrackingMethod = EditObject.SmartflowObject.TrackingMethod,
                    ChaserDesc = EditObject.SmartflowObject.ChaserDesc,
                    RescheduleDataItem = EditObject.SmartflowObject.RescheduleDataItem,
                    OptionalDocument = EditObject.SmartflowObject.OptionalDocument,
                    Agenda = EditObject.SmartflowObject.Agenda,
                    CustomItem = EditObject.SmartflowObject.CustomItem
            };

                var parameters = new ModalParameters();
                parameters.Add("_Option", _option);
                parameters.Add("_SelectedSmartflow", _SelectedSmartflow);
                parameters.Add("_Selected_ClientSmartflowRecord", _Selected_ClientSmartflowRecord);
                parameters.Add("_TaskObject", EditObject.SmartflowObject);
                parameters.Add("_CopyObject", copyObject);
                parameters.Add("_DataChanged", refreshSelectedList);
                parameters.Add("_RefreshLibraryDocumentsAndSteps", refreshLibraryDocumentsAndSteps);
                parameters.Add("_LibraryDocumentsAndSteps", _LibraryDocumentsAndSteps);
                parameters.Add("_TableDates", _TableDates);
                parameters.Add("_P4WCaseTypeGroups", _P4WCaseTypeGroups);
                parameters.Add("_ListOfStatus", _LstStatus);
                parameters.Add("_ListOfAgenda", _LstAgendas);

                var options = new ModalOptions()
                {
                    Class = "blazored-custom-modal modal-smartflow-doc" 
                };

                Modal.Show<ModalSmartflowDocumentDetail>("Steps and Documents", parameters, options);
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

        protected void ShowSmartflowDetailViewModal(VmSmartflowDocument _selectedObject)
        {
            try
            {
                var parameters = new ModalParameters();
                parameters.Add("_Object", _selectedObject);

                var options = new ModalOptions()
                {
                    Class = "blazored-custom-modal modal-smartflow-comparison"
                };

                Modal.Show<ModalSmartflowDocumentView>("Steps and Documents", parameters, options);
            }
            catch(Exception e)
            {
                GenericErrorLog(false,e, "ShowSmartflowDetailViewModal", e.Message);
            }
        }

        protected void ShowSmartflowDetailDelete(VmSmartflowDocument _selectedSmartflowItem) 
        {
            EditObject = _selectedSmartflowItem;

            string itemName = (string.IsNullOrEmpty(_selectedSmartflowItem.SmartflowObject.AltDisplayName) ? _selectedSmartflowItem.SmartflowObject.Name : _selectedSmartflowItem.SmartflowObject.AltDisplayName);
            
            Action SelectedDeleteAction = HandleDelete;
            var parameters = new ModalParameters();
            parameters.Add("_ItemName", itemName);
            parameters.Add("_DeleteAction", SelectedDeleteAction);
            parameters.Add("_InfoText", $"Are you sure you wish to delete the Document: '{itemName}'? ");

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal"
            };

            Modal.Show<ModalSmartflowDetailDelete>($"Delete Document", parameters, options);
        }

        private async void HandleDelete() 
        {
            //<ModalDelete> simply invokes this method when user cicks OK. No need for the modal to handle this action as we do not require any details from the Modal. 
            _SelectedSmartflow.Documents.Remove(EditObject.SmartflowObject);
            
            await ChapterItemsUpdated();

        }

        private async Task ChapterItemsUpdated()
        {
            await InvokeAsync(() =>
            {
                _SmartflowUpdated.InvokeAsync(_SelectedSmartflow);
                _RefreshSmartflowItems.InvokeAsync("Docs");
            });

        }

        

        /// ##################################################################################
        /// Attachments:
        /// 
        /// ##################################################################################
    
        private void PrepareAttachmentForAdd(VmSmartflowDocument _item)
        {
            SelectedList = "New Attachement";
            EditObject = _item;
            EditLinkedObject = null;

            ShowSmartflowAttachmentModal();
        }

        private void PrepareAttachmentForEdit(VmSmartflowDocument _item, LinkedDocument _linkedItem)
        {
            SelectedList = "Edit Attachement";
            EditObject = _item;
            EditLinkedObject = _linkedItem;

            ShowSmartflowAttachmentModal();
        }

        private void PrepareAttachmentForView(VmSmartflowDocument _item, LinkedDocument _linkedItem)
        {
            SelectedList = "Edit Attachement";
            EditObject = _item;
            EditLinkedObject = _linkedItem;

            ShowSmartflowAttachmentViewModal();
        }
       

        protected void ShowSmartflowAttachmentModal()
        {
            Action refreshSelectedList = HandleUpdate;
            Action refreshLibraryDocumentsAndSteps = RefreshLibraryDocumentsAndSteps;

            var copyObject = new SmartflowDocument
            {
                Name = EditObject.SmartflowObject.Name,
                EntityType = EditObject.SmartflowObject.EntityType,
                SeqNo = EditObject.SmartflowObject.SeqNo,
                CompleteName = EditObject.SmartflowObject.CompleteName,
                AsName = EditObject.SmartflowObject.AsName,
                RescheduleDays = EditObject.SmartflowObject.RescheduleDays,
                AltDisplayName = EditObject.SmartflowObject.AltDisplayName,
                UserMessage = EditObject.SmartflowObject.UserMessage,
                PopupAlert = EditObject.SmartflowObject.PopupAlert,
                NextStatus = EditObject.SmartflowObject.NextStatus,
                LinkedItems = EditObject.SmartflowObject.LinkedItems is null ? new List<LinkedDocument>() : EditObject.SmartflowObject.LinkedItems
            };

            
            var attachment = EditLinkedObject is null 
                ? new LinkedDocument
                {
                    Action = "INSERT",
                    ChaserDesc = "",
                    DocAsName = "",
                    DocName = "",
                    ScheduleDataItem = "",
                    TrackingMethod = "",
                    CustomItem = "N"
                } 
                : copyObject.LinkedItems.Where(F => F.DocName == EditLinkedObject.DocName).FirstOrDefault(); 


            var parameters = new ModalParameters();
            parameters.Add("_TaskObject", EditObject.SmartflowObject);
            parameters.Add("_CopyObject", copyObject);
            parameters.Add("_DataChanged", refreshSelectedList);
            parameters.Add("_RefreshLibraryDocumentsAndSteps", refreshLibraryDocumentsAndSteps);
            parameters.Add("_SelectedList", SelectedList);
            parameters.Add("_LibraryDocumentsAndSteps", _LibraryDocumentsAndSteps);
            parameters.Add("_P4WCaseTypeGroups", _P4WCaseTypeGroups);
            parameters.Add("_ListOfStatus", _LstStatus);
            parameters.Add("_ListOfAgenda", _LstAgendas);
            parameters.Add("_SelectedSmartflow", _SelectedSmartflow);
            parameters.Add("_Selected_ClientSmartflowRecord", _Selected_ClientSmartflowRecord);
            parameters.Add("_Attachment", attachment);
            parameters.Add("_TableDates", _TableDates);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-smartflow-doc"
            };

            Modal.Show<ModalSmartflowAttachments>("Linked Item", parameters, options);
        }

        protected void ShowSmartflowAttachmentViewModal()
        {
            var attachment = EditLinkedObject is null ? new LinkedDocument { Action = "INSERT" } : EditLinkedObject;

            var parameters = new ModalParameters();
            parameters.Add("_Attachment", attachment);
            parameters.Add("_SelectedSmartflow", _SelectedSmartflow);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-smartflow-comparison"
            };

            Modal.Show<ModalSmartflowAttachmentsView>("Linked Item", parameters, options);
        }

        private async void RefreshLibraryDocumentsAndSteps()
        {
            await RefreshLibraryDocumentsAndStepsTask();
        }

        private async Task RefreshLibraryDocumentsAndStepsTask()
        {
            try
            {
                await _RefreshLibraryDocumentsAndSteps.InvokeAsync();
                
                await InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            }
            catch (Exception ex)
            {
                GenericErrorLog(true,ex, "RefreshLibraryDocumentsAndStepsTask", $"Refreshing the document list: {ex.Message}");

            }
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

                        AltSmartflow = JsonConvert.DeserializeObject<SmartflowV2>(Alt_AppSmartflowRecord.SmartflowData);

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

                        var cItems = AltSmartflow.Documents;

                        LstAltSystemItems = cItems.Select(T => new VmSmartflowDocument { SmartflowObject = T, Compared = false }).ToList();
                        
                        foreach (var item in _LstDocs)
                        {
                            var altObject = LstAltSystemItems
                                        .Where(A => A.SmartflowObject.Name == item.SmartflowObject.Name)
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
                
                //return false;
            }
            finally
            {

                await InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            }

            


            //return true;
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
                foreach (var item in AltSmartflow.Documents.ToList())
                {
                    AltSmartflow.Documents.Remove(item);
                }

                AltSmartflow.Documents.AddRange(_SelectedSmartflow.Documents.ToList());

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

        private void ShowSmartflowComparisonModal(VmSmartflowDocument _selectedItem) 
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

                Modal.Show<SmartflowDocumentComparison>("Synchronise Smartflow Item", parameters, options);
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


        protected void ShowSmartflowDeleteAlt(VmSmartflowDocument _selectedItem)
        {
            try
            {
                EditObject = _selectedItem;
                
                Action SelectedDeleteAction = HandleAltDelete;
                var parameters = new ModalParameters();
                parameters.Add("_ItemName", _selectedItem.SmartflowObject.Name);
                parameters.Add("_DeleteAction", SelectedDeleteAction);
                parameters.Add("_InfoText", $"Are you sure you wish to delete the '{_selectedItem.SmartflowObject.Name}' item from {UserSession.AltSystem} system?");

                var options = new ModalOptions()
                {
                    Class = "blazored-custom-modal"
                };

                Modal.Show<ModalSmartflowDetailDelete>($"Delete Document", parameters, options);
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

            AltSmartflow.Documents.Remove(EditObject.SmartflowObject);
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
        protected async Task MoveSeq(SmartflowDocument _selectobject, string _direction)
        {
            try
            {
                SeqMoving = true; //prevents changes to the form whilst process of changing seq is carried out

                int incrementBy;

                incrementBy = (_direction.ToLower() == "up" ? -1 : 1);

                RowChanged = (int)(_selectobject.SeqNo + incrementBy);

                
                var swapItem = _LstDocs.Where(D => D.SmartflowObject.SeqNo == (_selectobject.SeqNo + incrementBy)).SingleOrDefault();
                if (!(swapItem is null))
                {
                    _selectobject.SeqNo += incrementBy;
                    swapItem.SmartflowObject.SeqNo = swapItem.SmartflowObject.SeqNo + (incrementBy * -1);

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

            _LstDocs.Select(C => { C.SmartflowObject.SeqNo = _LstDocs.IndexOf(C) + 1; return C; }).ToList();

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
        private void GenericErrorLog(bool _showNotificationMsg, Exception e, string _method, string _message)
        {
            using (LogContext.PushProperty("SourceSystem", UserSession.SelectedSystem))
            using (LogContext.PushProperty("SourceCompanyId", UserSession.Company.Id))
            using (LogContext.PushProperty("SourceUserId", UserSession.User.Id))
            using (LogContext.PushProperty("SourceContext", nameof(SmartflowDocumentDetail)))
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