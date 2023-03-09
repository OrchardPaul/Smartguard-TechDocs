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

namespace GadjIT_App.Pages.Smartflows.ComponentsSmartflowDetail._Status
{

    public partial class ModalSmartflowStatusDetail : ComponentBase
    {

        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }


        [Parameter]
        public string _Option { get; set; }


        [Parameter]
        public SmartflowV2 _SelectedSmartflow { get; set; }

        [Parameter]
        public SmartflowStatus _TaskObject { get; set; }

        [Parameter]
        public SmartflowStatus _CopyObject { get; set; }


        [Parameter]
        public Action _DataChanged { get; set; } 

        [Parameter]
        public List<VmSmartflowStatus> _ListOfStatus { get; set; } = new List<VmSmartflowStatus>();

                
        [Inject]
        ILogger<ModalSmartflowStatusDetail> Logger {get; set;}

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

        public bool _useCustomMilestone { get; set; }
        public bool UseCustomMilestone
        {
            get { return _useCustomMilestone; }
            set
            {
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

       
        private async void Close()
        {
            _TaskObject = new SmartflowStatus();
            await ModalInstance.CloseAsync();


        }

        private void HandleValidSubmit()
        {
            try
            {

                
                if (_SelectedSmartflow.Status.Where(I => I.Name != _CopyObject.Name).Select(I => I.Name).Contains(_CopyObject.Name))
                {
                    Error = 1;
                    StateHasChanged();
                }
                else
                {

                    
                    _TaskObject.Name = Regex.Replace(_CopyObject.Name, "[^0-9a-zA-Z-_ (){}!£$%^&*,./#?@<>`:]+", "");
                    _TaskObject.SeqNo = _CopyObject.SeqNo;
                    _TaskObject.SuppressStep = _CopyObject.SuppressStep;
                    _TaskObject.MilestoneStatus = _CopyObject.MilestoneStatus;
                    
                    if (_Option == "Insert")
                    {
                        _SelectedSmartflow.Status.Add(_TaskObject);
                    }

                    _TaskObject = new SmartflowStatus();
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
            using (LogContext.PushProperty("SourceContext", nameof(ModalSmartflowStatusDetail)))
            {
                Logger.LogError(e,"Error - Method: {0}, Message: {1}",_method, _message);
            }

        }

    }

    
}
