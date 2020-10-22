using Gizmo.Context.OR_RESI;
using Gizmo_V1_02.Services.SessionState;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Services
{
    public class ChapterManagementService : IChapterManagementService
    {
        private readonly HttpClient httpClient;
        private readonly IUserSessionState userSession;

        public ChapterManagementService(HttpClient httpClient,IUserSessionState userSession)
        {
            this.httpClient = httpClient;
            this.userSession = userSession;
        }

        public Task<UsrOrDefChapterManagement> Add(UsrOrDefChapterManagement item)
        {
            return httpClient.PostJsonAsync<UsrOrDefChapterManagement>($"{userSession.baseUri}api/ChapterManagement/Add", item);

        }

        public Task<UsrOrDefChapterManagement> Update(UsrOrDefChapterManagement item)
        {
            return httpClient.PutJsonAsync<UsrOrDefChapterManagement>($"{userSession.baseUri}api/ChapterManagement/Update/{item.Id}", item);
        }

        public Task Delete(int id)
        {
            return httpClient.DeleteAsync($"{userSession.baseUri}api/ChapterManagement/Delete/{id}");
        }

        public Task<List<UsrOrDefChapterManagement>> GetItemListByChapter(int chapterId)
        {
            return httpClient.GetJsonAsync<List<UsrOrDefChapterManagement>>($"{userSession.baseUri}api/ChapterManagement/GetItemListByChapter/{chapterId}");
        }

        public Task<List<UsrOrDefChapterManagement>> GetDocListByChapter(string caseType, string chapter)
        {
            return httpClient.GetJsonAsync<List<UsrOrDefChapterManagement>>($"{userSession.baseUri}api/ChapterManagement/GetDocListByChapter/{caseType}/{chapter}");
        }

        public Task<List<DmDocuments>> GetDocumentList(string caseType)
        {
            return httpClient.GetJsonAsync<List<DmDocuments>>($"{userSession.baseUri}api/ChapterManagement/GetDocumentList/{caseType}");
        }

        public Task<List<string>> GetCaseTypeGroup()
        {
            return httpClient.GetJsonAsync<List<string>>($"{userSession.baseUri}api/ChapterManagement/GetCaseTypeGroup");
        }

        public Task<List<string>> GetCaseTypes()
        {
            var result = httpClient.GetJsonAsync<List<string>>($"{userSession.baseUri}api/ChapterManagement/GetCaseTypes");

            if(result.Exception is null)
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
    }
}
