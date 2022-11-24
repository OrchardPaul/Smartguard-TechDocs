


using GadjIT_ClientContext.P4W;
using Microsoft.EntityFrameworkCore;

namespace GadjIT_ClientAPI.Services.Partner
{
    public interface IPartner_DB_Service
    {
        Task<List<CaseTypes>> GetAllCaseTypes();
        Task<List<CaseTypeGroups>> GetAllCaseTypeGroups();
        Task<List<MpSysViews>> GetAllP4WViews();
    }

    public class Partner_DB_Service : IPartner_DB_Service
    {
        private readonly P4W_Context context;

        public Partner_DB_Service(P4W_Context context)
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
