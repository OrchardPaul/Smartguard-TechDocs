using Blazored.Modal;
using GadjIT_App.Services;
using GadjIT_App.Services.SessionState;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Serilog.Context;
using Microsoft.Extensions.Logging;
using GadjIT_ClientContext.Models.Smartflow;
using GadjIT_ClientContext.Models.P4W;
using GadjIT_ClientContext.Models.Smartflow.Client;

namespace GadjIT_App.Pages.Smartflows.ComponentsSmartflowDetail._Documents
{

    public partial class ModalSmartflowDocumentDetail : ComponentBase
    {

        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }


        [Parameter]
        public string _Option { get; set; }

        [Parameter]
        public Client_SmartflowRecord _Selected_ClientSmartflowRecord { get; set; }

        [Parameter]
        public SmartflowV2 _SelectedSmartflow { get; set; }

        [Parameter]
        public SmartflowDocument _TaskObject { get; set; }

        [Parameter]
        public SmartflowDocument _CopyObject { get; set; }


        [Parameter]
        public Action _DataChanged { get; set; } 

        [Parameter]
        public Action _RefreshLibraryDocumentsAndSteps { get; set; } 
        
        [Parameter]
        public List<P4W_TableDate> _TableDates { get; set; }

        [Parameter]
        public List<P4W_CaseTypeGroups> _P4WCaseTypeGroups { get; set; }

        [Parameter]
        public List<P4W_DmDocuments> _LibraryDocumentsAndSteps { get; set; } = new List<P4W_DmDocuments>();
        
        [Parameter]
        public List<VmSmartflowStatus> _ListOfStatus { get; set; } = new List<VmSmartflowStatus>();

        [Parameter]
        public List<VmSmartflowAgenda> _ListOfAgenda { get; set; } = new List<VmSmartflowAgenda>();


        
        [Inject]
        ILogger<ModalSmartflowDocumentDetail> Logger {get; set;}

        [Inject]
        INotificationManager NotificationManager {get; set;}

        [Inject]
        IClientApiManagementService ClientApiManagementService { get; set; }


        [Inject]
        IUserSessionState UserSession { get; set; }


        
        public int Error { get; set; } = 0;

        public string FilterText { get; set; } = "";
        
        public string FilterTextDataItem { get; set; } = "";

        private int SelectedCaseTypeGroup { get; set; } = -2;

        List<string> Actions = new List<string>() { "TAKE", "INSERT" };
        
        List<string> TrackMethodList = new List<string>() { "N/A", "Send Only", "Response Required" };

        public bool UseCustomItem 
        { get { return _CopyObject.CustomItem == "Y" ? true : false; }
            set {
                if (value)
                {
                    _CopyObject.CustomItem = "Y";
                    _CopyObject.AltDisplayName = "";
                    _CopyObject.Agenda = "";
                }
                else
                {
                    _CopyObject.CustomItem = "N";
                }
            } 
        }
            

        public bool _useCustomReschedule { get; set; }
        public bool UseCustomReschedule {
            get { return _useCustomReschedule; }
            set
            {
                _CopyObject.RescheduleDataItem = !value ? "" : _CopyObject.RescheduleDataItem;
                _useCustomReschedule = value;
            }
        }

        public bool _useCustomMilestone { get; set; }
        public bool UseCustomMilestone
        {
            get { return _useCustomMilestone; }
            set
            {
                _CopyObject.RescheduleDataItem = !value ? "" : _CopyObject.RescheduleDataItem;
                _useCustomMilestone = value;
            }
        }

        

        public bool OptionalDocument
        {
            get { return (_CopyObject.OptionalDocument == "Y" ? true : false); }
            set
            {
                if (value)
                {
                    _CopyObject.OptionalDocument = "Y";
                }
                else
                {
                    _CopyObject.OptionalDocument = "N";
                }
            }
        }

        public int? RescheduleDays
        {
            get { return _CopyObject.RescheduleDays; }
            set
            {
                
                 _CopyObject.RescheduleDays = value;
            }
        }


        protected override async Task OnInitializedAsync()
        {
            try
            {

            
                if (!(string.IsNullOrEmpty(_SelectedSmartflow.P4WCaseTypeGroup)) && (_SelectedSmartflow.P4WCaseTypeGroup != "Select"))
                {
                    if(ModalInstance.Title == "Steps and Documents")
                    {
                        SelectedCaseTypeGroup = _P4WCaseTypeGroups.Where(CT => CT.Name == _SelectedSmartflow.P4WCaseTypeGroup).Select(CT => CT.Id).FirstOrDefault();

                        if (_SelectedSmartflow.P4WCaseTypeGroup == "Global Documents")
                        {
                            SelectedCaseTypeGroup = 0;
                        }

                        if (_SelectedSmartflow.P4WCaseTypeGroup == "Entity Documents")
                        {
                            SelectedCaseTypeGroup = -1;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(_CopyObject.RescheduleDataItem)
                        && _TableDates.ToList().Select(D => D.TableField).Contains(_CopyObject.RescheduleDataItem))
                {
                    UseCustomReschedule = true;
                }
                else
                {
                    UseCustomReschedule = false;
                }

            }
            catch(Exception e)
            {
                SelectedCaseTypeGroup = 0; 
                GenericErrorLog(false, e, "OnInitialized", e.Message);
            }

        }


        private async Task RefreshDocListOnModel()
        {
            try
            {
                _LibraryDocumentsAndSteps = await ClientApiManagementService.GetDocumentList(_SelectedSmartflow.CaseType);

                _RefreshLibraryDocumentsAndSteps.Invoke(); //refresh library for the main list
            
            }
            catch(Exception e)
            {
                _LibraryDocumentsAndSteps = new List<P4W_DmDocuments>();
                GenericErrorLog(false, e, "RefreshDocListOnModel", e.Message);
            }
        }

        private async void Close()
        {
            _TaskObject = new SmartflowDocument();
            await ModalInstance.CloseAsync();


        }

        private void HandleValidSubmit()
        {
            try
            {

                
                if (_SelectedSmartflow.Documents.Where(D => D.Name != _CopyObject.Name).Select(D => D.Name).Contains(_CopyObject.Name))
                {
                    Error = 1;
                    StateHasChanged();
                }
                else
                {


                    _TaskObject.Name = Regex.Replace(_CopyObject.Name, "[^0-9a-zA-Z-_ (){}!£$%^&*,./#?@<>`:]+", "");
                    
                    _TaskObject.EntityType = _CopyObject.EntityType;
                    _TaskObject.SeqNo = _CopyObject.SeqNo;
                    _TaskObject.CompleteName = _CopyObject.CompleteName is null ? "" : Regex.Replace(_CopyObject.CompleteName, "[^0-9a-zA-Z-_ (){}!£$%^&*,./#?@<>`:]+", "");
                    _TaskObject.AsName = _CopyObject.AsName is null ? "" : Regex.Replace(_CopyObject.AsName, "[^0-9a-zA-Z-_ (){}!£$%^&*,./#?@<>`:]+", "");
                    _TaskObject.RescheduleDays = _CopyObject.RescheduleDays is null ? 0 : _CopyObject.RescheduleDays;
                    _TaskObject.AltDisplayName = _CopyObject.AltDisplayName is null ? "" : Regex.Replace(_CopyObject.AltDisplayName, "[^0-9a-zA-Z-_ (){}!£$%^&*,./#?@<>`:]+", "");
                    _TaskObject.UserMessage = _CopyObject.UserMessage;
                    _TaskObject.PopupAlert = _CopyObject.PopupAlert;
                    _TaskObject.NextStatus = _CopyObject.NextStatus;
                    _TaskObject.Action = _CopyObject.Action;
                    _TaskObject.TrackingMethod = _CopyObject.TrackingMethod;
                    _TaskObject.ChaserDesc = _CopyObject.ChaserDesc;
                    _TaskObject.RescheduleDataItem = _CopyObject.RescheduleDataItem;
                    _TaskObject.OptionalDocument = _CopyObject.OptionalDocument;
                    _TaskObject.Agenda = _CopyObject.Agenda;
                    _TaskObject.CustomItem = _CopyObject.CustomItem;

                    if (_Option == "Insert")
                    {
                        _SelectedSmartflow.Documents.Add(_TaskObject);
                    }

                    _TaskObject = new SmartflowDocument();
                    FilterText = "";

                    _DataChanged.Invoke();
                    Close();
                    
                }

            }
            catch(Exception e)
            {
                GenericErrorLog(true,e, "HandleValidSubmit", e.Message);
            }
            finally
            {
                Close();
            }
        }

        private void ResetError()
        {
            Error = 0;
            StateHasChanged();
        }

        /****************************************/
        /* ERROR HANDLING */
        /****************************************/
        private async void GenericErrorLog(bool showNotificationMsg, Exception e, string _method, string _message)
        {
            if(showNotificationMsg)
            {
                await NotificationManager.ShowNotification("Danger", $"Oops! Something went wrong.").ConfigureAwait(false);
            }
            
            using (LogContext.PushProperty("SourceSystem", UserSession.SelectedSystem))
            using (LogContext.PushProperty("SourceCompanyId", UserSession.Company.Id))
            using (LogContext.PushProperty("SourceUserId", UserSession.User.Id))
            using (LogContext.PushProperty("SourceContext", nameof(ModalSmartflowDocumentDetail)))
            {
                Logger.LogError(e,"Error - Method: {0}, Message: {1}",_method, _message);
            }

        }

    }

    
}
