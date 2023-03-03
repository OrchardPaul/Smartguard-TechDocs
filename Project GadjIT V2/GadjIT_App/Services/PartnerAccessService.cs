using GadjIT_App.Services.SessionState;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;
using GadjIT_ClientContext.Models.P4W;

namespace GadjIT_App.Services
{
    public interface IPartnerAccessService
    {
        Task<List<P4W_CaseTypes>> GetPartnerCaseTypes();
        Task<List<P4W_CaseTypeGroups>> GetPartnerCaseTypeGroups();
        Task<List<P4W_MpSysViews>> GetPartnerViews();
    }

    public class PartnerAccessService : IPartnerAccessService
    {
        private readonly HttpClient httpClient;
        private readonly IUserSessionState userSession;
        
        [Inject]
        private ILogger<PartnerAccessService> Logger {get; set;}

        public PartnerAccessService(HttpClient httpClient, IUserSessionState userSession)
        {
            this.httpClient = httpClient;
            this.userSession = userSession;
        }

        public async Task<List<P4W_CaseTypes>> GetPartnerCaseTypes()
        {
            List<P4W_CaseTypes> caseTypes = new List<P4W_CaseTypes>();
            try
            {
                HttpResponseMessage result = await httpClient.GetAsync($"{userSession.BaseUri}api/PartnerAccess/GetAllCaseTypes");

                if(result.IsSuccessStatusCode)
                {
                    caseTypes = await result.Content.ReadFromJsonAsync<List<P4W_CaseTypes>>();
                }
                else
                {
                    var errMsg = await result.Content.ReadAsStringAsync();
                    Logger.LogError($"GetPartnerCaseTypes: {errMsg}");
                }
            }
            catch (Exception e)
            {
                Logger.LogError($"GetPartnerCaseTypes: {e.Message}");
            }
            

            return caseTypes;
        }

        public Task<List<P4W_CaseTypeGroups>> GetPartnerCaseTypeGroups()
        {
            var result = httpClient.GetFromJsonAsync<List<P4W_CaseTypeGroups>>($"{userSession.BaseUri}api/PartnerAccess/GetAllCaseTypeGroups");

            return result;
        }

        public Task<List<P4W_MpSysViews>> GetPartnerViews()
        {
            var result = httpClient.GetFromJsonAsync<List<P4W_MpSysViews>>($"{userSession.BaseUri}api/PartnerAccess/GetAllP4WViews");

            return result;
        }
    }
}
