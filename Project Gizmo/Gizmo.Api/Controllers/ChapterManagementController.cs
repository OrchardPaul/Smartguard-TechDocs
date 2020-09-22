using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gizmo.Api.Repository.OR_RESI;
using Microsoft.AspNetCore.Http;
using System.Net.Http;

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



        [HttpGet]
        public async Task<ActionResult> GetChapterListByCaseType(string caseType)
        {
            try
            {
                return Ok(await chapterRepository.GetChapterListByCaseType(caseType));
            }catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }




    }
}
