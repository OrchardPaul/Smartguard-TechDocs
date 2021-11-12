using GadjIT.ClientContext.P4W;
using GadjIT.ClientContext.P4W.Custom;
using GadjIT.ClientContext.P4W.Functions;
using GadjIT_V1_02.Data.Admin;
using GadjIT_V1_02.Services.SessionState;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace GadjIT_V1_02.Services
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


        public async Task<UsrOrsfSmartflows> Add(UsrOrsfSmartflows item)
        {
            return await httpClient.PostJsonAsync<UsrOrsfSmartflows>($"{userSession.baseUri}api/ChapterManagement/Add", item);
        }

        public async Task<UsrOrsfSmartflows> Update(UsrOrsfSmartflows item)
        {
            bool gotLock = companyDbAccess.Lock;
            while (gotLock)
            {
                await Task.Yield();
                gotLock = companyDbAccess.Lock;
            }


            await companyDbAccess.SaveSmartFlowRecordData(item, userSession);

            return await httpClient.PutJsonAsync<UsrOrsfSmartflows>($"{userSession.baseUri}api/ChapterManagement/Update/{item.Id}", item);
        }

        public async Task<UsrOrsfSmartflows> UpdateMainItem(UsrOrsfSmartflows item)
        {
            await companyDbAccess.SaveSmartFlowRecord(item, userSession);

            return await httpClient.PutJsonAsync<UsrOrsfSmartflows>($"{userSession.baseUri}api/ChapterManagement/Update/{item.Id}", item);
        }



        public Task<List<UsrOrsfSmartflows>> UpdateCaseType(string newCaseTypeName, string originalCaseTypeName, string caseTypeGroup)
        {
            var item = new UsrOrsfSmartflows();
            return httpClient.PutJsonAsync<List<UsrOrsfSmartflows>>($"{userSession.baseUri}api/ChapterManagement/UpdateCaseType/{newCaseTypeName}/{originalCaseTypeName}/{caseTypeGroup}", item);
        }


        public Task<List<UsrOrsfSmartflows>> UpdateCaseTypeGroups(string newCaseTypeGroupName, string originalCaseTypeGroupName)
        {
            var item = new UsrOrsfSmartflows();
            return httpClient.PutJsonAsync<List<UsrOrsfSmartflows>>($"{userSession.baseUri}api/ChapterManagement/UpdateCaseTypeGroups/{newCaseTypeGroupName}/{originalCaseTypeGroupName}", item);
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

        public Task<List<UsrOrsfSmartflows>> GetAllChapters()
        {
            return httpClient.GetJsonAsync<List<UsrOrsfSmartflows>>($"{userSession.baseUri}api/ChapterManagement/GetAllChapters");
        }
        public Task<List<TableDate>> GetDatabaseTableDateFields()
        {
            return httpClient.GetJsonAsync<List<TableDate>>($"{userSession.baseUri}api/ChapterManagement/GetDatabaseTableDateFields");
        }

        //public Task<List<fnORCHAGetFeeDefinitions>> GetFeeDefs(string caseTypeGroup, string caseType)
        //{
        //    return httpClient.GetJsonAsync<List<fnORCHAGetFeeDefinitions>>($"{userSession.baseUri}api/ChapterManagement/GetFeeDefs/{caseTypeGroup}/{caseType}");
        //}

        public Task<List<DmDocuments>> GetDocumentList(string caseType)
        {
            Lock = true;
            

            var returnValue = httpClient.GetJsonAsync<List<DmDocuments>>($"{userSession.baseUri}api/ChapterManagement/GetDocumentList/{caseType}");
            Lock = false;

            return returnValue;
        }

        public Task<List<DmDocuments>> GetDocumentListByCaseTypeGroup(int caseTypeGroupRef)
        {
            return httpClient.GetJsonAsync<List<DmDocuments>>($"{userSession.baseUri}api/ChapterManagement/GetDocumentListByCaseTypeGroupRef/{caseTypeGroupRef}");
        }

        public Task<List<string>> GetCaseTypeGroup()
        {
            return httpClient.GetJsonAsync<List<string>>($"{userSession.baseUri}api/ChapterManagement/GetCaseTypeGroup");
        }

        public Task<List<string>> GetCaseTypes()
        {
            var result = httpClient.GetJsonAsync<List<string>>($"{userSession.baseUri}api/ChapterManagement/GetCaseTypes");

            if (result.Exception is null)
            {
                return result;
            }
            else
            {
                return null;
            }
        }


        public Task<List<UsrOrsfSmartflows>> GetChapterListByCaseType(string caseType)
        {
            return httpClient.GetJsonAsync<List<UsrOrsfSmartflows>>($"{userSession.baseUri}api/ChapterManagement/GetChapterListByCaseType/{caseType}");
        }

        public Task<bool> CreateStep(VmChapterP4WStepSchemaJSONObject stepSchemaJSONObject)
        {
            Lock = true;
            var returnValue = httpClient.PutJsonAsync<bool>($"{userSession.baseUri}api/ChapterManagement/CreateStep", stepSchemaJSONObject);
            Lock = false;

            return returnValue;
        }

    }
}
