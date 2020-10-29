using Gizmo.Context.OR_RESI;
using Gizmo_V1_02.Services.SessionState;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Services
{
    public interface IPartnerAccessService
    {
        Task<List<CaseTypes>> GetPartnerCaseTypes();
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
    }
}
