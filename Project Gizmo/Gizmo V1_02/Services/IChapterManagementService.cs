using Gizmo.Context.OR_RESI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Services
{
    public interface IChapterManagementService 
    {
        Task<UsrOrDefChapterManagement> Add(UsrOrDefChapterManagement item);
        Task<UsrOrDefChapterManagement> Update(UsrOrDefChapterManagement item);
        Task Delete(int id);
        Task<List<string>> GetCaseTypeGroup();
        Task<List<string>> GetCaseTypes();
        Task<List<DmDocuments>> GetDocumentList(string caseType);
        Task<List<UsrOrDefChapterManagement>> GetItemListByChapter(int chapterId);
        Task<List<UsrOrDefChapterManagement>> GetDocListByChapter(string caseType, string chapter);
        Task<List<UsrOrDefChapterManagement>> GetChapterListByCaseType(string caseType);
        Task<List<UsrOrDefChapterManagement>> GetDocListByChapterAndDocType(string caseType, string chapter, string docType);
    }
}
