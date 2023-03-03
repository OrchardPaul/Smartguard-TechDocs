using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using GadjIT_ClientContext.P4W;
using GadjIT_ClientContext.P4W.Custom;
using GadjIT_ClientAPI.Repository.Chapters;

namespace GadjIT_ClientAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ChapterManagementController : ControllerBase
    {
        private readonly IChapters_Service chapterRepository;

        public ChapterManagementController(IChapters_Service ChapterRepository)
        {
            this.chapterRepository = ChapterRepository;
        }

        [HttpGet]
        public async Task<ActionResult> GetCaseTypeGroup()
        {
            try
            {
                return Ok(await chapterRepository.GetCaseTypeGroup());
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetDatabaseTableDateFields()
        {
            try
            {
                return Ok(await chapterRepository.GetDatabaseTableDateFields());
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetCaseTypes()
        {
            try
            {
                return Ok(await chapterRepository.GetCaseTypes());
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetAllSmartflows()
        {
            return Ok(await chapterRepository.GetAllSmartflows());

        }

        [HttpGet]
        public async Task<ActionResult> TestApi()
        {
            return Ok("Test");

        }

        //[HttpGet("{caseTypeGroup}/{caseType}")]
        //public async Task<ActionResult> GetFeeDefs(string caseTypeGroup, string caseType)
        //{
        //    try
        //    {
        //        return Ok(await chapterRepository.GetFeeDefs(caseTypeGroup, caseType));
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError);
        //    }
        //}

        [HttpGet("{caseType}")]
        public async Task<ActionResult> GetSmartflowListByCaseType(string caseType)
        {
            try
            {
                return Ok(await chapterRepository.GetSmartflowListByCaseType(caseType));
            } catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("{caseType}")]
        public async Task<ActionResult> GetDocumentList(string caseType)
        {
            try
            {
                return Ok(await chapterRepository.GetDocumentList(caseType));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("{caseTypeGroupRef:int}")]
        public async Task<ActionResult> GetDocumentListByCaseTypeGroupRef(int caseTypeGroupRef)
        {
            try
            {
                return Ok(await chapterRepository.GetDocumentListByCaseTypeGroupRef(caseTypeGroupRef));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Add(Client_SmartflowRecord item)
        {

            try
            {
                if (item is null)
                {
                    return BadRequest();
                }

                var newItem = await chapterRepository.Add(item);

                return CreatedAtAction(nameof(Add), new { id = newItem.Id });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Client_SmartflowRecord>> Update(int Id, Client_SmartflowRecord item)
        {
            try
            {
                if (Id != item.Id)
                {
                    return BadRequest("Id missmatch");
                }

                var selectedItem = await chapterRepository.GetChapterItemById(item.Id);

                if (selectedItem is null)
                {
                    return NotFound($"Item with ID = {Id} not found");
                }

                return await chapterRepository.Update(item);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error Updating Data");
            }
        }


        [HttpPut("{newCaseType}/{originalCaseType}/{caseTypeGroup}")]
        public async Task<ActionResult<List<Client_SmartflowRecord>>> UpdateCaseType(string newCaseType, string originalCaseType, string caseTypeGroup)
        {
            try
            {
                return await chapterRepository.UpdateCaseType(newCaseType, originalCaseType,caseTypeGroup);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error Updating Data");
            }
        }

        [HttpPut("{newCaseTypeGroup}/{originalCaseTypeGroup}")]
        public async Task<ActionResult<List<Client_SmartflowRecord>>> UpdateCaseTypeGroups(string newCaseTypeGroup, string originalCaseTypeGroup)
        {
            try
            {
                return await chapterRepository.UpdateCaseTypeGroups(newCaseTypeGroup, originalCaseTypeGroup);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error Updating Data");
            }
        }


        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var selectedItem = await chapterRepository.GetChapterItemById(id);

                if (selectedItem is null)
                {
                    return NotFound($"Item with ID = {selectedItem.Id} not found");
                }

                return Ok(await chapterRepository.Delete(id));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error Deleting Data");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteChapter(int id)
        {
            try
            {
                var selectedItem = await chapterRepository.GetChapterItemById(id);

                if (selectedItem is null)
                {
                    return NotFound($"Item with ID = {selectedItem.Id} not found");
                }

                return Ok(await chapterRepository.DeleteChapter(id));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error Deleting Data");
            }
        }



        [HttpPost()]
        public async Task<ActionResult<bool>> CreateStep(P4W_SmartflowStepSchemaJSONObject schemaJSONObject)
        {
            try
            {
                if (schemaJSONObject is null || string.IsNullOrEmpty(schemaJSONObject.StepSchemaJSON))
                {
                    return BadRequest("JSON Empty");
                }

                var success = await chapterRepository.CreateStep(schemaJSONObject.StepSchemaJSON);

                if (!success)
                {
                    return BadRequest($"Step Creation Failed");
                }

                return success;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error Updating Data");
            }
        }


    }
}
