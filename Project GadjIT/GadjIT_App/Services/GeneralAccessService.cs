using GadjIT.ClientContext.P4W;
using GadjIT.ClientContext.P4W.Custom;
using GadjIT.ClientContext.P4W.Functions;
using GadjIT_App.Data.Admin;
using GadjIT_App.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
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
        Task<List<Dictionary<string,dynamic>>> RunQuery(string _query);

    }

    public class GeneralAccessService : IGeneralAccessService
    {
        [Inject]
        private ILogger<GeneralAccessService> Logger { get; set; }

        private readonly HttpClient httpClient;
        private readonly IUserSessionState userSession;

        public GeneralAccessService(HttpClient httpClient, IUserSessionState userSession)
        {
            this.httpClient = httpClient;
            this.userSession = userSession;
        }

        public async Task<List<Dictionary<string,dynamic>>> RunQuery(string _query)
        {
            var sql = new SQLRequest { Query = _query};

            using var response = await httpClient.PutAsJsonAsync($"{userSession.baseUri}api/GeneralAccess/GetListOfData", sql);

            List<Dictionary<string,dynamic>> results = new List<Dictionary<string, dynamic>>();

            if(response.IsSuccessStatusCode)
            {
                var resultsfromApi = await response.Content.ReadFromJsonAsync<IEnumerable<Dictionary<string,dynamic>>>();

                if (resultsfromApi.Count() > 200)
                {
                    results.Add(new Dictionary<string,dynamic>
                    {
                        {"WarningFromApi","Results from query exceed 200 rows, not all will be displayed"}
                    });
                    
                    results.AddRange(resultsfromApi.Take(200).ToList());
                }
                else
                {
                    results = resultsfromApi.ToList();
                }
            }
            else
            {
                var errorReason = await response.Content.ReadFromJsonAsync<Dictionary<string,dynamic>>();

                results = new List<Dictionary<string,dynamic>>
                {
                    new Dictionary<string,dynamic>
                    {
                        {"ErrorFromApi",errorReason["detail"]}
                    }
                };
            }

            return results;

        }


    }
}
