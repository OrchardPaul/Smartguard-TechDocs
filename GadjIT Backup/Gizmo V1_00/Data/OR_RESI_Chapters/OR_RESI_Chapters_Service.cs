using Gizmo_V1_00.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo_V1_00.Data.OR_RESI_Chapters
{
    public interface IOR_RESI_Chapters_Service
    {
        Task<UsrOrDefChapterManagement> Add(UsrOrDefChapterManagement item);
        Task<UsrOrDefChapterManagement> Delete(int id);
        Task<List<UsrOrDefChapterManagement>> GetChapterListByCaseType(string Type, string caseType);
        Task<List<UsrOrDefChapterManagement>> GetDocListByChapter(string caseType, string chapter);
        Task<List<UsrOrDefChapterManagement>> GetDocListByChapterAndDocType(string caseType, string chapter, string docType);
        int GetParentId(String caseType, String chapter);
        int? GetMaxSeqNum(int parentId);
        Task<UsrOrDefChapterManagement> Update(UsrOrDefChapterManagement item);
    }

    public class OR_RESI_Chapters_Service : IOR_RESI_Chapters_Service
    {
        private readonly P4W_OR_RESI_V5_DEVContext _context;

        public OR_RESI_Chapters_Service(P4W_OR_RESI_V5_DEVContext context)
        {
            _context = context;
        }

        public async Task<List<UsrOrDefChapterManagement>> GetChapterListByCaseType(string Type, string caseType)
        {
            var Chapters = await _context.UsrOrDefChapterManagement
                .Where(C => C.Type == Type)
                .Where(C => C.CaseType == caseType)
                .OrderBy(C => C.SeqNo)
                .ToListAsync();
            return Chapters;
        }

        public async Task<List<UsrOrDefChapterManagement>> GetDocListByChapter(String caseType, String chapter)
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

            int idRecord =  _context.UsrOrDefChapterManagement
                .Where(C => C.Name == chapter)
                .Where(C => C.CaseType == caseType)
                .Where(C => C.Type == "Chapter")
                .Select(C=> C.Id)
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


        public async Task<UsrOrDefChapterManagement> Add(UsrOrDefChapterManagement item)
        {
            _context.UsrOrDefChapterManagement.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<UsrOrDefChapterManagement> Update(UsrOrDefChapterManagement item)
        {
            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<UsrOrDefChapterManagement> Delete(int id)
        {
            var toDo = await _context.UsrOrDefChapterManagement.FindAsync(id);
            _context.UsrOrDefChapterManagement.Remove(toDo);
            await _context.SaveChangesAsync();
            return toDo;
        }
    }
}
