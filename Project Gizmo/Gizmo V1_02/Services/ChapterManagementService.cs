using Gizmo.Context.OR_RESI;
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

        public ChapterManagementService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public Task<UsrOrDefChapterManagement> Add(UsrOrDefChapterManagement item)
        {
            return httpClient.PostJsonAsync<UsrOrDefChapterManagement>("https://localhost:44399/api/ChapterManagement/Add", item);

        }

        public Task<UsrOrDefChapterManagement> Update(UsrOrDefChapterManagement item)
        {
            return httpClient.PutJsonAsync<UsrOrDefChapterManagement>($"https://localhost:44399/api/ChapterManagement/Update/{item.Id}", item);
        }

        public Task Delete(int id)
        {
            return httpClient.DeleteAsync($"https://localhost:44399/api/ChapterManagement/Delete/{id}");
        }

        public Task<List<UsrOrDefChapterManagement>> GetItemListByChapter(int chapterId)
        {
            return httpClient.GetJsonAsync<List<UsrOrDefChapterManagement>>($"https://localhost:44399/api/ChapterManagement/GetItemListByChapter/{chapterId}");
        }

        public Task<List<UsrOrDefChapterManagement>> GetDocListByChapter(string caseType, string chapter)
        {
            return httpClient.GetJsonAsync<List<UsrOrDefChapterManagement>>($"https://localhost:44399/api/ChapterManagement/GetDocListByChapter/{caseType}/{chapter}");
        }

        public Task<List<DmDocuments>> GetDocumentList(string caseType)
        {
            return httpClient.GetJsonAsync<List<DmDocuments>>($"https://localhost:44399/api/ChapterManagement/GetDocumentList/{caseType}");
        }

        public Task<List<string>> GetCaseTypeGroup()
        {
            return httpClient.GetJsonAsync<List<string>>("https://localhost:44399/api/ChapterManagement/GetCaseTypeGroup");
        }

        public Task<List<string>> GetCaseTypes()
        {
            return httpClient.GetJsonAsync<List<string>>("https://localhost:44399/api/ChapterManagement/GetCaseTypes");
        }

        public Task<List<UsrOrDefChapterManagement>> GetChapterListByCaseType(string caseType)
        {
            return httpClient.GetJsonAsync<List<UsrOrDefChapterManagement>>($"https://localhost:44399/api/ChapterManagement/GetChapterListByCaseType/{caseType}");
        }

        public Task<List<UsrOrDefChapterManagement>> GetDocListByChapterAndDocType(string caseType, string chapter, string docType)
        {
            return httpClient.GetJsonAsync<List<UsrOrDefChapterManagement>>($"https://localhost:44399/api/ChapterManagement/GetDocListByChapterAndDocType/{caseType}/{chapter}/{docType}");
        }
    }
}
