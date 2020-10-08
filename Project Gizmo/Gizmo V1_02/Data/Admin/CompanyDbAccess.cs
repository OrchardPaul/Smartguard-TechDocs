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
        Task<AppCompanyDetails> DeleteCompany(AppCompanyDetails company);
        Task<List<AppCompanyDetails>> GetCompanies();
        Task<string> GetCompanyBaseUri(int id);
        Task<AppCompanyDetails> GetCompanyById(int id);
        Task<AppCompanyDetails> SubmitChanges(AppCompanyDetails company);
    }

    public class CompanyDbAccess : ICompanyDbAccess
    {
        private readonly ApplicationDbContext context;

        public CompanyDbAccess(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<List<AppCompanyDetails>> GetCompanies()
        {
            return await context.AppCompanyDetails.ToListAsync();
        }

        public async Task<AppCompanyDetails> GetCompanyById(int id)
        {
            return await context.AppCompanyDetails.SingleAsync(C => C.Id == id);
        }

        public async Task<string> GetCompanyBaseUri(int id)
        {
            var selectedCompany = await context.AppCompanyDetails.SingleOrDefaultAsync(C => C.Id == id);

            return (selectedCompany is null) ? null : selectedCompany.BaseUri;
        }

        public async Task<AppCompanyDetails> SubmitChanges(AppCompanyDetails company)
        {
            var selectedCompany = await context.AppCompanyDetails.SingleOrDefaultAsync(C => C.Id == company.Id);

            if (selectedCompany is null)
            {
                context.AppCompanyDetails.Add(company);
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

        public async Task<AppCompanyDetails> DeleteCompany(AppCompanyDetails company)
        {
            var selectedCompany = await context.AppCompanyDetails.SingleOrDefaultAsync(C => C.Id == company.Id);

            context.AppCompanyDetails.Remove(selectedCompany);
            await context.SaveChangesAsync();

            return selectedCompany;
        }
    }
}
