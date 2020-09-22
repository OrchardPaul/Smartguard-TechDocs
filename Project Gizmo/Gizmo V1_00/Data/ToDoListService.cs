using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gizmo.Context.OR_RESI;
using Microsoft.EntityFrameworkCore;


namespace Gizmo_V1_00.Data
{
    public interface IToDoListService
    {
        Task<List<TblToDo>> Get();
        Task<TblToDo> Get(int id);
        Task<TblToDo> Add(TblToDo toDo);
        Task<TblToDo> Update(TblToDo toDo);
        Task<TblToDo> Delete(int id);
    }
    public class ToDoListService : IToDoListService
    {
        private readonly P4W_OR_RESI_V5_DEVContext _context;

        public ToDoListService(P4W_OR_RESI_V5_DEVContext context)
        {
            _context = context;
        }
        public async Task<List<TblToDo>> Get()
        {
            return await _context.TblToDo.ToListAsync();
        }

        public async Task<TblToDo> Get(int id)
        {
            var toDo = await _context.TblToDo.FindAsync(id);
            return toDo;
        }

        public async Task<TblToDo> Add(TblToDo toDo)
        {
            _context.TblToDo.Add(toDo);
            await _context.SaveChangesAsync();
            return toDo;
        }

        public async Task<TblToDo> Update(TblToDo toDo)
        {
            _context.Entry(toDo).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return toDo;
        }

        public async Task<TblToDo> Delete(int id)
        {
            var toDo = await _context.TblToDo.FindAsync(id);
            _context.TblToDo.Remove(toDo);
            await _context.SaveChangesAsync();
            return toDo;
        }
    }
}
