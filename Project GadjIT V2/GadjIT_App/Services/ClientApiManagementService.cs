using GadjIT_ClientContext.Models;
using GadjIT_App.Data.Admin;
using GadjIT_App.Services.SessionState;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using GadjIT_ClientContext.Models.P4W;
using GadjIT_ClientContext.Models.Smartflow.Client;

namespace GadjIT_App.Services
{
    public interface IClientApiManagementService
    {
        Task<Client_SmartflowRecord> Add(Client_SmartflowRecord item);
        Task<Task<HttpResponseMessage>> Delete(int id);
        Task<Task<HttpResponseMessage>> DeleteChapter(int id);
        Task<List<Client_SmartflowRecord>> GetAllSmartflows();
        Task<List<string>> GetCaseTypeGroup();
        Task<List<string>> GetCaseTypes();
        Task<List<Client_SmartflowRecord>> GetSmartflowListByCaseType(string caseType);
        Task<List<P4W_DmDocuments>> GetDocumentList(string caseType);
        Task<Client_SmartflowRecord> Update(Client_SmartflowRecord item);
        Task<Client_SmartflowRecord> UpdateMainItem(Client_SmartflowRecord item);
        Task<List<Client_SmartflowRecord>> UpdateCaseType(string newCaseTypeName, string originalCaseTypeName, string caseTypeGroup);
        Task<List<Client_SmartflowRecord>> UpdateCaseTypeGroups(string newCaseTypeGroupName, string originalCaseTypeGroupName);
        Task<bool> CreateStep(P4W_SmartflowStepSchemaJSONObject stepSchemaJSONObject);
        Task<List<P4W_TableDate>> GetDatabaseTableDateFields();
        bool Lock { get; set; }

    }

    public class ClientApiManagementService : IClientApiManagementService
    {
        private readonly HttpClient HttpClient;
        private readonly IUserSessionState UserSession;
        private readonly ICompanyDbAccess CompanyDbAccess;
        private ILogger<ClientApiManagementService> Logger;

        public ClientApiManagementService(HttpClient _httpClient, IUserSessionState _userSession, ICompanyDbAccess _companyDbAccess, ILogger<ClientApiManagementService> _logger)
        {
            HttpClient = _httpClient;
            UserSession = _userSession;
            CompanyDbAccess = _companyDbAccess;
            Logger = _logger;

        }

        public bool Lock { get; set; } = false;


        private async Task HandleBadResponse(HttpResponseMessage response)
        {
            string respContent = await response.Content.ReadAsStringAsync();
            ExceptionModel exResponse = JsonConvert.DeserializeObject<ExceptionModel>(respContent);

            using (LogContext.PushProperty("SourceSystem", UserSession.SelectedSystem))
            using (LogContext.PushProperty("SourceCompanyId", UserSession.Company.Id))
            using (LogContext.PushProperty("SourceUserId", UserSession.User.Id))
            {
                Logger.LogError("API Error - Method: {0}, Message: {1}; Stack Trace: {2}",exResponse.Method, exResponse.Message, exResponse.StackTrace);
            }
        }

        public async Task<Client_SmartflowRecord> Add(Client_SmartflowRecord _smartFlow)
        {
            var content = new StringContent(JsonConvert.SerializeObject(_smartFlow), Encoding.UTF8, "application/json");  

            using HttpResponseMessage response = await HttpClient.PostAsync($"{UserSession.BaseUri}api/Smartflow/Add", content);

            if(response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Client_SmartflowRecord>();
            }
            else
            {
                await HandleBadResponse(response);
                return new Client_SmartflowRecord();
            }

        }

        public async Task<Client_SmartflowRecord> Update(Client_SmartflowRecord _smartFlow)
        {
            bool gotLock = CompanyDbAccess.Lock;
            while (gotLock)
            {
                await Task.Yield();
                gotLock = CompanyDbAccess.Lock;
            }


            await CompanyDbAccess.SaveSmartFlowRecordData(_smartFlow, UserSession);

            var content = new StringContent(JsonConvert.SerializeObject(_smartFlow), Encoding.UTF8, "application/json");  

            using var response = await HttpClient.PutAsync($"{UserSession.BaseUri}api/Smartflow/Update/{_smartFlow.Id}", content);

            return await response.Content.ReadFromJsonAsync<Client_SmartflowRecord>();

        }

        public async Task<Client_SmartflowRecord> UpdateMainItem(Client_SmartflowRecord _smartFlow)
        {
            await CompanyDbAccess.SaveSmartFlowRecord(_smartFlow, UserSession);

            var content = new StringContent(JsonConvert.SerializeObject(_smartFlow), Encoding.UTF8, "application/json");  

            using var response = await HttpClient.PutAsync($"{UserSession.BaseUri}api/Smartflow/Update/{_smartFlow.Id}", content);

            return await response.Content.ReadFromJsonAsync<Client_SmartflowRecord>();

        }



        public async Task<List<Client_SmartflowRecord>> UpdateCaseType(string newCaseTypeName, string originalCaseTypeName, string caseTypeGroup)
        {
            var _smartFlow = new Client_SmartflowRecord();

            var content = new StringContent(JsonConvert.SerializeObject(_smartFlow), Encoding.UTF8, "application/json");  

            using var response = await HttpClient.PostAsync($"{UserSession.BaseUri}api/Smartflow/UpdateCaseType/{newCaseTypeName}/{originalCaseTypeName}/{caseTypeGroup}", content);

            return await response.Content.ReadFromJsonAsync<List<Client_SmartflowRecord>>();
        }


        public async Task<List<Client_SmartflowRecord>> UpdateCaseTypeGroups(string newCaseTypeGroupName, string originalCaseTypeGroupName)
        {
            var _smartFlow = new Client_SmartflowRecord();

            var content = new StringContent(JsonConvert.SerializeObject(_smartFlow), Encoding.UTF8, "application/json");  

            using var response = await HttpClient.PostAsync($"{UserSession.BaseUri}api/Smartflow/UpdateCaseTypeGroups/{newCaseTypeGroupName}/{originalCaseTypeGroupName}", content);

            return await response.Content.ReadFromJsonAsync<List<Client_SmartflowRecord>>();
        }

        public async Task<Task<HttpResponseMessage>> Delete(int id)
        {
            await CompanyDbAccess.RemoveSmartFlowRecord(id, UserSession);

            return HttpClient.DeleteAsync($"{UserSession.BaseUri}api/Smartflow/Delete/{id}");
        }

        public async Task<Task<HttpResponseMessage>> DeleteChapter(int id)
        {
            await CompanyDbAccess.RemoveSmartFlowRecord(id, UserSession);

            return HttpClient.DeleteAsync($"{UserSession.BaseUri}api/Smartflow/DeleteChapter/{id}");
        }

        public async Task<List<Client_SmartflowRecord>> GetAllSmartflows()
        {

            var response = await HttpClient.GetAsync($"{UserSession.BaseUri}api/Smartflow/GetAllSmartflows");

            if(response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Client_SmartflowRecord>>();
            }
            else
            {
                if(response.ReasonPhrase == "Not Found") //check for old API call to Chapters(pre V2)
                {
                    response = await HttpClient.GetAsync($"{UserSession.BaseUri}api/Smartflow/GetAllChapters");

                    if(response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadFromJsonAsync<List<Client_SmartflowRecord>>();
                    }
                    else
                    {
                        await HandleBadResponse(response);
                        throw new HttpRequestException("Error retrieving Chapters from the client");
                    }
                }
                else
                {
                    await HandleBadResponse(response);
                    throw new HttpRequestException("Error retrieving Smartflows from the client");
                }
            }

        }

        public async Task<List<Client_SmartflowRecord>> GetAllChapters()
        {

            var response = await HttpClient.GetAsync($"{UserSession.BaseUri}api/Smartflow/GetAllSmartflows");

            if(response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Client_SmartflowRecord>>();
            }
            else
            {
                await HandleBadResponse(response);
                throw new HttpRequestException("Error retrieving Smartflows from the client");
            }

        }

        public async Task<List<P4W_TableDate>> GetDatabaseTableDateFields()
        {
            return await HttpClient.GetFromJsonAsync<List<P4W_TableDate>>($"{UserSession.BaseUri}api/Smartflow/GetDatabaseTableDateFields");
        }

        public async Task<List<P4W_DmDocuments>> GetDocumentList(string caseType)
        {
            Lock = true;
            
            var returnValue = await HttpClient.GetFromJsonAsync<List<P4W_DmDocuments>>($"{UserSession.BaseUri}api/Smartflow/GetDocumentList/{caseType}");
            
            Lock = false;

            return returnValue;
        }

        public async Task<List<P4W_DmDocuments>> GetDocumentListByCaseTypeGroup(int caseTypeGroupRef)
        {
            return await HttpClient.GetFromJsonAsync<List<P4W_DmDocuments>>($"{UserSession.BaseUri}api/Smartflow/GetDocumentListByCaseTypeGroupRef/{caseTypeGroupRef}");
        }

        public async Task<List<string>> GetCaseTypeGroup()
        {
            return await HttpClient.GetFromJsonAsync<List<string>>($"{UserSession.BaseUri}api/Smartflow/GetCaseTypeGroup");
        }

        public async Task<List<string>> GetCaseTypes()
        {
            return await HttpClient.GetFromJsonAsync<List<string>>($"{UserSession.BaseUri}api/Smartflow/GetCaseTypes");
        }

        public async Task<List<Client_SmartflowRecord>> GetSmartflowListByCaseType(string caseType)
        {
            return await HttpClient.GetFromJsonAsync<List<Client_SmartflowRecord>>($"{UserSession.BaseUri}api/Smartflow/GetSmartflowListByCaseType/{caseType}");
        }

        public async Task<bool> CreateStep(P4W_SmartflowStepSchemaJSONObject _stepSchemaJSONObject)
        {
            Lock = true;

            var content = new StringContent(JsonConvert.SerializeObject(_stepSchemaJSONObject), Encoding.UTF8, "application/json");  

            using var response = await HttpClient.PostAsync($"{UserSession.BaseUri}api/Smartflow/CreateStep", content);

            Lock = false;

            return await response.Content.ReadFromJsonAsync<bool>();
        }


        

    }

}
