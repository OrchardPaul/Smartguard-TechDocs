using GadjIT_ClientAPI.Repository.Partner;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GadjIT_ClientAPI.Controllers
{
    /*
     * Controller to access P4W default tables
     */

    

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PartnerAccessController : ControllerBase
    {
        private readonly IPartner_Access_Service partner_Access_Service;

        public PartnerAccessController(IPartner_Access_Service partner_Access_Service)
        {
            this.partner_Access_Service = partner_Access_Service;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllCaseTypes()
        {
            try
            {
                return Ok(await partner_Access_Service.GetAllCaseTypes());
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetAllCaseTypeGroups()
        {
            try
            {
                return Ok(await partner_Access_Service.GetAllCaseTypeGroups());
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetAllP4WViews()
        {
            try
            {
                return Ok(await partner_Access_Service.GetAllP4WViews());
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
