using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gizmo.Api.Repository.OR_RESI;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using Gizmo.Context.OR_RESI;

namespace Gizmo.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ChapterManagementController : ControllerBase
    {
        private readonly IOR_RESI_Chapters_Service chapterRepository;

        public ChapterManagementController(IOR_RESI_Chapters_Service ChapterRepository)
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
            try
            {
                return Ok(await chapterRepository.GetAllChapters());
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



    }
}
