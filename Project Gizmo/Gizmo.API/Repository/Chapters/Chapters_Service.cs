using GadjIT.ClientContext.P4W;
using GadjIT.ClientContext.P4W.Custom;
using GadjIT.ClientContext.P4W.Functions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GadjIT.ClientAPI.Repository.Chapters
{
    public interface IChapters_Service
    {
        Task<UsrOrDefChapterManagement> Add(UsrOrDefChapterManagement item);
        Task<UsrOrDefChapterManagement> Delete(int id);
        Task<UsrOrDefChapterManagement> DeleteChapter(int id);
        Task<List<UsrOrDefChapterManagement>> GetAllChapters();
        Task<int?> GetCaseTypeCode(string caseType, int? caseTypeGroup);
        Task<List<string>> GetCaseTypeGroup();
        Task<int?> GetCaseTypeGroupRef();
        Task<List<string>> GetCaseTypes();
        Task<UsrOrDefChapterManagement> GetChapterItemById(int id);
        Task<List<UsrOrDefChapterManagement>> GetChapterListByCaseType(string caseType);
        Task<List<UsrOrDefChapterManagement>> GetDocListByChapter(string caseType, string chapter);
        Task<List<UsrOrDefChapterManagement>> GetDocListByChapterAndDocType(string caseType, string chapter, string docType);
        Task<List<DmDocuments>> GetDocumentList(string caseType);
        Task<List<DmDocuments>> GetDocumentListByCaseTypeGroupRef(int caseTypeGroupRef);
        Task<List<UsrOrDefChapterManagement>> GetItemListByChapter(int chapterId);
        Task<List<UsrOrDefChapterManagement>> GetItemListByChapterName(string casetypegroup, string casetype, string chapterName);
        int? GetMaxSeqNum(int parentId);
        Task<int> GetParentId(string caseType, string chapter);
        Task<UsrOrDefChapterManagement> Update(UsrOrDefChapterManagement item);
        Task<List<UsrOrDefChapterManagement>> UpdateCaseType(string newCaseType, string originalCaseType, string caseTypeGroup);
        Task<List<UsrOrDefChapterManagement>> UpdateCaseTypeGroups(string newCaseTypeGroup, string originalCaseTypeGroup);
        Task<List<VmChapterFee>> UpdateChapterFees(int ChapterId, List<VmChapterFee> vmChapterFees);
        Task<bool> CreateStep(string JSON);
    }

    public class Chapters_Service : IChapters_Service
    {
        private readonly P4W_Context _context;

        public Chapters_Service(P4W_Context context)
        {
            _context = context;
        }

        



        public async Task<List<UsrOrDefChapterManagement>> GetAllChapters()
        {
            

            return await _context.UsrOrDefChapterManagement.Where(C => C.ParentId == 0).ToListAsync();
        }

        public async Task<UsrOrDefChapterManagement> GetChapterItemById(int id)
        {
            return await _context.UsrOrDefChapterManagement.SingleOrDefaultAsync(C => C.Id == id);
        }

        public async Task<UsrOrDefChapterManagement> Add(UsrOrDefChapterManagement item)
        {
            _context.UsrOrDefChapterManagement.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<UsrOrDefChapterManagement> Update(UsrOrDefChapterManagement item)
        {
            var updatedItem = await _context.UsrOrDefChapterManagement.SingleOrDefaultAsync(U => U.Id == item.Id);

            updatedItem.Name = item.Name;
            updatedItem.RescheduleDays = item.RescheduleDays;
            updatedItem.AsName = item.AsName;
            updatedItem.Type = item.Type;
            updatedItem.SeqNo = item.SeqNo;
            updatedItem.CaseType = item.CaseType;
            updatedItem.CaseTypeGroup = item.CaseTypeGroup;
            updatedItem.CompleteName = item.CompleteName;
            updatedItem.EntityType = item.EntityType;
            updatedItem.SuppressStep = item.SuppressStep;
            updatedItem.AltDisplayName = item.AltDisplayName;
            updatedItem.NextStatus = item.NextStatus;
            updatedItem.ChapterData = item.ChapterData;

            await _context.SaveChangesAsync();

            return item;
        }

        public async Task<List<UsrOrDefChapterManagement>> UpdateCaseType(string newCaseType, string originalCaseType, string caseTypeGroup)
        {
            var updatedItems = await _context.UsrOrDefChapterManagement
                                                                    .Where(C => C.CaseTypeGroup == caseTypeGroup)
                                                                    .Where(C => C.CaseType == originalCaseType)
                                                                    .ToListAsync();

            if (updatedItems.Count() > 0)
            {
                updatedItems = updatedItems.Select(C => { C.CaseType = newCaseType; return C; }).ToList();
                _context.UpdateRange(updatedItems);
                await _context.SaveChangesAsync();
            }

            return updatedItems;
        }


        public async Task<List<UsrOrDefChapterManagement>> UpdateCaseTypeGroups(string newCaseTypeGroup, string originalCaseTypeGroup)
        {
            var updatedItems = await _context.UsrOrDefChapterManagement.Where(C => C.CaseTypeGroup == originalCaseTypeGroup).ToListAsync();

            if (updatedItems.Count() > 0)
            {
                updatedItems = updatedItems.Select(C => { C.CaseTypeGroup = newCaseTypeGroup; return C; }).ToList();
                _context.UpdateRange(updatedItems);
                await _context.SaveChangesAsync();
            }

            return updatedItems;
        }

        public async Task<List<VmChapterFee>> UpdateChapterFees(int ChapterId, List<VmChapterFee> vmChapterFees)
        {
            bool change = false;

            var existingItems = await _context.UsrOrDefChapterManagement
                                                .Where(C => C.ParentId == ChapterId)
                                                .Where(C => C.Type == "Fee")
                                                .ToListAsync();

            var itemsToAdd = vmChapterFees
                                .Where(C => C.selected)
                                .Where(C => !existingItems
                                                .Select(E => E.Name)
                                                .ToList()
                                                .Contains(C.FeeItem.FeeName))
                                .Select(C => C.FeeItem)
                                .ToList();

            var itemsToRemove = existingItems
                                    .Where(C => vmChapterFees
                                                    .Where(V => !V.selected)
                                                    .Select(V => V.FeeItem.FeeName)
                                                    .ToList()
                                                    .Contains(C.Name))
                                    .ToList();


            

            if (itemsToRemove.Count() > 0)
            {
                _context.UsrOrDefChapterManagement.RemoveRange(itemsToRemove);
                change = true;
            }

            if (change)
            {
                await _context.SaveChangesAsync();
            }

            return vmChapterFees;
        }

        public async Task<UsrOrDefChapterManagement> DeleteChapter(int id)
        {
            var toDo = await _context.UsrOrDefChapterManagement.FindAsync(id);

            var chapterItems = await _context.UsrOrDefChapterManagement.Where(C => C.ParentId == toDo.Id).ToListAsync();

            if (chapterItems.Count() > 0)
            {
                _context.RemoveRange(chapterItems);
            }

            _context.UsrOrDefChapterManagement.Remove(toDo);
            await _context.SaveChangesAsync();
            return toDo;
        }

        public async Task<UsrOrDefChapterManagement> Delete(int id)
        {
            var toDo = await _context.UsrOrDefChapterManagement.FindAsync(id);
            _context.UsrOrDefChapterManagement.Remove(toDo);
            await _context.SaveChangesAsync();
            return toDo;
        }

        public async Task<List<string>> GetCaseTypeGroup()
        {
            return await _context.UsrOrDefChapterManagement
                .Where(C => C.ParentId == 0)
                .Select(s => s.CaseTypeGroup)
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<string>> GetCaseTypes()
        {
            return await _context.UsrOrDefChapterManagement
                .Where(C => C.ParentId == 0)
                .Select(s => s.CaseType)
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<UsrOrDefChapterManagement>> GetChapterListByCaseType(string caseType)
        {
            List<UsrOrDefChapterManagement> Test = await _context.UsrOrDefChapterManagement
                .Where(C => C.Type == "Chapter")
                .Where(C => C.CaseType == caseType)
                .OrderBy(C => C.SeqNo)
                .ToListAsync();

            return Test;
        }

        public async Task<List<UsrOrDefChapterManagement>> GetDocListByChapter(string caseType, string chapter)
        {
            List<string> Doctype = new List<string>() { "Doc", "Form", "Step", "Date", "Email", "Letter" };

            var idRecord = _context.UsrOrDefChapterManagement
                .Where(C => C.Name == chapter)
                .Where(C => C.CaseType == caseType)
                .Where(C => C.Type == "Chapter")
                .SingleOrDefaultAsync();

            return await _context.UsrOrDefChapterManagement
                                .Where(C => C.ParentId == idRecord.Id)
                                .Where(C => Doctype.Contains(C.Type))
                                .ToListAsync();
        }


        public async Task<List<UsrOrDefChapterManagement>> GetItemListByChapter(int chapterId)
        {
            return await _context.UsrOrDefChapterManagement
                                .Where(C => C.ParentId == chapterId)
                                .ToListAsync();
        }

        public async Task<List<UsrOrDefChapterManagement>> GetItemListByChapterName(string casetypegroup, string casetype, string chapterName)
        {
            return await _context.UsrOrDefChapterManagement
                                .Where(C => C.CaseTypeGroup == casetypegroup)
                                .Where(C => C.CaseType == casetype)
                                .Where(C => C.Name == chapterName)
                                .ToListAsync();
        }

        public async Task<List<UsrOrDefChapterManagement>> GetDocListByChapterAndDocType(String caseType, String chapter, String docType)
        {

            var idRecord = _context.UsrOrDefChapterManagement
                .Where(C => C.Name == chapter)
                .Where(C => C.CaseType == caseType)
                .Where(C => C.Type == "Chapter")
                .Single();

            return await _context.UsrOrDefChapterManagement
                                .Where(C => C.ParentId == idRecord.Id)
                                .Where(C => C.Type == docType)
                                .ToListAsync();
        }

        public async Task<int> GetParentId(String caseType, String chapter)
        {

            int idRecord = await _context.UsrOrDefChapterManagement
                .Where(C => C.Name == chapter)
                .Where(C => C.CaseType == caseType)
                .Where(C => C.Type == "Chapter")
                .Select(C => C.Id)
                .SingleOrDefaultAsync();

            return idRecord;
        }

        public int? GetMaxSeqNum(int parentId)
        {
            int? seqRecord = _context.UsrOrDefChapterManagement
                .Where(C => C.ParentId == parentId)
                .Max(C => C.SeqNo);

            if (seqRecord is null)
                seqRecord = 0;
            else
                seqRecord++;

            return seqRecord;
        }

        public async Task<int?> GetCaseTypeGroupRef()
        {
            int? caseTypeGroupRef = await _context.DmDocuments
                .Where(D => D.Name == "FS-PRX-Pre-Exchange (P)")
                .Select(D => D.CaseTypeGroupRef)
                .SingleOrDefaultAsync();

            if (caseTypeGroupRef is null)
                caseTypeGroupRef = -1;

            return caseTypeGroupRef;
        }

        public async Task<int?> GetCaseTypeCode(string caseType, int? caseTypeGroup)
        {
            int? caseTypeCode = await _context.CaseTypes
                .Where(C => C.CaseTypeGroupRef == caseTypeGroup)
                .Where(C => EF.Functions.Like(C.Description, "%" + caseType + "%"))
                .Select(C => C.Code)
                .SingleOrDefaultAsync();

            if (caseTypeCode is null)
                caseTypeCode = -1;

            return caseTypeCode;
        }

        public async Task<List<DmDocuments>> GetDocumentList(string caseType)
        {
            int? caseTypeGroupRef = await GetCaseTypeGroupRef();

            int? caseTypeCode = await GetCaseTypeCode(caseType, caseTypeGroupRef);

            return await _context.DmDocuments
                            .ToListAsync();

        }

        public async Task<List<DmDocuments>> GetDocumentListByCaseTypeGroupRef(int caseTypeGroupRef)
        {
            return await _context.DmDocuments
                            .Where(x => x.CaseTypeGroupRef == caseTypeGroupRef)
                            .ToListAsync();

        }



        public async Task<bool> CreateStep(string JSON)
        {

                await _context.Database.ExecuteSqlRawAsync("EXEC up_ORSF_CreateSmartflowStep {0}",JSON);
                return true;

        }

    }
}
