using Gizmo.Context.OR_RESI;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gizmo.Api.Repository.OR_RESI
{
    public interface IPartner_Access_Service
    {
        Task<List<CaseTypes>> GetAllCaseTypes();
    }

    public class Partner_Access_Service : IPartner_Access_Service
    {
        private readonly P4W_OR_RESI_V6_DEVContext context;

        public Partner_Access_Service(P4W_OR_RESI_V6_DEVContext context)
        {
            this.context = context;
        }

        public async Task<List<CaseTypes>> GetAllCaseTypes()
        {
            return await context.CaseTypes.ToListAsync();
        }

    }
}
