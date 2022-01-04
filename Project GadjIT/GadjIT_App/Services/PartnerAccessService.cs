using GadjIT.ClientContext.P4W;
using GadjIT_App.Services.SessionState;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace GadjIT_App.Services
{
    public interface IPartnerAccessService
    {
        Task<List<CaseTypes>> GetPartnerCaseTypes();
        Task<List<CaseTypeGroups>> GetPartnerCaseTypeGroups();
        Task<List<MpSysViews>> GetPartnerViews();
    }

    public class PartnerAccessService : IPartnerAccessService
    {
        private readonly HttpClient httpClient;
        private readonly IUserSessionState userSession;

        public PartnerAccessService(HttpClient httpClient, IUserSessionState userSession)
        {
            this.httpClient = httpClient;
            this.userSession = userSession;
        }

        public async Task<List<CaseTypes>> GetPartnerCaseTypes()
        {
            var result = await httpClient.GetFromJsonAsync<List<CaseTypes>>($"{userSession.baseUri}api/PartnerAccess/GetAllCaseTypes");

            return result;
        }

        public Task<List<CaseTypeGroups>> GetPartnerCaseTypeGroups()
        {
            var result = httpClient.GetFromJsonAsync<List<CaseTypeGroups>>($"{userSession.baseUri}api/PartnerAccess/GetAllCaseTypeGroups");

            return result;
        }

        public Task<List<MpSysViews>> GetPartnerViews()
        {
            var result = httpClient.GetFromJsonAsync<List<MpSysViews>>($"{userSession.baseUri}api/PartnerAccess/GetAllP4WViews");

            return result;
        }
    }
}
