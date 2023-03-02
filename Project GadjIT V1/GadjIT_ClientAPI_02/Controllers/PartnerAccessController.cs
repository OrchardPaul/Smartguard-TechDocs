
using GadjIT_ClientAPI.Services.Partner;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GadjIT_ClientAPI.Controllers;

    /*
     * Controller to access P4W default tables
     */



[Route("api/[controller]/[action]")]
[ApiController]
public class PartnerAccessController : ControllerBase
{
    private readonly IPartner_DB_Service PartnerService;


    public PartnerAccessController(IPartner_DB_Service PartnerService)
    {
        this.PartnerService = PartnerService;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllCaseTypes()
    {
        return Ok(await PartnerService.GetAllCaseTypes());
    }

    [HttpGet]
    public async Task<ActionResult> GetAllCaseTypeGroups()
    {
        return Ok(await PartnerService.GetAllCaseTypeGroups());
        
    }

    [HttpGet]
    public async Task<ActionResult> GetAllP4WViews()
    {
        return Ok(await PartnerService.GetAllP4WViews());
       
    }
}
