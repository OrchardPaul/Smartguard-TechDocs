using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gizmo_V1._00.Models

namespace Gizmo_V1._00.Data
{
    public class OR_RESI_DATA
    {

        private ApplicationDbContext _Context;

        public CustomersController()
        {
            _Context = new ApplicationDbContext();
            _P4WContext = new P4W_LIVE();
        }


        public Task<List<UsrOrResiMt>> GetDataRows()
        {
            return
        }
    }
}
