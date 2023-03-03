using GadjIT_ClientContext.P4W;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GadjIT_ClientAPI.Repository.Chapters
{
    public interface IChapters_Service
    {
        Task<Client_SmartflowRecord> Add(Client_SmartflowRecord item);
        Task<bool> CreateStep(string JSON);
        Task<Client_SmartflowRecord> Delete(int id);
        Task<Client_SmartflowRecord> DeleteChapter(int id);
        Task<List<Client_SmartflowRecord>> GetAllSmartflows();
        Task<int?> GetCaseTypeCode(string caseType, int? caseTypeGroup);
        Task<List<string>> GetCaseTypeGroup();
        Task<int?> GetCaseTypeGroupRef();
        Task<List<string>> GetCaseTypes();
        Task<Client_SmartflowRecord> GetChapterItemById(int id);
        Task<List<Client_SmartflowRecord>> GetSmartflowListByCaseType(string caseType);
        Task<List<DmDocuments>> GetDocumentList(string caseType);
        Task<List<DmDocuments>> GetDocumentListByCaseTypeGroupRef(int caseTypeGroupRef);
        Task<Client_SmartflowRecord> Update(Client_SmartflowRecord item);
        Task<List<Client_SmartflowRecord>> UpdateCaseType(string newCaseType, string originalCaseType, string caseTypeGroup);
        Task<List<Client_SmartflowRecord>> UpdateCaseTypeGroups(string newCaseTypeGroup, string originalCaseTypeGroup);
        Task<List<TableDate>> GetDatabaseTableDateFields();
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

        public async Task<List<TableDate>> GetDatabaseTableDateFields()
        {
            return await _context.GetTableDates().ToListAsync();
        }


        public async Task<List<Client_SmartflowRecord>> GetAllSmartflows()
        {


            return await _context.Client_SmartflowRecord.ToListAsync();
        }

        public async Task<Client_SmartflowRecord> GetChapterItemById(int id)
        {
            return await _context.Client_SmartflowRecord.SingleOrDefaultAsync(C => C.Id == id);
        }

        public async Task<Client_SmartflowRecord> Add(Client_SmartflowRecord item)
        {
            _context.Client_SmartflowRecord.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<Client_SmartflowRecord> Update(Client_SmartflowRecord item)
        {
            var updatedItem = await _context.Client_SmartflowRecord.SingleOrDefaultAsync(U => U.Id == item.Id);

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

        public async Task<List<Client_SmartflowRecord>> UpdateCaseType(string newCaseType, string originalCaseType, string caseTypeGroup)
        {
            var updatedItems = await _context.Client_SmartflowRecord
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


        public async Task<List<Client_SmartflowRecord>> UpdateCaseTypeGroups(string newCaseTypeGroup, string originalCaseTypeGroup)
        {
            var updatedItems = await _context.Client_SmartflowRecord.Where(C => C.CaseTypeGroup == originalCaseTypeGroup).ToListAsync();

            if (updatedItems.Count() > 0)
            {
                updatedItems = updatedItems.Select(C => { C.CaseTypeGroup = newCaseTypeGroup; return C; }).ToList();
                _context.UpdateRange(updatedItems);
                await _context.SaveChangesAsync();
            }

            return updatedItems;
        }

        public async Task<Client_SmartflowRecord> DeleteChapter(int id)
        {
            var toDo = await _context.Client_SmartflowRecord.FindAsync(id);

            _context.Client_SmartflowRecord.Remove(toDo);
            await _context.SaveChangesAsync();
            return toDo;
        }

        public async Task<Client_SmartflowRecord> Delete(int id)
        {
            var toDo = await _context.Client_SmartflowRecord.FindAsync(id);
            _context.Client_SmartflowRecord.Remove(toDo);
            await _context.SaveChangesAsync();
            return toDo;
        }

        public async Task<List<string>> GetCaseTypeGroup()
        {
            return await _context.Client_SmartflowRecord
                .Select(s => s.CaseTypeGroup)
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<string>> GetCaseTypes()
        {
            return await _context.Client_SmartflowRecord
                .Select(s => s.CaseType)
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<Client_SmartflowRecord>> GetSmartflowListByCaseType(string caseType)
        {
            List<Client_SmartflowRecord> Test = await _context.Client_SmartflowRecord
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
            try
            {
                await _context.Database.ExecuteSqlRawAsync("EXEC up_ORSF_CreateSmartflowStep {0}", JSON);
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
            

        }

    }
}
