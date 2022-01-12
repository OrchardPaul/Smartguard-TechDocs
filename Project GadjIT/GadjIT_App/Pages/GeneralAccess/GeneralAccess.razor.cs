using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GadjIT_App.Services;
using GadjIT_App.Services.SessionState;
using Microsoft.AspNetCore.Components;

namespace GadjIT_App.Pages.GeneralAccess
{

    public partial class GeneralAccess
    {
        [Inject]
        public IUserSessionState UserSession { get; set; }

        [Inject]
        private IGeneralAccessService GeneralAccessService { get; set; }

        public List<Dictionary<string,dynamic>> QueryResults { get; set; } = new List<Dictionary<string, dynamic>>();

        public DateTime TryDate;

        public string Query { get; set; }

        /// <summary>
        /// Run query user typed in
        /// </summary>
        /// <returns></returns>
        public async Task RunQuery()
        {
            QueryResults = await GeneralAccessService.RunQuery(Query);

            StateHasChanged();
        }

    }
}