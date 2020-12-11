using Gizmo.Context.OR_RESI;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gizmo.Api.Repository.OR_RESI
{
    public interface IOR_RESI_Chapters_Service
    {
        Task<List<UsrOrDefChapterManagement>> GetAllChapters();
        Task<UsrOrDefChapterManagement> GetChapterItemById(int id);
        Task<UsrOrDefChapterManagement> Add(UsrOrDefChapterManagement item);
        Task<List<UsrOrDefChapterManagement>> UpdateCaseType(string newCaseType, string originalCaseType, string caseTypeGroup);
        Task<List<UsrOrDefChapterManagement>> UpdateCaseTypeGroups(string newCaseTypeGroup, string originalCaseTypeGroup);
        Task<UsrOrDefChapterManagement> Update(UsrOrDefChapterManagement item);
        Task<UsrOrDefChapterManagement> DeleteChapter(int id);
        Task<UsrOrDefChapterManagement> Delete(int id);
        Task<List<string>> GetCaseTypeGroup();
        Task<List<string>> GetCaseTypes();
        Task<List<UsrOrDefChapterManagement>> GetChapterListByCaseType(string caseType);
        Task<List<UsrOrDefChapterManagement>> GetDocListByChapter(string caseType, string chapter);
        Task<List<UsrOrDefChapterManagement>> GetItemListByChapter(int chapterId);
        Task<List<UsrOrDefChapterManagement>> GetDocListByChapterAndDocType(string caseType, string chapter, string docType);
        Task<List<DmDocuments>> GetDocumentList(string caseType);
        int? GetCaseTypeCode(string caseType, int? caseTypeGroup);
        int? GetCaseTypeGroupRef();
        int? GetMaxSeqNum(int parentId);
        int GetParentId(string caseType, string chapter);

    }
}