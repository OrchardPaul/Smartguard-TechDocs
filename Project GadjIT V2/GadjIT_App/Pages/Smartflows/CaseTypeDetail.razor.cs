using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GadjIT_App.Services;
using GadjIT_App.Services.SessionState;
using Microsoft.JSInterop;
using System.Timers;
using GadjIT_App.Services.AppState;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using Microsoft.Extensions.Configuration;
using GadjIT_ClientContext.Models.Smartflow;
using GadjIT_ClientContext.Models.Smartflow.Client;

namespace GadjIT_App.Pages.Smartflows
{
    public partial class CaseTypeDetail
    {
        [Parameter]
        public List<Client_VmSmartflowRecord> _LstVmClientSmartflowRecord { get; set; }

        [Parameter]
        public string _SelectedCaseTypeGroup { get; set; }

        [Parameter]
        public string _SelectedCaseType { get; set; }

        [Parameter]
        public EventCallback _SelectHome {get; set;}

        [Parameter]
        public EventCallback _RefreshSmartflowsTask {get; set;}

        [Parameter]
        public EventCallback<Client_SmartflowRecord> _SelectSmartflow {get; set;}

        [Parameter]
        public EventCallback _SaveSelectedCaseType {get; set;}

        [Inject]
        private IJSRuntime JSRuntime {get; set;}

        [Inject]
        public INotificationManager NotificationManager {get; set;}



        [Inject]
        public IUserSessionState UserSession { get; set; }


        [Inject]
        public IAppSmartflowsState AppSmartflowsState { get; set; }


        [Inject]
        private ILogger<CaseTypeDetail> Logger { get; set; }

        [Inject]
        public IConfiguration Configuration { get; set;}

        
        

        public String UserGuideURL => Configuration["AppSettings:UserGuideURL"];
        
        


        public Smartflow SelectedSmartflow { get; set; } = new Smartflow { Items = new List<GenSmartflowItem>() }; //SmartflowData

        int RowChanged { get; set; } = 0; //moved partial


        public string NavDisplay = "Chapter";


        protected bool CompareSystems = false;

        public bool DisplaySpinner = true;







#region Page Events
        protected override async Task OnInitializedAsync()
        {
            NavDisplay = "CaseType";

            await SelectCaseType();

        }

        //private Timer timer;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                try
                {
                    await JSRuntime.InvokeVoidAsync("showPageAfterFirstRender");
                }
                catch (Exception e)
                {
                    GenericErrorLog(false,e, "OnAfterRenderAsync", e.Message);  
                }
            }
            else
            {
                //TODO: reinstate after testing
                // timer = new Timer();
                // timer.Interval = 10000; //10 seconds
                // timer.Elapsed += OnTimerInterval;
                // timer.AutoReset = true;
                // // Start the timer
                // timer.Enabled = true;
            }
            base.OnAfterRender(firstRender);
        }

        private bool Disposed = false;

        public void Dispose()
        {
            if(!Disposed)
            {
                //AppSmartflowsState.DisposeUser(UserSession.User);
            }
        }

#endregion

#region ParentEvents 


        public async void SelectHome()
        {
            await _SelectHome.InvokeAsync();

            Disposed = true;
        }

        public async Task RefreshSmartflowsTask()
        {
            await _RefreshSmartflowsTask.InvokeAsync();
        }

        public async Task SelectSmartflow(Client_SmartflowRecord smartflow)
        {
            
            await _SelectSmartflow.InvokeAsync(smartflow);
         
        }
        


    

#endregion

#region Chapter Details

        private async Task SelectCaseType()
        {
            try
            {
                
                DisplaySpinner = true;

                NavDisplay = "CaseType";
                


            }
            catch (Exception ex)
            {

                GenericErrorLog(true,ex, "SelectCaseType", $"Loading selected Case Type: {ex.Message}");

            }
            finally
            {
                DisplaySpinner = false;

                StateHasChanged();


            }


        }


        private async Task ChaseTypeUpdated(string listType)
        {

            // RefreshSmartflowItems(listType);
            
            // _SelectedSmartflowObject.SmartflowObject.SmartflowData = JsonConvert.SerializeObject(SelectedSmartflow);
            // await ClientApiManagementService.Update(_SelectedSmartflowObject.SmartflowObject);

            // AppSmartflowsState.SetLastUpdated(UserSession, SelectedSmartflow);
            
        }


        
        protected void ShowNav(string displayChange)
        {
            CompareSystems = false;
            RowChanged = 0;

            NavDisplay = displayChange;
        }

        

#endregion


#region Error Handling



        /****************************************/
        /* ERROR HANDLING */
        /****************************************/
        private void GenericErrorLog(bool showNotificationMsg, Exception e, string _method, string _message)
        {
            if(showNotificationMsg)
            {
                NotificationManager.ShowNotification("Danger", $"Oops! Something went wrong.");
            }

            using (LogContext.PushProperty("SourceSystem", UserSession.SelectedSystem))
            using (LogContext.PushProperty("SourceCompanyId", UserSession.Company.Id))
            using (LogContext.PushProperty("SourceUserId", UserSession.User.Id))
            using (LogContext.PushProperty("SourceContext", nameof(CaseTypeDetail)))
            {
                Logger.LogError(e,"Error - Method: {0}, Message: {1}",_method, _message);
            }

        }


#endregion

    }
}
