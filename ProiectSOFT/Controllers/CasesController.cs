using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProiectSoft.DAL.Models.CasesModels;
using ProiectSoft.DAL.Wrappers;
using ProiectSoft.Services.CasesServices;
using System.ComponentModel.DataAnnotations;

namespace ProiectSOFT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CasesController : Controller
    {
        private readonly ICasesServices _casesService;

        public CasesController(ICasesServices casesService)
        {
            _casesService = casesService;
        }
        //[Authorize(Roles = "Admin")]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] PaginationFilter filter,
            [FromQuery] string? searchCase,
            [FromQuery] string? orderBy,
            [FromQuery] bool descending,
            [FromQuery] string[] filters
        )
        {
            var route = Request.Path.Value;

            var cases = await _casesService.GetAll(filter, route, searchCase, orderBy, descending, filters);

            if (cases.Succeeded == false)
            {
                return NotFound("Something went wrong in GetAll cases query");
            }

            return Ok(cases);
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById([FromQuery] int id)
        {
            var _case = await _casesService.GetById(id);
            
            return Ok(_case);
        }

        [HttpPost("AddCase")]
        public async Task<IActionResult> AddCase([FromBody][Required] CasesPostModel model)
        {
            await _casesService.Create(model);
            
            return Ok("Created succesfully");
        }

        [HttpPut("UpdateCase")]
        public async Task<IActionResult> UpdateCase([FromBody][Required] CasesPutModel model, [FromQuery] int id)
        {
            await _casesService.Update(model, id);

            return Ok("Updated succesfully");
        }

        [HttpDelete("DeleteCase")]
        public async Task<IActionResult> DeleteCase([FromQuery] int id)
        {
            await _casesService.Delete(id);

            return Ok("Deleted succesfully");
        }

    }
}
