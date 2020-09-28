using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gizmo.Context.OR_RESI;

namespace Gizmo.Api.Repository.OR_RESI
{
    public class OR_RESI_Chapters_Service : IOR_RESI_Chapters_Service
    {
        private readonly P4W_OR_RESI_V6_DEVContext _context;

        public OR_RESI_Chapters_Service(P4W_OR_RESI_V6_DEVContext context)
        {
            _context = context;
        }

        public async Task<List<UsrOrDefChapterManagement>> GetAllChapters()
        {
            return await _context.UsrOrDefChapterManagement.ToListAsync();
        }

        public async Task<UsrOrDefChapterManagement> GetChapterItemById(int id)
        {
            return await _context.UsrOrDefChapterManagement.SingleAsync(C => C.Id == id);
        }

        public async Task<UsrOrDefChapterManagement> Add(UsrOrDefChapterManagement item)
        {
            _context.UsrOrDefChapterManagement.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<UsrOrDefChapterManagement> Update(UsrOrDefChapterManagement item)
        {
            var updatedItem = await _context.UsrOrDefChapterManagement.SingleAsync(U => U.Id == item.Id);

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

            await _context.SaveChangesAsync();
            //_context.Entry(item).State = EntityState.Modified;
            return item;
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
                .Select(s => s.CaseTypeGroup )
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
            List<string> Doctype = new List<string>() { "Doc", "Letter", "Form", "Step" };

            var idRecord = _context.UsrOrDefChapterManagement
                .Where(C => C.Name == chapter)
                .Where(C => C.CaseType == caseType)
                .Where(C => C.Type == "Chapter")
                .Single();

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

        public int GetParentId(String caseType, String chapter)
        {

            int idRecord = _context.UsrOrDefChapterManagement
                .Where(C => C.Name == chapter)
                .Where(C => C.CaseType == caseType)
                .Where(C => C.Type == "Chapter")
                .Select(C => C.Id)
                .Single();

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

        public int? GetCaseTypeGroupRef()
        {
            int? caseTypeGroupRef = _context.DmDocuments
                .Where(D => D.Name == "FS-PRX-Pre-Exchange (P)")
                .Select(D => D.CaseTypeGroupRef)
                .Single();

            if (caseTypeGroupRef is null)
                caseTypeGroupRef = -1;

            return caseTypeGroupRef;
        }

        public int? GetCaseTypeCode(String caseType, int? caseTypeGroup)
        {
            int? caseTypeCode = _context.CaseTypes
                .Where(C => C.CaseTypeGroupRef == caseTypeGroup)
                .Where(C => EF.Functions.Like(C.Description, "%" + caseType + "%"))
                .Select(C => C.Code)
                .Single();

            if (caseTypeCode is null)
                caseTypeCode = -1;

            return caseTypeCode;
        }

        public async Task<List<DmDocuments>> GetDocumentList(String caseType)
        {
            int? caseTypeGroupRef = GetCaseTypeGroupRef();

            int? caseTypeCode = GetCaseTypeCode(caseType, caseTypeGroupRef);

            /*
            return (from d in _context.DmDocuments
                    join dm in _context.DmDocumentsPermissions on d.Code equals dm.Doccode
                    where dm.Casetype == caseTypeCode
                    orderby d.Name
                    select d.Name)
                    .ToList();
            */

            return await _context.DmDocuments
                            .Join(_context.DmDocumentsPermissions,
                                D => D.Code,
                                Dm => Dm.Doccode,
                                (D, Dm) => new { Name = D, DmDocumentsPermissions = Dm })
                            .Select(x => x.Name)
                            .ToListAsync(); 

        }



    }
}
