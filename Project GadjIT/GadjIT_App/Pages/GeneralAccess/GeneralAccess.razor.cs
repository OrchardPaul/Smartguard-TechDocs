using System;
using System.Collections.Generic;
using System.Linq;
using Blazored.Modal;
using Blazored.Modal.Services;
using GadjIT_App.Pages.Shared.Modals;
using GadjIT_App.Services;
using GadjIT_App.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Serilog.Context;

namespace GadjIT_App.Pages.GeneralAccess
{
    public partial class GeneralAccess
    {
        [Inject]
        public IUserSessionState UserSession { get; set; }

        [Inject]
        private IGeneralAccessService GeneralAccessService { get; set; }

        [Inject]
        IModalService Modal { get; set; }

        [Inject]
        private ILogger<GeneralAccess> Logger { get; set; }

        public List<Dictionary<string,dynamic>> QueryResults { get; set; } = new List<Dictionary<string, dynamic>>();

        public DateTime TryDate;

        public string Query { get {return query;} set {QueryValidation(value);} }

        public string query = "";

        public string EntityRef { get; set; }

        public int MatterNo {get; set;} = 0;

        public bool FirstRun = true;

        /// <summary>
        /// Ensures results from query are limited
        /// </summary>
        /// <param name="newQuery"></param>
        public void QueryValidation(string newQuery)
        {
            if(!newQuery.ToUpper().Contains("ALTER") &&  !newQuery.ToUpper().Contains("CREATE"))
            {
                if(newQuery.ToUpper().Contains("SELECT"))
                {
                    if(!newQuery.ToUpper().Contains("TOP"))
                    {
                        query = newQuery.ToUpper().Replace("SELECT","SELECT TOP(200)");
                        return;
                    }
                }
            }

            query = newQuery;
        }


        /// <summary>
        /// Checks for dangerous elements before running the query
        /// </summary>
        /// <returns></returns>
        public void RunQuery()
        {
            if (Query.ToUpper()
                    .Contains("DELETE")
                || Query.ToUpper()
                    .Contains("UPDATE") 
                || Query.ToUpper()
                    .Contains("EXEC"))
            {
                ExecuteConfirm();
            }
            else
            {
                ExecuteQuery();
            }
        }

        /// <summary>
        /// Sends Query to API and retrieves results
        /// Gives out error messages if any are recieved from the API
        /// </summary>
        public async void ExecuteQuery()
        {
            try
            {
                var results = await GeneralAccessService.RunQuery(Query);
                bool IsError = false;
                bool procedureSuccess = false;

                if(results.FirstOrDefault() is null)
                {
                    IsError = true;

                    if(query.ToUpper().Contains("ALTER") || query.ToUpper().Contains("CREATE"))
                    {
                        procedureSuccess = true;
                        ShowSuccessModal("Procedure operation successfull");
                    }
                    else
                    {
                        ShowErrorModal("No results returned");
                    }
                }
                else if (results.FirstOrDefault().ContainsKey("ErrorFromApi"))
                {
                    IsError = true;
                    ShowErrorModal(Convert.ToString(results.FirstOrDefault()["ErrorFromApi"]));
                }
                else if (results.FirstOrDefault().ContainsKey("WarningFromApi"))
                {
                    ShowWarningModal(results.FirstOrDefault()["WarningFromApi"]);
                    results.Remove(results.FirstOrDefault());
                }

                
                if(!IsError)
                {
                    QueryResults = results;

                    if(FirstRun)
                    {
                        StateHasChanged();
                    }
                    else
                    {
                        await jsRuntime.InvokeAsync<object>("destroyDataTable", "#QueryResults");
                        StateHasChanged();
                    }

                    await jsRuntime.InvokeAsync<object>("loadDataTable", "#QueryResults");

                    FirstRun = false;

                    using (LogContext.PushProperty("SourceSystem", UserSession.selectedSystem))
                    using (LogContext.PushProperty("SourceCompanyId", UserSession.Company.Id))
                    using (LogContext.PushProperty("SourceUserId", UserSession.User.Id))
                    using (LogContext.PushProperty("SourceContext", nameof(GeneralAccess)))
                    {
                        Logger?.LogInformation($"Query successfully ran: {query}");
                    }
                }
                else if(procedureSuccess)
                {
                    using (LogContext.PushProperty("SourceSystem", UserSession.selectedSystem))
                    using (LogContext.PushProperty("SourceCompanyId", UserSession.Company.Id))
                    using (LogContext.PushProperty("SourceUserId", UserSession.User.Id))
                    using (LogContext.PushProperty("SourceContext", nameof(GeneralAccess)))
                    {
                        Logger?.LogInformation($"Procedure operation successfull");
                    }
                }
                else
                {
                    using (LogContext.PushProperty("SourceSystem", UserSession.selectedSystem))
                    using (LogContext.PushProperty("SourceCompanyId", UserSession.Company.Id))
                    using (LogContext.PushProperty("SourceUserId", UserSession.User.Id))
                    using (LogContext.PushProperty("SourceContext", nameof(GeneralAccess)))
                    {
                        Logger?.LogInformation($"Query unsuccessfully ran: {query}");
                    }
                }
            }
            catch(Exception e)
            {
                ShowErrorModal("Critical error from API, check if API is working or is the latest version");

                using (LogContext.PushProperty("SourceSystem", UserSession.selectedSystem))
                using (LogContext.PushProperty("SourceCompanyId", UserSession.Company.Id))
                using (LogContext.PushProperty("SourceUserId", UserSession.User.Id))
                using (LogContext.PushProperty("SourceContext", nameof(GeneralAccess)))
                {
                    Logger?.LogError(e, "Critical error retrieving query results");
                }
            }
        }

        /// <summary>
        /// Add references to entity ref and matter no to begining of query
        /// </summary>
        public void AddEntityRefMatterNo()
        {
            if(!query.ToUpper().Contains("DECLARE @MATTERNO"))
            {
                query = $"DECLARE @MATTERNO INT = {MatterNo} {System.Environment.NewLine}{query}";  
            }            

            if(!string.IsNullOrEmpty(EntityRef))
            {
                if(!query.ToUpper().Contains("DECLARE @ENTITYREF"))
                {
                    query = $"DECLARE @ENTITYREF VARCHAR(15) = '{EntityRef}' {System.Environment.NewLine}{query}";  
                }
            }

            StateHasChanged();
        }


        /// <summary>
        /// Brings up the confirm model
        /// </summary>
        protected void ExecuteConfirm()
        {
            string infoText = "Query contains possibly dangerous elements. Do you still wish to execute?";

            Action SelectedAction = ExecuteQuery;
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


        /// <summary>
        /// Brings up the error modal
        /// </summary>
        /// <param name="errorFromApi">description of the error</param>
        protected void ShowErrorModal(string errorFromApi)
        {
            string errorDesc = "Query resulted in an error for the following reason";

            var errorDetails = new List<string>{errorFromApi};

            var parameters = new ModalParameters();
            parameters.Add("ErrorDesc", errorDesc);
            parameters.Add("ErrorDetails", errorDetails);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-confirm"
            };

            Modal.Show<ModalErrorInfo>("Error", parameters, options);
        }

        /// <summary>
        /// Brings up the warning modal 
        /// </summary>
        /// <param name="infoText">Warning text to be displayed</param>
        protected void ShowWarningModal(string infoText)
        {
            var parameters = new ModalParameters();
            parameters.Add("InfoText", infoText);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-confirm"
            };

            Modal.Show<ModalInfo>("Warning", parameters, options);
        }

        protected void ShowSuccessModal(string infoText)
        {
            var parameters = new ModalParameters();
            parameters.Add("InfoText", infoText);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-confirm"
            };

            Modal.Show<ModalInfo>("Success", parameters, options);
        }




    }
}