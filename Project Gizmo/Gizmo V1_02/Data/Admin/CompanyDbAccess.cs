using Gizmo.Context.Gizmo_Authentification;
using Gizmo_V1_02.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Data.Admin
{
    public interface ICompanyDbAccess
    {
        Task<CompanyDetails> DeleteCompany(CompanyDetails company);
        Task<List<CompanyDetails>> GetCompanies();
        Task<string> GetCompanyBaseUri(int id);
        Task<CompanyDetails> GetCompanyById(int id);
        Task<CompanyDetails> SubmitChanges(CompanyDetails company);
    }

    public class CompanyDbAccess : ICompanyDbAccess
    {
        private readonly ApplicationDbContext context;

        public CompanyDbAccess(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<List<CompanyDetails>> GetCompanies()
        {
            return await context.CompanyDetails.ToListAsync();
        }

        public async Task<CompanyDetails> GetCompanyById(int id)
        {
            return await context.CompanyDetails.SingleAsync(C => C.Id == id);
        }

        public async Task<string> GetCompanyBaseUri(int id)
        {
            var selectedCompany = await context.CompanyDetails.SingleAsync(C => C.Id == id);

            return (selectedCompany is null) ? null : selectedCompany.BaseUri;
        }

        public async Task<CompanyDetails> SubmitChanges(CompanyDetails company)
        {
            var selectedCompany = await context.CompanyDetails.SingleOrDefaultAsync(C => C.Id == company.Id);

            if (selectedCompany is null)
            {
                context.CompanyDetails.Add(company);
                await context.SaveChangesAsync();
                return company;
            }
            else
            {
                selectedCompany.CompanyName = company.CompanyName;
                selectedCompany.BaseUri = company.BaseUri;

                await context.SaveChangesAsync();
                return selectedCompany;
            }
        }

        public async Task<CompanyDetails> DeleteCompany(CompanyDetails company)
        {
            var selectedCompany = await context.CompanyDetails.SingleOrDefaultAsync(C => C.Id == company.Id);

            context.CompanyDetails.Remove(selectedCompany);
            await context.SaveChangesAsync();

            return selectedCompany;
        }
    }
}
