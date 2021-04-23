using GadjIT.ClientContext.P4W;
using GadjIT.ClientContext.P4W.Custom;
using GadjIT.ClientContext.P4W.Functions;
using Gizmo_V1_02.Data.Admin;
using Gizmo_V1_02.Services.SessionState;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Services
{
    public interface IChapterManagementService
    {
        Task<UsrOrDefChapterManagement> Add(UsrOrDefChapterManagement item);
        Task<Task<HttpResponseMessage>> Delete(int id);
        Task<Task<HttpResponseMessage>> DeleteChapter(int id);
        Task<List<UsrOrDefChapterManagement>> GetAllChapters();
        Task<List<string>> GetCaseTypeGroup();
        Task<List<string>> GetCaseTypes();
        Task<List<UsrOrDefChapterManagement>> GetChapterListByCaseType(string caseType);
        Task<List<UsrOrDefChapterManagement>> GetDocListByChapter(string caseType, string chapter);
        Task<List<UsrOrDefChapterManagement>> GetDocListByChapterAndDocType(string caseType, string chapter, string docType);
        Task<List<DmDocuments>> GetDocumentList(string caseType);
        Task<List<UsrOrDefChapterManagement>> GetItemListByChapter(int chapterId);
        Task<List<UsrOrDefChapterManagement>> GetItemListByChapterName(string casetypegroup, string casetype, string chapterName);
        Task<UsrOrDefChapterManagement> Update(UsrOrDefChapterManagement item);
        Task<UsrOrDefChapterManagement> UpdateMainItem(UsrOrDefChapterManagement item);
        Task<List<UsrOrDefChapterManagement>> UpdateCaseType(string newCaseTypeName, string originalCaseTypeName, string caseTypeGroup);
        Task<List<UsrOrDefChapterManagement>> UpdateCaseTypeGroups(string newCaseTypeGroupName, string originalCaseTypeGroupName);
        Task<List<fnORCHAGetFeeDefinitions>> GetFeeDefs(string caseTypeGroup, string caseType);
        Task<List<VmChapterFee>> UpdateChapterFees(int ChapterId, List<VmChapterFee> vmChapterFees);

        Task<bool> CreateStep(VmChapterP4WStepSchemaJSONObject stepSchemaJSONObject);
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

        public async Task<UsrOrDefChapterManagement> Add(UsrOrDefChapterManagement item)
        {
            return await httpClient.PostJsonAsync<UsrOrDefChapterManagement>($"{userSession.baseUri}api/ChapterManagement/Add", item);
        }

        public async Task<UsrOrDefChapterManagement> Update(UsrOrDefChapterManagement item)
        {
            await companyDbAccess.SaveSmartFlowRecordData(item, userSession);

            return await httpClient.PutJsonAsync<UsrOrDefChapterManagement>($"{userSession.baseUri}api/ChapterManagement/Update/{item.Id}", item);
        }

        public async Task<UsrOrDefChapterManagement> UpdateMainItem(UsrOrDefChapterManagement item)
        {
            await companyDbAccess.SaveSmartFlowRecord(item, userSession);

            return await httpClient.PutJsonAsync<UsrOrDefChapterManagement>($"{userSession.baseUri}api/ChapterManagement/Update/{item.Id}", item);
        }

        public Task<List<VmChapterFee>> UpdateChapterFees(int ChapterId, List<VmChapterFee> vmChapterFees)
        {
            return httpClient.PutJsonAsync<List<VmChapterFee>>($"{userSession.baseUri}api/ChapterManagement/UpdateChapterFees/{ChapterId}",vmChapterFees);
        }


        public Task<List<UsrOrDefChapterManagement>> UpdateCaseType(string newCaseTypeName, string originalCaseTypeName, string caseTypeGroup)
        {
            var item = new UsrOrDefChapterManagement();
            return httpClient.PutJsonAsync<List<UsrOrDefChapterManagement>>($"{userSession.baseUri}api/ChapterManagement/UpdateCaseType/{newCaseTypeName}/{originalCaseTypeName}/{caseTypeGroup}", item);
        }


        public Task<List<UsrOrDefChapterManagement>> UpdateCaseTypeGroups(string newCaseTypeGroupName, string originalCaseTypeGroupName)
        {
            var item = new UsrOrDefChapterManagement();
            return httpClient.PutJsonAsync<List<UsrOrDefChapterManagement>>($"{userSession.baseUri}api/ChapterManagement/UpdateCaseTypeGroups/{newCaseTypeGroupName}/{originalCaseTypeGroupName}", item);
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

        public Task<List<UsrOrDefChapterManagement>> GetAllChapters()
        {
            return httpClient.GetJsonAsync<List<UsrOrDefChapterManagement>>($"{userSession.baseUri}api/ChapterManagement/GetAllChapters");
        }

        public Task<List<fnORCHAGetFeeDefinitions>> GetFeeDefs(string caseTypeGroup, string caseType)
        {
            return httpClient.GetJsonAsync<List<fnORCHAGetFeeDefinitions>>($"{userSession.baseUri}api/ChapterManagement/GetFeeDefs/{caseTypeGroup}/{caseType}");
        }

        public Task<List<UsrOrDefChapterManagement>> GetItemListByChapter(int chapterId)
        {
            return httpClient.GetJsonAsync<List<UsrOrDefChapterManagement>>($"{userSession.baseUri}api/ChapterManagement/GetItemListByChapter/{chapterId}");
        }

        public Task<List<UsrOrDefChapterManagement>> GetItemListByChapterName(string casetypegroup, string casetype, string chapterName)
        {
            return httpClient.GetJsonAsync<List<UsrOrDefChapterManagement>>($"{userSession.baseUri}api/ChapterManagement/GetItemListByChapterName/{casetypegroup}/{casetype}/{chapterName}");
        }

        public Task<List<UsrOrDefChapterManagement>> GetDocListByChapter(string caseType, string chapter)
        {
            return httpClient.GetJsonAsync<List<UsrOrDefChapterManagement>>($"{userSession.baseUri}api/ChapterManagement/GetDocListByChapter/{caseType}/{chapter}");
        }

        public Task<List<DmDocuments>> GetDocumentList(string caseType)
        {
            return httpClient.GetJsonAsync<List<DmDocuments>>($"{userSession.baseUri}api/ChapterManagement/GetDocumentList/{caseType}");
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


        public Task<List<UsrOrDefChapterManagement>> GetChapterListByCaseType(string caseType)
        {
            return httpClient.GetJsonAsync<List<UsrOrDefChapterManagement>>($"{userSession.baseUri}api/ChapterManagement/GetChapterListByCaseType/{caseType}");
        }

        public Task<List<UsrOrDefChapterManagement>> GetDocListByChapterAndDocType(string caseType, string chapter, string docType)
        {
            return httpClient.GetJsonAsync<List<UsrOrDefChapterManagement>>($"{userSession.baseUri}api/ChapterManagement/GetDocListByChapterAndDocType/{caseType}/{chapter}/{docType}");
        }


        public Task<bool> CreateStep(VmChapterP4WStepSchemaJSONObject stepSchemaJSONObject)
        {
            var test = $"{userSession.baseUri}api/ChapterManagement/CreateStep";

            return httpClient.PutJsonAsync<bool>($"{userSession.baseUri}api/ChapterManagement/CreateStep", stepSchemaJSONObject);
        }

    }
}
