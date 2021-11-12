using Gizmo_V1_00.Session_Data;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Gizmo_V1_00.Services
{
    public class ChapterManagementService : IChapterManagementService
    {
        private readonly HttpClient httpClient;

        public ChapterManagementService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public Task<List<string>> GetCaseTypeGroup()
        {
            return httpClient.GetJsonAsync<List<string>>("https://localhost:44399/api/ChapterManagement/GetCaseTypeGroup");
        }

        public Task<List<string>> GetCaseTypes()
        {
            return httpClient.GetJsonAsync<List<string>>("https://localhost:44399/api/ChapterManagement/GetCaseTypes");
        }
    }
}
