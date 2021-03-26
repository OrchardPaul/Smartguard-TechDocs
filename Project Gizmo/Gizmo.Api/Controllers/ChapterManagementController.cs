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

        [HttpGet("{caseTypeGroup}/{caseType}")]
        public async Task<ActionResult> GetFeeDefs(string caseTypeGroup, string caseType)
        {
            try
            {
                return Ok(await chapterRepository.GetFeeDefs(caseTypeGroup,caseType));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

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

        [HttpGet("{caseType}/{chapter}")]
        public async Task<ActionResult> GetDocListByChapter(string caseType, string chapter)
        {
            try
            {
                return Ok(await chapterRepository.GetDocListByChapter(caseType, chapter));
            }
            catch (Exception)
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

        [HttpGet("{chapterId:int}")]
        public async Task<ActionResult> GetItemListByChapter(int chapterId)
        {
            try
            {
                return Ok(await chapterRepository.GetItemListByChapter(chapterId));
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

        [HttpGet("{casetypegroup}/{casetype}/{chapterName}")]
        public async Task<ActionResult> GetItemListByChapterName(string casetypegroup, string casetype, string chapterName)
        {
            try
            {
                return Ok(await chapterRepository.GetItemListByChapterName(casetypegroup, casetype, chapterName));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        [HttpGet("{caseType}/{chapter}/{docType}")]
        public async Task<ActionResult> GetDocListByChapterAndDocType(string caseType, string chapter, string docType)
        {
            try
            {
                return Ok(await chapterRepository.GetDocListByChapterAndDocType(caseType, chapter, docType));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Add(UsrOrDefChapterManagement item)
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
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<UsrOrDefChapterManagement>> Update(int Id, UsrOrDefChapterManagement item)
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

        [HttpPut("{ChapterId:int}")]
        public async Task<ActionResult<List<VmChapterFee>>> UpdateChapterFees(int ChapterId, List<VmChapterFee> vmChapterFees)
        {
            try
            {
                return await chapterRepository.UpdateChapterFees(ChapterId,vmChapterFees);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error Updating Data");
            }
        }

        [HttpPut("{newCaseType}/{originalCaseType}/{caseTypeGroup}")]
        public async Task<ActionResult<List<UsrOrDefChapterManagement>>> UpdateCaseType(string newCaseType, string originalCaseType, string caseTypeGroup)
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
        public async Task<ActionResult<List<UsrOrDefChapterManagement>>> UpdateCaseTypeGroups(string newCaseTypeGroup, string originalCaseTypeGroup)
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



        [HttpPost]
        public async Task<ActionResult<bool>> CreateStep(string JSON)
        {
            try
            {
                if (string.IsNullOrEmpty(JSON))
                {
                    return BadRequest("JSON Empty");
                }

                var success = await chapterRepository.CreateStep(JSON);

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
