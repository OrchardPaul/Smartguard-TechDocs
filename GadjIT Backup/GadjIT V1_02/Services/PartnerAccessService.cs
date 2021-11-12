using GadjIT.ClientContext.P4W;
using GadjIT_V1_02.Services.SessionState;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace GadjIT_V1_02.Services
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

        public Task<List<CaseTypes>> GetPartnerCaseTypes()
        {
            var result = httpClient.GetJsonAsync<List<CaseTypes>>($"{userSession.baseUri}api/PartnerAccess/GetAllCaseTypes");

            if (result.Exception is null)
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        public Task<List<CaseTypeGroups>> GetPartnerCaseTypeGroups()
        {
            var result = httpClient.GetJsonAsync<List<CaseTypeGroups>>($"{userSession.baseUri}api/PartnerAccess/GetAllCaseTypeGroups");

            if (result.Exception is null)
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        public Task<List<MpSysViews>> GetPartnerViews()
        {
            var result = httpClient.GetJsonAsync<List<MpSysViews>>($"{userSession.baseUri}api/PartnerAccess/GetAllP4WViews");

            if (result.Exception is null)
            {
                return result;
            }
            else
            {
                return null;
            }
        }
    }
}
