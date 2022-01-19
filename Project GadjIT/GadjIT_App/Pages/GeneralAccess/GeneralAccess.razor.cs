using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazored.Modal;
using Blazored.Modal.Services;
using GadjIT_App.Pages.Shared.Modals;
using GadjIT_App.Services;
using GadjIT_App.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

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

        public string Query { get; set; }

        public bool FirstRun = true;

        /// <summary>
        /// Checks for dangerous elements before running the query
        /// </summary>
        /// <returns></returns>
        public void RunQuery()
        {
            if (Query.ToUpper().Contains("DELETE"))
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
        /// </summary>
        /// <returns></returns>
        public async void ExecuteQuery()
        {

            if(FirstRun)
            {
                QueryResults = await GeneralAccessService.RunQuery(Query);
                StateHasChanged();
            }
            else
            {

                QueryResults = await GeneralAccessService.RunQuery(Query);
                await jsRuntime.InvokeAsync<object>("destroyDataTable", "#QueryResults");
                StateHasChanged();
            }

            await jsRuntime.InvokeAsync<object>("loadDataTable", "#QueryResults");

            FirstRun = false;
        }

        /// <summary>
        /// Brings up confirm model
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

    }
}