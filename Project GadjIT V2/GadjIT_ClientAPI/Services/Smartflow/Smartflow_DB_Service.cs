
using GadjIT_ClientContext.Data;
using GadjIT_ClientContext.Models.P4W;
using GadjIT_ClientContext.Models.Smartflow.Client;
using Microsoft.EntityFrameworkCore;

namespace GadjIT_ClientAPI.Services.Smartflow
{
    public interface ISmartflow_DB_Service
    {
        Task<Client_SmartflowRecord> Add(Client_SmartflowRecord item);
        Task<bool> CreateStep(string JSON);
        Task<Client_SmartflowRecord> Delete(int id);
        Task<Client_SmartflowRecord> DeleteSmartflow(int id);
        Task<List<Client_SmartflowRecord>> GetAllSmartflows();
        Task<int?> GetCaseTypeCode(string caseType, int? caseTypeGroup);
        Task<List<string>> GetCaseTypeGroup();
        Task<int?> GetCaseTypeGroupRef();
        Task<List<string>> GetCaseTypes();
        Task<Client_SmartflowRecord> GetSmartflowItemById(int id);
        Task<List<Client_SmartflowRecord>> GetSmartflowListByCaseType(string caseType);
        Task<List<P4W_DmDocuments>> GetDocumentList(string caseType);
        Task<List<P4W_DmDocuments>> GetDocumentListByCaseTypeGroupRef(int caseTypeGroupRef);
        Task<Client_SmartflowRecord> Update(Client_SmartflowRecord item);
        Task<List<Client_SmartflowRecord>> UpdateCaseType(string newCaseType, string originalCaseType, string caseTypeGroup);
        Task<List<Client_SmartflowRecord>> UpdateCaseTypeGroups(string newCaseTypeGroup, string originalCaseTypeGroup);
        Task<List<P4W_TableDate>> GetDatabaseTableDateFields();
    }

    public class Smartflow_DB_Service : ISmartflow_DB_Service
    {
        private readonly P4W_Context DBContext;
        

        public Smartflow_DB_Service( P4W_Context _context)
        {
            DBContext = _context;
        }


        public async Task<List<P4W_TableDate>> GetDatabaseTableDateFields()
        {
            return await DBContext.GetTableDates().ToListAsync();
            
        }


        public async Task<List<Client_SmartflowRecord>> GetAllSmartflows()
        {
            return await DBContext.Client_SmartflowRecord.ToListAsync();
            
        }

        public async Task<Client_SmartflowRecord> GetSmartflowItemById(int id)
        {
            return await DBContext.Client_SmartflowRecord.SingleOrDefaultAsync(C => C.Id == id);
            
        }

        public async Task<Client_SmartflowRecord> Add(Client_SmartflowRecord item)
        {
            DBContext.Client_SmartflowRecord.Add(item);
            await DBContext.SaveChangesAsync();
            return item;
            
        }

        public async Task<Client_SmartflowRecord> Update(Client_SmartflowRecord item)
        {
            var updatedItem = await DBContext.Client_SmartflowRecord.SingleOrDefaultAsync(U => U.Id == item.Id);

            updatedItem.SmartflowName = item.SmartflowName;
            updatedItem.SeqNo = item.SeqNo;
            updatedItem.VariantName = item.VariantName;
            updatedItem.VariantNo = item.VariantNo;
            updatedItem.CaseType = item.CaseType;
            updatedItem.CaseTypeGroup = item.CaseTypeGroup;
            updatedItem.SmartflowData = item.SmartflowData;

            await DBContext.SaveChangesAsync();

            return item;
        }
        

        public async Task<List<Client_SmartflowRecord>> UpdateCaseType(string newCaseType, string originalCaseType, string caseTypeGroup)
        {
            
            var updatedItems = await DBContext.Client_SmartflowRecord
                                                                    .Where(C => C.CaseTypeGroup == caseTypeGroup)
                                                                    .Where(C => C.CaseType == originalCaseType)
                                                                    .ToListAsync();

            if (updatedItems.Count() > 0)
            {
                updatedItems = updatedItems.Select(C => { C.CaseType = newCaseType; return C; }).ToList();
                DBContext.UpdateRange(updatedItems);
                await DBContext.SaveChangesAsync();
            }

            return updatedItems;
        }
        


        public async Task<List<Client_SmartflowRecord>> UpdateCaseTypeGroups(string newCaseTypeGroup, string originalCaseTypeGroup)
        {
            
            var updatedItems = await DBContext.Client_SmartflowRecord.Where(C => C.CaseTypeGroup == originalCaseTypeGroup).ToListAsync();

            if (updatedItems.Count() > 0)
            {
                updatedItems = updatedItems.Select(C => { C.CaseTypeGroup = newCaseTypeGroup; return C; }).ToList();
                DBContext.UpdateRange(updatedItems);
                await DBContext.SaveChangesAsync();
            }

            return updatedItems;
            
        }

        public async Task<Client_SmartflowRecord> DeleteSmartflow(int id)
        {
            
            var toDo = await DBContext.Client_SmartflowRecord.FindAsync(id);

            DBContext.Client_SmartflowRecord.Remove(toDo);
            await DBContext.SaveChangesAsync();
            return toDo;
            
        }

        public async Task<Client_SmartflowRecord> Delete(int id)
        {
            
            var toDo = await DBContext.Client_SmartflowRecord.FindAsync(id);
            DBContext.Client_SmartflowRecord.Remove(toDo);
            await DBContext.SaveChangesAsync();
            return toDo;
            
        }

        public async Task<List<string>> GetCaseTypeGroup()
        {
            
            return await DBContext.Client_SmartflowRecord
                .Select(s => s.CaseTypeGroup)
                .Distinct()
                .ToListAsync();
            
        }

        public async Task<List<string>> GetCaseTypes()
        {
            
            return await DBContext.Client_SmartflowRecord
                .Select(s => s.CaseType)
                .Distinct()
                .ToListAsync();
            
        }

        public async Task<List<Client_SmartflowRecord>> GetSmartflowListByCaseType(string caseType)
        {
            
            List<Client_SmartflowRecord> Test = await DBContext.Client_SmartflowRecord
                .Where(C => C.CaseType == caseType)
                .OrderBy(C => C.SeqNo)
                .ToListAsync();

            return Test;
            
        }

        public async Task<int?> GetCaseTypeGroupRef()
        {
            
            int? caseTypeGroupRef = await DBContext.DmDocuments
                .Where(D => D.Name == "FS-PRX-Pre-Exchange (P)")
                .Select(D => D.CaseTypeGroupRef)
                .SingleOrDefaultAsync();

            if (caseTypeGroupRef is null)
                caseTypeGroupRef = -1;

            return caseTypeGroupRef;
            
        }

        public async Task<int?> GetCaseTypeCode(string caseType, int? caseTypeGroup)
        {
            
            int? caseTypeCode = await DBContext.CaseTypes
                .Where(C => C.CaseTypeGroupRef == caseTypeGroup)
                .Where(C => EF.Functions.Like(C.Description, "%" + caseType + "%"))
                .Select(C => C.Code)
                .SingleOrDefaultAsync();

            if (caseTypeCode is null)
                caseTypeCode = -1;

            return caseTypeCode;
            
        }

        public async Task<List<P4W_DmDocuments>> GetDocumentList(string caseType)
        {
            
            int? caseTypeGroupRef = await GetCaseTypeGroupRef();

            int? caseTypeCode = await GetCaseTypeCode(caseType, caseTypeGroupRef);

            return await DBContext.DmDocuments
                            .ToListAsync();
            

        }

        public async Task<List<P4W_DmDocuments>> GetDocumentListByCaseTypeGroupRef(int caseTypeGroupRef)
        {
            
            return await DBContext.DmDocuments
                        .Where(x => x.CaseTypeGroupRef == caseTypeGroupRef)
                        .ToListAsync();
            
        }



        public async Task<bool> CreateStep(string JSON)
        {
            
            try
            {
                await DBContext.Database.ExecuteSqlRawAsync("EXEC up_ORSF_CreateSmartflowStep {0}", JSON);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            
        }

    }
}
