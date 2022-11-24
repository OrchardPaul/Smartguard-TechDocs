using GadjIT_ClientContext.P4W;
using GadjIT_ClientContext.P4W.Custom;
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
using System;

namespace GadjIT_App.Services
{
    public interface IChapterManagementService
    {
        Task<UsrOrsfSmartflows> Add(UsrOrsfSmartflows item);
        Task<Task<HttpResponseMessage>> Delete(int id);
        Task<Task<HttpResponseMessage>> DeleteChapter(int id);
        Task<List<UsrOrsfSmartflows>> GetAllChapters();
        Task<List<string>> GetCaseTypeGroup();
        Task<List<string>> GetCaseTypes();
        Task<List<UsrOrsfSmartflows>> GetChapterListByCaseType(string caseType);
        Task<List<DmDocuments>> GetDocumentList(string caseType);
        Task<UsrOrsfSmartflows> Update(UsrOrsfSmartflows item);
        Task<UsrOrsfSmartflows> UpdateMainItem(UsrOrsfSmartflows item);
        Task<List<UsrOrsfSmartflows>> UpdateCaseType(string newCaseTypeName, string originalCaseTypeName, string caseTypeGroup);
        Task<List<UsrOrsfSmartflows>> UpdateCaseTypeGroups(string newCaseTypeGroupName, string originalCaseTypeGroupName);
        Task<bool> CreateStep(VmChapterP4WStepSchemaJSONObject stepSchemaJSONObject);
        Task<List<TableDate>> GetDatabaseTableDateFields();
        bool Lock { get; set; }

    }

    public class ChapterManagementService : IChapterManagementService
    {
        private readonly HttpClient HttpClient;
        private readonly IUserSessionState UserSession;
        private readonly ICompanyDbAccess CompanyDbAccess;
        private ILogger<ChapterManagementService> Logger;

        public ChapterManagementService(HttpClient _httpClient, IUserSessionState _userSession, ICompanyDbAccess _companyDbAccess, ILogger<ChapterManagementService> _logger)
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

            using (LogContext.PushProperty("SourceSystem", UserSession.selectedSystem))
            using (LogContext.PushProperty("SourceCompanyId", UserSession.Company.Id))
            using (LogContext.PushProperty("SourceUserId", UserSession.User.Id))
            {
                Logger.LogError("API Error - Method: {0}, Message: {1}; Stack Trace: {2}",exResponse.Method, exResponse.Message, exResponse.StackTrace);
            }
        }

        public async Task<UsrOrsfSmartflows> Add(UsrOrsfSmartflows _smartFlow)
        {
            var content = new StringContent(JsonConvert.SerializeObject(_smartFlow), Encoding.UTF8, "application/json");  

            using HttpResponseMessage response = await HttpClient.PostAsync($"{UserSession.baseUri}api/Smartflow/Add", content);

            if(response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<UsrOrsfSmartflows>();
            }
            else
            {
                await HandleBadResponse(response);
                return new UsrOrsfSmartflows();
            }

        }

        public async Task<UsrOrsfSmartflows> Update(UsrOrsfSmartflows _smartFlow)
        {
            bool gotLock = CompanyDbAccess.Lock;
            while (gotLock)
            {
                await Task.Yield();
                gotLock = CompanyDbAccess.Lock;
            }


            await CompanyDbAccess.SaveSmartFlowRecordData(_smartFlow, UserSession);

            var content = new StringContent(JsonConvert.SerializeObject(_smartFlow), Encoding.UTF8, "application/json");  

            using var response = await HttpClient.PutAsync($"{UserSession.baseUri}api/Smartflow/Update/{_smartFlow.Id}", content);

            return await response.Content.ReadFromJsonAsync<UsrOrsfSmartflows>();

        }

        public async Task<UsrOrsfSmartflows> UpdateMainItem(UsrOrsfSmartflows _smartFlow)
        {
            await CompanyDbAccess.SaveSmartFlowRecord(_smartFlow, UserSession);

            var content = new StringContent(JsonConvert.SerializeObject(_smartFlow), Encoding.UTF8, "application/json");  

            using var response = await HttpClient.PutAsync($"{UserSession.baseUri}api/Smartflow/Update/{_smartFlow.Id}", content);

            return await response.Content.ReadFromJsonAsync<UsrOrsfSmartflows>();

        }



        public async Task<List<UsrOrsfSmartflows>> UpdateCaseType(string newCaseTypeName, string originalCaseTypeName, string caseTypeGroup)
        {
            var _smartFlow = new UsrOrsfSmartflows();

            var content = new StringContent(JsonConvert.SerializeObject(_smartFlow), Encoding.UTF8, "application/json");  

            using var response = await HttpClient.PostAsync($"{UserSession.baseUri}api/Smartflow/UpdateCaseType/{newCaseTypeName}/{originalCaseTypeName}/{caseTypeGroup}", content);

            return await response.Content.ReadFromJsonAsync<List<UsrOrsfSmartflows>>();
        }


        public async Task<List<UsrOrsfSmartflows>> UpdateCaseTypeGroups(string newCaseTypeGroupName, string originalCaseTypeGroupName)
        {
            var _smartFlow = new UsrOrsfSmartflows();

            var content = new StringContent(JsonConvert.SerializeObject(_smartFlow), Encoding.UTF8, "application/json");  

            using var response = await HttpClient.PostAsync($"{UserSession.baseUri}api/Smartflow/UpdateCaseTypeGroups/{newCaseTypeGroupName}/{originalCaseTypeGroupName}", content);

            return await response.Content.ReadFromJsonAsync<List<UsrOrsfSmartflows>>();
        }

        public async Task<Task<HttpResponseMessage>> Delete(int id)
        {
            await CompanyDbAccess.RemoveSmartFlowRecord(id, UserSession);

            return HttpClient.DeleteAsync($"{UserSession.baseUri}api/Smartflow/Delete/{id}");
        }

        public async Task<Task<HttpResponseMessage>> DeleteChapter(int id)
        {
            await CompanyDbAccess.RemoveSmartFlowRecord(id, UserSession);

            return HttpClient.DeleteAsync($"{UserSession.baseUri}api/Smartflow/DeleteChapter/{id}");
        }

        public async Task<List<UsrOrsfSmartflows>> GetAllChapters()
        {

            var response = await HttpClient.GetAsync($"{UserSession.baseUri}api/Smartflow/GetAllChapters");

            if(response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<UsrOrsfSmartflows>>();
            }
            else
            {
                await HandleBadResponse(response);
                throw new HttpRequestException("Error retrieving Smartflows from the client");
            }

        }
        public async Task<List<TableDate>> GetDatabaseTableDateFields()
        {
            return await HttpClient.GetFromJsonAsync<List<TableDate>>($"{UserSession.baseUri}api/Smartflow/GetDatabaseTableDateFields");
        }

        public async Task<List<DmDocuments>> GetDocumentList(string caseType)
        {
            Lock = true;
            
            var returnValue = await HttpClient.GetFromJsonAsync<List<DmDocuments>>($"{UserSession.baseUri}api/Smartflow/GetDocumentList/{caseType}");
            
            Lock = false;

            return returnValue;
        }

        public async Task<List<DmDocuments>> GetDocumentListByCaseTypeGroup(int caseTypeGroupRef)
        {
            return await HttpClient.GetFromJsonAsync<List<DmDocuments>>($"{UserSession.baseUri}api/Smartflow/GetDocumentListByCaseTypeGroupRef/{caseTypeGroupRef}");
        }

        public async Task<List<string>> GetCaseTypeGroup()
        {
            return await HttpClient.GetFromJsonAsync<List<string>>($"{UserSession.baseUri}api/Smartflow/GetCaseTypeGroup");
        }

        public async Task<List<string>> GetCaseTypes()
        {
            return await HttpClient.GetFromJsonAsync<List<string>>($"{UserSession.baseUri}api/Smartflow/GetCaseTypes");
        }

        public async Task<List<UsrOrsfSmartflows>> GetChapterListByCaseType(string caseType)
        {
            return await HttpClient.GetFromJsonAsync<List<UsrOrsfSmartflows>>($"{UserSession.baseUri}api/Smartflow/GetChapterListByCaseType/{caseType}");
        }

        public async Task<bool> CreateStep(VmChapterP4WStepSchemaJSONObject _stepSchemaJSONObject)
        {
            Lock = true;

            var content = new StringContent(JsonConvert.SerializeObject(_stepSchemaJSONObject), Encoding.UTF8, "application/json");  

            using var response = await HttpClient.PostAsync($"{UserSession.baseUri}api/Smartflow/CreateStep", content);

            Lock = false;

            return await response.Content.ReadFromJsonAsync<bool>();
        }


        

    }

}
