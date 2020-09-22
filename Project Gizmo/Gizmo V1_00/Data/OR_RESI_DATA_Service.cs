using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gizmo.Context.OR_RESI;

namespace Gizmo_V1_00.Data
{
    public class OR_RESI_DATA_Service
    {

        private readonly P4W_OR_RESI_V5_DEVContext _Context;

        public OR_RESI_DATA_Service(P4W_OR_RESI_V5_DEVContext context)
        {
            _Context = context;
        }

        public async Task<UsrOrResiMt[]> GetDataRows()
        {
            var dataRows = _Context.UsrOrResiMt.ToArray();

            return await Task.FromResult(result: dataRows);
        }

    }
}
