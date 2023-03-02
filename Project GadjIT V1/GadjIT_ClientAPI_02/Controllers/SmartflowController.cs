using Microsoft.AspNetCore.Mvc;
using GadjIT_ClientAPI.Services.Smartflow;
using GadjIT_ClientContext.P4W;
using GadjIT_ClientContext.P4W.Custom;


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
        Logger.LogInformation("API Test: {0}",$"Application is working");
        return Ok("Test Complete: Application is working");
    }
    
    [HttpGet]
    public async Task<ActionResult> TestGet()
    {
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
    public async Task<ActionResult> GetAllChapters()
    {
        return Ok(await SmartflowService.GetAllChapters());
    }



    [HttpGet("{caseType}")]
    public async Task<ActionResult> GetChapterListByCaseType(string caseType)
    {
        return Ok(await SmartflowService.GetChapterListByCaseType(caseType));
        
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
    public async Task<ActionResult> Add(UsrOrsfSmartflows item)
    {

        
        if (item is null)
        {
            return BadRequest();
        }

        var newItem = await SmartflowService.Add(item);

        return CreatedAtAction(nameof(Add), new { id = newItem.Id });
        
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<UsrOrsfSmartflows>> Update(int Id, UsrOrsfSmartflows item)
    {
        if (Id != item.Id)
        {
            return BadRequest("Id missmatch");
        }

        var selectedItem = await SmartflowService.GetChapterItemById(item.Id);

        if (selectedItem is null)
        {
            return NotFound($"Item with ID = {Id} not found");
        }

        return await SmartflowService.Update(item);
       
    }


    [HttpPut("{newCaseType}/{originalCaseType}/{caseTypeGroup}")]
    public async Task<ActionResult<List<UsrOrsfSmartflows>>> UpdateCaseType(string newCaseType, string originalCaseType, string caseTypeGroup)
    {
        return await SmartflowService.UpdateCaseType(newCaseType, originalCaseType,caseTypeGroup);
        
    }

    [HttpPut("{newCaseTypeGroup}/{originalCaseTypeGroup}")]
    public async Task<ActionResult<List<UsrOrsfSmartflows>>> UpdateCaseTypeGroups(string newCaseTypeGroup, string originalCaseTypeGroup)
    {
        return await SmartflowService.UpdateCaseTypeGroups(newCaseTypeGroup, originalCaseTypeGroup);
        
    }


    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var selectedItem = await SmartflowService.GetChapterItemById(id);

        if (selectedItem is null)
        {
            return NotFound($"Item with ID = {selectedItem.Id} not found");
        }

        return Ok(await SmartflowService.Delete(id));
        
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteChapter(int id)
    {
        var selectedItem = await SmartflowService.GetChapterItemById(id);

        if (selectedItem is null)
        {
            return NotFound($"Item with ID = {selectedItem.Id} not found");
        }

        return Ok(await SmartflowService.DeleteChapter(id));
        
    }



    [HttpPost()]
    public async Task<ActionResult<bool>> CreateStep(VmSmartflowP4WStepSchemaJSONObject schemaJSONObject)
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
