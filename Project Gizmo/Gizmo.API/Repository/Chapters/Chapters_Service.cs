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
        Task<UsrOrsfSmartflows> Add(UsrOrsfSmartflows item);
        Task<bool> CreateStep(string JSON);
        Task<UsrOrsfSmartflows> Delete(int id);
        Task<UsrOrsfSmartflows> DeleteChapter(int id);
        Task<List<UsrOrsfSmartflows>> GetAllChapters();
        Task<int?> GetCaseTypeCode(string caseType, int? caseTypeGroup);
        Task<List<string>> GetCaseTypeGroup();
        Task<int?> GetCaseTypeGroupRef();
        Task<List<string>> GetCaseTypes();
        Task<UsrOrsfSmartflows> GetChapterItemById(int id);
        Task<List<UsrOrsfSmartflows>> GetChapterListByCaseType(string caseType);
        Task<List<DmDocuments>> GetDocumentList(string caseType);
        Task<List<DmDocuments>> GetDocumentListByCaseTypeGroupRef(int caseTypeGroupRef);
        Task<UsrOrsfSmartflows> Update(UsrOrsfSmartflows item);
        Task<List<UsrOrsfSmartflows>> UpdateCaseType(string newCaseType, string originalCaseType, string caseTypeGroup);
        Task<List<UsrOrsfSmartflows>> UpdateCaseTypeGroups(string newCaseTypeGroup, string originalCaseTypeGroup);
    }

    public class Chapters_Service : IChapters_Service
    {
        private readonly P4W_Context _context;

        public Chapters_Service(P4W_Context context)
        {
            _context = context;
        }

        //public async Task<List<fnORCHAGetFeeDefinitions>> GetFeeDefs(string caseTypeGroup, string caseType)
        //{
        //    return await _context.fnORCHAGetFeeDefinitions(caseTypeGroup, caseType).ToListAsync();
        //}



        public async Task<List<UsrOrsfSmartflows>> GetAllChapters()
        {


            return await _context.UsrOrsfSmartflows.ToListAsync();
        }

        public async Task<UsrOrsfSmartflows> GetChapterItemById(int id)
        {
            return await _context.UsrOrsfSmartflows.SingleOrDefaultAsync(C => C.Id == id);
        }

        public async Task<UsrOrsfSmartflows> Add(UsrOrsfSmartflows item)
        {
            _context.UsrOrsfSmartflows.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<UsrOrsfSmartflows> Update(UsrOrsfSmartflows item)
        {
            var updatedItem = await _context.UsrOrsfSmartflows.SingleOrDefaultAsync(U => U.Id == item.Id);

            updatedItem.SmartflowName = item.SmartflowName;
            updatedItem.SeqNo = item.SeqNo;
            updatedItem.VariantName = item.VariantName;
            updatedItem.VariantNo = item.VariantNo;
            updatedItem.CaseType = item.CaseType;
            updatedItem.CaseTypeGroup = item.CaseTypeGroup;
            updatedItem.SmartflowData = item.SmartflowData;

            await _context.SaveChangesAsync();

            return item;
        }

        public async Task<List<UsrOrsfSmartflows>> UpdateCaseType(string newCaseType, string originalCaseType, string caseTypeGroup)
        {
            var updatedItems = await _context.UsrOrsfSmartflows
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


        public async Task<List<UsrOrsfSmartflows>> UpdateCaseTypeGroups(string newCaseTypeGroup, string originalCaseTypeGroup)
        {
            var updatedItems = await _context.UsrOrsfSmartflows.Where(C => C.CaseTypeGroup == originalCaseTypeGroup).ToListAsync();

            if (updatedItems.Count() > 0)
            {
                updatedItems = updatedItems.Select(C => { C.CaseTypeGroup = newCaseTypeGroup; return C; }).ToList();
                _context.UpdateRange(updatedItems);
                await _context.SaveChangesAsync();
            }

            return updatedItems;
        }

        public async Task<UsrOrsfSmartflows> DeleteChapter(int id)
        {
            var toDo = await _context.UsrOrsfSmartflows.FindAsync(id);

            _context.UsrOrsfSmartflows.Remove(toDo);
            await _context.SaveChangesAsync();
            return toDo;
        }

        public async Task<UsrOrsfSmartflows> Delete(int id)
        {
            var toDo = await _context.UsrOrsfSmartflows.FindAsync(id);
            _context.UsrOrsfSmartflows.Remove(toDo);
            await _context.SaveChangesAsync();
            return toDo;
        }

        public async Task<List<string>> GetCaseTypeGroup()
        {
            return await _context.UsrOrsfSmartflows
                .Select(s => s.CaseTypeGroup)
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<string>> GetCaseTypes()
        {
            return await _context.UsrOrsfSmartflows
                .Select(s => s.CaseType)
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<UsrOrsfSmartflows>> GetChapterListByCaseType(string caseType)
        {
            List<UsrOrsfSmartflows> Test = await _context.UsrOrsfSmartflows
                .Where(C => C.CaseType == caseType)
                .OrderBy(C => C.SeqNo)
                .ToListAsync();

            return Test;
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

            await _context.Database.ExecuteSqlRawAsync("EXEC up_ORSF_CreateSmartflowStep {0}", JSON);
            return true;

        }

    }
}
