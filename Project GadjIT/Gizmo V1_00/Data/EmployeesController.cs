using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gizmo.Context.OR_RESI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Gizmo_V1_00.Data
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly P4W_OR_RESI_V6_DEVContext _dbContext;

        public EmployeesController(P4W_OR_RESI_V6_DEVContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        [Route("Get")]
        public async Task<List<TblEmployee>> Get()
        {
            return await _dbContext.TblEmployee.ToListAsync();
        }



        [HttpPost]
        [Route("Create")]
        public async Task<bool> Create([FromBody] TblEmployee employee)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Add(employee);
                try
                {
                    await _dbContext.SaveChangesAsync();
                    return true;
                }
                catch (DbUpdateException)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }


        [HttpGet]
        [Route("Details/{id}")]
        public async Task<TblEmployee> Details(int id)
        {
            return await _dbContext.TblEmployee.FindAsync(id);
        }

        [HttpPut]
        [Route("Edit/{id}")]
        public async Task<bool> Edit(int id, [FromBody] TblEmployee employee)
        {
            if (id != employee.EmployeeId)
            {
                return false;
            }

            _dbContext.Entry(employee).State = EntityState.Modified;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task<bool> DeleteConfirmed(int id)
        {
            var employee = await _dbContext.TblEmployee.FindAsync(id);
            if (employee == null)
            {
                return false;
            }

            _dbContext.TblEmployee.Remove(employee);
            await _dbContext.SaveChangesAsync();
            return true;
        }

    }
}
