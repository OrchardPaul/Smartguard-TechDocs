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

namespace GadjIT_App.Pages.Chapters.ComponentsChapterDetail._Agenda
{
    public partial class ChapterAgendaDetail
    {

        [Parameter]
        public List<VmGenSmartflowItem> _LstAgendas { get; set; }
        
        [Parameter]
        public UsrOrsfSmartflows _SelectedChapterObject { get; set; }

        [Parameter]
        public VmChapter _SelectedChapter { get; set; }

        [Parameter]
        public EventCallback<string> _RefreshChapterItems {get; set;}




        [Inject]
        private ILogger<ChapterAgendaDetail> Logger { get; set; }

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

        public VmGenSmartflowItem EditObject = new VmGenSmartflowItem { ChapterObject = new GenSmartflowItem() };

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

    
        protected void PrepareForInsert() 
        {
            try
            {
                
                EditObject = new VmGenSmartflowItem { ChapterObject = new GenSmartflowItem() };
                
                EditObject.ChapterObject.Type = "Agenda";   //Required for both Edit Object 
                EditObject.ChapterObject.SeqNo = 0;         //and Copy Object for validation

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

                Action dataChanged = HandleUpdate;

                
                var copyObject = new GenSmartflowItem
                {
                    Type = EditObject.ChapterObject.Type,
                    SeqNo = EditObject.ChapterObject.SeqNo,
                    Name = EditObject.ChapterObject.Name,
                    Agenda = EditObject.ChapterObject.Agenda,
                };

                var parameters = new ModalParameters();
                parameters.Add("_Option", _option);
                parameters.Add("_SelectedChapterObject", _SelectedChapterObject);
                parameters.Add("_SelectedChapter", _SelectedChapter);
                parameters.Add("_TaskObject", EditObject.ChapterObject);
                parameters.Add("_CopyObject", copyObject);
                parameters.Add("_DataChanged", dataChanged);

                var options = new ModalOptions()
                {
                    Class = "blazored-custom-modal modal-chapter-agenda" 
                };

                Modal.Show<ModalChapterDetail>("Agenda", parameters, options);
            }
            catch(Exception e)
            {
                GenericErrorLog(false,e, "ShowChapterDetailModal", e.Message);
            }

        }

        private void HandleUpdate()
        {
            //RefreshSelectedList();

            _RefreshChapterItems.InvokeAsync("Agenda");

        }


        protected void ShowChapterDetailViewModal(VmGenSmartflowItem _selectedObject)//moved partial
        {
            try
            {
                var parameters = new ModalParameters();
                parameters.Add("_Object", _selectedObject);
                parameters.Add("_SelectedList", "Agenda"); 

                var options = new ModalOptions()
                {
                    Class = "blazored-custom-modal modal-chapter-agenda"
                };

                Modal.Show<ModalChapterDetailView>("Agenda", parameters, options);
            }
            catch(Exception e)
            {
                GenericErrorLog(false,e, "ShowChapterDetailViewModal", e.Message);
            }
        }

        protected void ShowChapterDetailDelete(VmGenSmartflowItem _selectedChapterItem) 
        {
            EditObject = _selectedChapterItem;

            string itemName = _selectedChapterItem.ChapterObject.Name;
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
            await _RefreshChapterItems.InvokeAsync("Agenda");
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
                    
                    foreach (var item in _LstAgendas)
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
                foreach (var item in AltChapter.Items.Where(I => I.Type == "Agenda").ToList())
                {
                    AltChapter.Items.Remove(item);
                }

                AltChapter.Items.AddRange(_SelectedChapter.Items.Where(I => I.Type == "Agenda").ToList());

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

                Modal.Show<ChapterAgendaComparison>("Synchronise Smartflow Item", parameters, options);
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

                Modal.Show<ModalChapterDetailDelete>($"Delete Agenda", parameters, options);
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
        


        

        /****************************************/
        /* ERROR HANDLING */
        /****************************************/
        private async void GenericErrorLog(bool _showNotificationMsg, Exception e, string _method, string _message)
        {
            using (LogContext.PushProperty("SourceSystem", UserSession.SelectedSystem))
            using (LogContext.PushProperty("SourceCompanyId", UserSession.Company.Id))
            using (LogContext.PushProperty("SourceUserId", UserSession.User.Id))
            using (LogContext.PushProperty("SourceContext", nameof(ChapterAgendaDetail)))
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