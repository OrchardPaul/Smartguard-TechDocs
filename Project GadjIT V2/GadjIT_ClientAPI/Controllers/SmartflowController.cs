using Microsoft.AspNetCore.Mvc;
using GadjIT_ClientAPI.Services.Smartflow;
using GadjIT_ClientContext.Models.Smartflow.Client;
using GadjIT_ClientContext.Models.P4W;

namespace GadjIT_ClientAPI.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class SmartflowController : ControllerBase
{
    private readonly ISmartflow_DB_Service SmartflowService;

    private ILogger<SmartflowController> Logger {get; set;}

    public SmartflowController(ISmartflow_DB_Service _smartflowService, ILogger<SmartflowController> _logger)
    {
        SmartflowService = _smartflowService;
        Logger = _logger;
    }

    /// ###########################################
    /// TESTING
    /// ###########################################
    [HttpGet]
    public ActionResult TestApi()
    {
        //e.g http://live.orarizonadebt.co.uk:8393/api/smartflow/testapi
        //    https://livewilliamsons.gadjit.co.uk:8393/api/smartflow/testapi

        Logger.LogInformation("API Test: {0}",$"Application is working");
        return Ok("Test Complete: Application is working");

    }
    
    [HttpGet]
    public async Task<ActionResult> TestGet()
    {
        //http://live.orarizonadebt.co.uk:8393/api/smartflow/testget

        var caseTypeGroups = await SmartflowService.GetCaseTypeGroup();
            
        if(caseTypeGroups.Count > 0){
            Logger.LogInformation("API Test: {0}",$"Get is working, the database has {caseTypeGroups.Count.ToString()} case type groups");
            return Ok($"Get is working, the database has {caseTypeGroups.Count.ToString()} case type groups");
        }
        else{
            Logger.LogWarning("API Test: {0}",$"Get did not error. However, the database did not return any case type groups");
            return Ok($"Get did not error. However, the database did not return any case type groups");

        }
       
    }

    [HttpGet]
    public async Task<ActionResult> GetCaseTypeGroup()
    {
        return Ok(await SmartflowService.GetCaseTypeGroup());
        
    }

    [HttpGet]
    public async Task<ActionResult> GetDatabaseTableDateFields()
    {
        return Ok(await SmartflowService.GetDatabaseTableDateFields());
        
    }

    [HttpGet]
    public async Task<ActionResult> GetCaseTypes()
    {
        return Ok(await SmartflowService.GetCaseTypes());
        
    }


    [HttpGet]
    public async Task<ActionResult> GetAllSmartflows()
    {
        return Ok(await SmartflowService.GetAllSmartflows());
    }


    [HttpGet("{caseType}")]
    public async Task<ActionResult> GetSmartflowListByCaseType(string caseType)
    {
        return Ok(await SmartflowService.GetSmartflowListByCaseType(caseType));
        
    }

    [HttpGet("{caseType}")]
    public async Task<ActionResult> GetDocumentList(string caseType)
    {
        return Ok(await SmartflowService.GetDocumentList(caseType));
        
    }

    [HttpGet("{caseTypeGroupRef:int}")]
    public async Task<ActionResult> GetDocumentListByCaseTypeGroupRef(int caseTypeGroupRef)
    {
        return Ok(await SmartflowService.GetDocumentListByCaseTypeGroupRef(caseTypeGroupRef));
        
    }

    [HttpPost]
    public async Task<ActionResult> Add(Client_SmartflowRecord item)
    {

        
        if (item is null)
        {
            return BadRequest();
        }

        var newItem = await SmartflowService.Add(item);

        return CreatedAtAction(nameof(Add), new { id = newItem.Id });
        
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Client_SmartflowRecord>> Update(int Id, Client_SmartflowRecord item)
    {
        if (Id != item.Id)
        {
            return BadRequest("Id missmatch");
        }

        var selectedItem = await SmartflowService.GetSmartflowItemById(item.Id);

        if (selectedItem is null)
        {
            return NotFound($"Item with ID = {Id} not found");
        }

        return await SmartflowService.Update(item);
       
    }


    [HttpPut("{newCaseType}/{originalCaseType}/{caseTypeGroup}")]
    public async Task<ActionResult<List<Client_SmartflowRecord>>> UpdateCaseType(string newCaseType, string originalCaseType, string caseTypeGroup)
    {
        return await SmartflowService.UpdateCaseType(newCaseType, originalCaseType,caseTypeGroup);
        
    }

    [HttpPut("{newCaseTypeGroup}/{originalCaseTypeGroup}")]
    public async Task<ActionResult<List<Client_SmartflowRecord>>> UpdateCaseTypeGroups(string newCaseTypeGroup, string originalCaseTypeGroup)
    {
        return await SmartflowService.UpdateCaseTypeGroups(newCaseTypeGroup, originalCaseTypeGroup);
        
    }


    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var selectedItem = await SmartflowService.GetSmartflowItemById(id);

        if (selectedItem is null)
        {
            return NotFound($"Item with ID = {selectedItem.Id} not found");
        }

        return Ok(await SmartflowService.Delete(id));
        
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteSmartflow(int id)
    {
        var selectedItem = await SmartflowService.GetSmartflowItemById(id);

        if (selectedItem is null)
        {
            return NotFound($"Item with ID = {selectedItem.Id} not found");
        }

        return Ok(await SmartflowService.DeleteSmartflow(id));
        
    }



    [HttpPost()]
    public async Task<ActionResult<bool>> CreateStep(P4W_SmartflowStepSchemaJSONObject schemaJSONObject)
    {
        if (schemaJSONObject is null || string.IsNullOrEmpty(schemaJSONObject.StepSchemaJSON))
        {
            return BadRequest("JSON Empty");
        }

        var success = await SmartflowService.CreateStep(schemaJSONObject.StepSchemaJSON);

        if (!success)
        {
            return BadRequest($"Step Creation Failed");
        }

        return success;
        
    }


}
