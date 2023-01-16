using Blazored.Modal;
using GadjIT_ClientContext.P4W;
using GadjIT_ClientContext.P4W.Custom;
using GadjIT_App.Data.Admin;
using GadjIT_App.Services;
using GadjIT_App.Services.AppState;
using GadjIT_App.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Serilog.Context;
using Microsoft.Extensions.Logging;

namespace GadjIT_App.Pages.Chapters.ComponentsChapterDetail._SharedItems
{

    public partial class ModalChapterDetail : ComponentBase
    {

        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }


        [Parameter]
        public string _Option { get; set; }

        [Parameter]
        public UsrOrsfSmartflows _SelectedChapterObject { get; set; }

        [Parameter]
        public VmChapter _SelectedChapter { get; set; }

        [Parameter]
        public GenSmartflowItem _TaskObject { get; set; }

        [Parameter]
        public GenSmartflowItem _CopyObject { get; set; }


        [Parameter]
        public Action _DataChanged { get; set; }

                
        
        [Parameter]
        public List<TableDate> _TableDates { get; set; }

        [Parameter]
        public List<CaseTypeGroups> _P4WCaseTypeGroups { get; set; }

        [Parameter]
        public List<DmDocuments> _LibraryDocumentsAndSteps { get; set; } = new List<DmDocuments>();
        
        [Parameter]
        public List<VmGenSmartflowItem> _ListOfStatus { get; set; } = new List<VmGenSmartflowItem>();

        [Parameter]
        public List<VmGenSmartflowItem> _ListOfAgenda { get; set; } = new List<VmGenSmartflowItem>();


        
        [Inject]
        ILogger<ModalChapterDetail> Logger {get; set;}

        [Inject]
        INotificationManager NotificationManager {get; set;}

        [Inject]
        IChapterManagementService ChapterManagementService { get; set; }

        [Inject]
        IAppChapterState AppChapterState { get; set; }

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

        public bool SuppressStep
        {
            get { return (_CopyObject.SuppressStep == "Y" ? true : false); }
            set
            {
                if (value)
                {
                    _CopyObject.SuppressStep = "Y";
                }
                else
                {
                    _CopyObject.SuppressStep = "N";
                }
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

            
                if (!(string.IsNullOrEmpty(_SelectedChapter.P4WCaseTypeGroup)) && (_SelectedChapter.P4WCaseTypeGroup != "Select"))
                {
                    if(ModalInstance.Title == "Steps and Documents")
                    {
                        SelectedCaseTypeGroup = _P4WCaseTypeGroups.Where(CT => CT.Name == _SelectedChapter.P4WCaseTypeGroup).Select(CT => CT.Id).FirstOrDefault();

                        if (_SelectedChapter.P4WCaseTypeGroup == "Global Documents")
                        {
                            SelectedCaseTypeGroup = 0;
                        }

                        if (_SelectedChapter.P4WCaseTypeGroup == "Entity Documents")
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
                _LibraryDocumentsAndSteps = await ChapterManagementService.GetDocumentList(_SelectedChapter.CaseType);
            
            }
            catch(Exception e)
            {
                _LibraryDocumentsAndSteps = new List<DmDocuments>();
                GenericErrorLog(false, e, "RefreshDocListOnModel", e.Message);
            }
        }

        private async void Close()
        {
            _TaskObject = new GenSmartflowItem();
            await ModalInstance.CloseAsync();


        }

        private void HandleValidSubmit()
        {
            try
            {

                
                if (_SelectedChapter.Items.Where(I => I.Name != _CopyObject.Name).Select(I => I.Name).Contains(_CopyObject.Name))
                {
                    Error = 1;
                    StateHasChanged();
                }
                else
                {

                    if (!(new string[] { "Agenda", "Status" }.Any(s => _TaskObject.Type.ToString().Contains(s))))
                    {
                        //clears lagacy value of "Letter" and revert it back to "Doc"
                        _TaskObject.Type = "Doc";
                    }
                    else
                    {
                        _TaskObject.Type = _CopyObject.Type;
                    }

                    if (_TaskObject.Type == "Agenda")
                    {
                        _TaskObject.Name = _CopyObject.Name;
                    }
                    else
                    {
                        _TaskObject.Name = Regex.Replace(_CopyObject.Name, "[^0-9a-zA-Z-_ (){}!£$%^&*,./#?@<>`:]+", "");
                    }


                    _TaskObject.EntityType = _CopyObject.EntityType;
                    _TaskObject.SeqNo = _CopyObject.SeqNo;
                    _TaskObject.SuppressStep = _CopyObject.SuppressStep;
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
                    _TaskObject.MilestoneStatus = _CopyObject.MilestoneStatus;
                    _TaskObject.OptionalDocument = _CopyObject.OptionalDocument;
                    _TaskObject.Agenda = _CopyObject.Agenda;
                    _TaskObject.CustomItem = _CopyObject.CustomItem;

                    if (_Option == "Insert")
                    {
                        _SelectedChapter.Items.Add(_TaskObject);
                    }

                    _SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(_SelectedChapter);
                    var returnChapterObject = ChapterManagementService.Update(_SelectedChapterObject);

                    _TaskObject = new GenSmartflowItem();
                    FilterText = "";

                    //keep track of time last updated ready for comparison by other sessions checking for updates
                    AppChapterState.SetLastUpdated(UserSession, _SelectedChapter);

                    _DataChanged.Invoke();
                    
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
            using (LogContext.PushProperty("SourceSystem", UserSession.SelectedSystem))
            using (LogContext.PushProperty("SourceCompanyId", UserSession.Company.Id))
            using (LogContext.PushProperty("SourceUserId", UserSession.User.Id))
            using (LogContext.PushProperty("SourceContext", nameof(ModalChapterDetail)))
            {
                Logger.LogError(e,"Error - Method: {0}, Message: {1}",_method, _message);
            }

            if(showNotificationMsg)
            {
                await NotificationManager.ShowNotification("Danger", $"Oops! Something went wrong.").ConfigureAwait(false);
            }
        }

    }

    
}
