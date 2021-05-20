using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using GadjIT.ClientContext.P4W;
using GadjIT.ClientContext.P4W.Custom;
using GadjIT.ClientAPI.Repository.Chapters;

namespace GadjIT.ClientAPI.Controllers
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
        public async Task<ActionResult> GetAllChapters()
        {
            return Ok(await chapterRepository.GetAllChapters());

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
        public async Task<ActionResult> GetChapterListByCaseType(string caseType)
        {
            try
            {
                return Ok(await chapterRepository.GetChapterListByCaseType(caseType));
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
        public async Task<ActionResult> Add(UsrOrsfSmartflows item)
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
        public async Task<ActionResult<UsrOrsfSmartflows>> Update(int Id, UsrOrsfSmartflows item)
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
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error Updating Data");
            }
        }


        [HttpPut("{newCaseType}/{originalCaseType}/{caseTypeGroup}")]
        public async Task<ActionResult<List<UsrOrsfSmartflows>>> UpdateCaseType(string newCaseType, string originalCaseType, string caseTypeGroup)
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
        public async Task<ActionResult<List<UsrOrsfSmartflows>>> UpdateCaseTypeGroups(string newCaseTypeGroup, string originalCaseTypeGroup)
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



        [HttpPut()]
        public async Task<ActionResult<bool>> CreateStep(VmChapterP4WStepSchemaJSONObject schemaJSONObject)
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
