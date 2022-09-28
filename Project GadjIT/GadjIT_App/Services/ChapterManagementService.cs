using GadjIT_ClientContext.P4W;
using GadjIT_ClientContext.P4W.Custom;
using GadjIT_App.Data.Admin;
using GadjIT_App.Services.SessionState;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;


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
        private readonly HttpClient httpClient;
        private readonly IUserSessionState userSession;
        private readonly ICompanyDbAccess companyDbAccess;

        public ChapterManagementService(HttpClient httpClient, IUserSessionState userSession, ICompanyDbAccess companyDbAccess)
        {
            this.httpClient = httpClient;
            this.userSession = userSession;
            this.companyDbAccess = companyDbAccess;
        }

        public bool Lock { get; set; } = false;



        public async Task<UsrOrsfSmartflows> Add(UsrOrsfSmartflows _smartFlow)
        {
            var content = new StringContent(JsonConvert.SerializeObject(_smartFlow), Encoding.UTF8, "application/json");  

            using HttpResponseMessage response = await httpClient.PostAsync($"{userSession.baseUri}api/ChapterManagement/Add", content);

            return await response.Content.ReadFromJsonAsync<UsrOrsfSmartflows>();

        }

        public async Task<UsrOrsfSmartflows> Update(UsrOrsfSmartflows _smartFlow)
        {
            bool gotLock = companyDbAccess.Lock;
            while (gotLock)
            {
                await Task.Yield();
                gotLock = companyDbAccess.Lock;
            }


            await companyDbAccess.SaveSmartFlowRecordData(_smartFlow, userSession);

            var content = new StringContent(JsonConvert.SerializeObject(_smartFlow), Encoding.UTF8, "application/json");  

            using var response = await httpClient.PutAsync($"{userSession.baseUri}api/ChapterManagement/Update/{_smartFlow.Id}", content);

            return await response.Content.ReadFromJsonAsync<UsrOrsfSmartflows>();

        }

        public async Task<UsrOrsfSmartflows> UpdateMainItem(UsrOrsfSmartflows _smartFlow)
        {
            await companyDbAccess.SaveSmartFlowRecord(_smartFlow, userSession);

            var content = new StringContent(JsonConvert.SerializeObject(_smartFlow), Encoding.UTF8, "application/json");  

            using var response = await httpClient.PutAsync($"{userSession.baseUri}api/ChapterManagement/Update/{_smartFlow.Id}", content);

            return await response.Content.ReadFromJsonAsync<UsrOrsfSmartflows>();

        }



        public async Task<List<UsrOrsfSmartflows>> UpdateCaseType(string newCaseTypeName, string originalCaseTypeName, string caseTypeGroup)
        {
            var _smartFlow = new UsrOrsfSmartflows();

            var content = new StringContent(JsonConvert.SerializeObject(_smartFlow), Encoding.UTF8, "application/json");  

            using var response = await httpClient.PostAsync($"{userSession.baseUri}api/ChapterManagement/UpdateCaseType/{newCaseTypeName}/{originalCaseTypeName}/{caseTypeGroup}", content);

            return await response.Content.ReadFromJsonAsync<List<UsrOrsfSmartflows>>();
        }


        public async Task<List<UsrOrsfSmartflows>> UpdateCaseTypeGroups(string newCaseTypeGroupName, string originalCaseTypeGroupName)
        {
            var _smartFlow = new UsrOrsfSmartflows();

            var content = new StringContent(JsonConvert.SerializeObject(_smartFlow), Encoding.UTF8, "application/json");  

            using var response = await httpClient.PostAsync($"{userSession.baseUri}api/ChapterManagement/UpdateCaseTypeGroups/{newCaseTypeGroupName}/{originalCaseTypeGroupName}", content);

            return await response.Content.ReadFromJsonAsync<List<UsrOrsfSmartflows>>();
        }

        public async Task<Task<HttpResponseMessage>> Delete(int id)
        {
            await companyDbAccess.RemoveSmartFlowRecord(id, userSession);

            return httpClient.DeleteAsync($"{userSession.baseUri}api/ChapterManagement/Delete/{id}");
        }

        public async Task<Task<HttpResponseMessage>> DeleteChapter(int id)
        {
            await companyDbAccess.RemoveSmartFlowRecord(id, userSession);

            return httpClient.DeleteAsync($"{userSession.baseUri}api/ChapterManagement/DeleteChapter/{id}");
        }

        public async Task<List<UsrOrsfSmartflows>> GetAllChapters()
        {

            return await httpClient.GetFromJsonAsync<List<UsrOrsfSmartflows>>($"{userSession.baseUri}api/ChapterManagement/GetAllChapters");

        }
        public async Task<List<TableDate>> GetDatabaseTableDateFields()
        {
            return await httpClient.GetFromJsonAsync<List<TableDate>>($"{userSession.baseUri}api/ChapterManagement/GetDatabaseTableDateFields");
        }

        public async Task<List<DmDocuments>> GetDocumentList(string caseType)
        {
            Lock = true;
            
            var returnValue = await httpClient.GetFromJsonAsync<List<DmDocuments>>($"{userSession.baseUri}api/ChapterManagement/GetDocumentList/{caseType}");
            
            Lock = false;

            return returnValue;
        }

        public async Task<List<DmDocuments>> GetDocumentListByCaseTypeGroup(int caseTypeGroupRef)
        {
            return await httpClient.GetFromJsonAsync<List<DmDocuments>>($"{userSession.baseUri}api/ChapterManagement/GetDocumentListByCaseTypeGroupRef/{caseTypeGroupRef}");
        }

        public async Task<List<string>> GetCaseTypeGroup()
        {
            return await httpClient.GetFromJsonAsync<List<string>>($"{userSession.baseUri}api/ChapterManagement/GetCaseTypeGroup");
        }

        public async Task<List<string>> GetCaseTypes()
        {
            return await httpClient.GetFromJsonAsync<List<string>>($"{userSession.baseUri}api/ChapterManagement/GetCaseTypes");
        }

        public async Task<List<UsrOrsfSmartflows>> GetChapterListByCaseType(string caseType)
        {
            return await httpClient.GetFromJsonAsync<List<UsrOrsfSmartflows>>($"{userSession.baseUri}api/ChapterManagement/GetChapterListByCaseType/{caseType}");
        }

        public async Task<bool> CreateStep(VmChapterP4WStepSchemaJSONObject _stepSchemaJSONObject)
        {
            Lock = true;

            var content = new StringContent(JsonConvert.SerializeObject(_stepSchemaJSONObject), Encoding.UTF8, "application/json");  

            using var response = await httpClient.PostAsync($"{userSession.baseUri}api/ChapterManagement/CreateStep", content);

            Lock = false;

            return await response.Content.ReadFromJsonAsync<bool>();
        }


        

    }

}
