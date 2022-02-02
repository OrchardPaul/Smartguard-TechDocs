using GadjIT.ClientAPI.Repository.GeneralAccess;
using GadjIT.ClientAPI.Repository.Partner;
using GadjIT.ClientContext.P4W;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GadjIT.ClientAPI.Controllers
{
    /*
     * Controller to access P4W default tables
     */

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GeneralAccessController : ControllerBase
    {
        private readonly IGeneralAccessService generalAccessService;

        public GeneralAccessController(IGeneralAccessService generalAccessService)
        {
            this.generalAccessService = generalAccessService;
        }


        /// <summary>
        /// Run query received and return results
        /// </summary>
        /// <param name="sql">Query</param>
        /// <returns>Dictionary of key/value</returns>
        [HttpPut]
        public async Task<ActionResult> GetListOfData(SQLRequest sql)
        {
            try
            {
                return Ok(await generalAccessService.LoadData<dynamic,dynamic>(sql.Query));
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }
        }


    }
}
