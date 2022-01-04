using GadjIT.ClientContext.P4W;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GadjIT.ClientAPI.Repository.Partner
{
    public interface IPartner_Access_Service
    {
        Task<List<CaseTypes>> GetAllCaseTypes();
        Task<List<CaseTypeGroups>> GetAllCaseTypeGroups();
        Task<List<MpSysViews>> GetAllP4WViews();
    }

    public class Partner_Access_Service : IPartner_Access_Service
    {
        private readonly P4W_Context context;

        public Partner_Access_Service(P4W_Context context)
        {
            this.context = context;
        }

        public async Task<List<CaseTypes>> GetAllCaseTypes()
        {
            return await context.CaseTypes.ToListAsync();
        }

        public async Task<List<CaseTypeGroups>> GetAllCaseTypeGroups()
        {
            return await context.CaseTypeGroups.ToListAsync();
        }

        public async Task<List<MpSysViews>> GetAllP4WViews()
        {
            return await context.MpSysViews.ToListAsync();
        }

    }
}
