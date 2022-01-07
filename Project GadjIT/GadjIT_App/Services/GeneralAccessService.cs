using GadjIT.ClientContext.P4W;
using GadjIT.ClientContext.P4W.Custom;
using GadjIT.ClientContext.P4W.Functions;
using GadjIT_App.Data.Admin;
using GadjIT_App.Services.SessionState;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;


namespace GadjIT_App.Services
{
    public interface IGeneralAccessService
    {
        Task<List<MpSysViews>> GetPartnerViews();

    }

    public class GeneralAccessService : IGeneralAccessService
    {
        private readonly HttpClient httpClient;
        private readonly IUserSessionState userSession;

        public GeneralAccessService(HttpClient httpClient, IUserSessionState userSession)
        {
            this.httpClient = httpClient;
            this.userSession = userSession;
        }

        public async Task<List<MpSysViews>> GetPartnerViews()
        {
            var sql = new SQLRequest { Query = "SELECT * FROM Mp_Sys_Views"};

            using var response = await httpClient.PutAsJsonAsync($"{userSession.baseUri}api/GeneralAccess/GetListOfData", sql);

            var results = await response.Content.ReadFromJsonAsync<IEnumerable<MpSysViews>>();

            return results.ToList();

        }


    }
}
