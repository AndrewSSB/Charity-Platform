using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProiectSoft.DAL.Models.CasesModels;
using ProiectSoft.DAL.Wrappers;
using ProiectSoft.Services.CasesServices;

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
        public async Task<IActionResult> GetAll([FromQuery] PaginationFilter filter)
        {
            var route = Request.Path.Value;

            var cases = await _casesService.GetAll(filter, route);

            return Ok(cases);
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById([FromQuery] int id)
        {
            var _case = await _casesService.GetById(id);

            return Ok(new Response<CasesGetModel>(_case));
        }

        [HttpPost("AddCase")]
        public async Task<IActionResult> AddCase([FromBody] CasesPostModel model)
        {
            if (model == null)
                return BadRequest();

            await _casesService.Create(model);
            
            return Ok();
        }

        [HttpPut("UpdateCase")]
        public async Task<IActionResult> UpdateCase([FromBody] CasesPutModel model, [FromQuery] int id)
        {
            await _casesService.Update(model, id);

            return Ok();
        }

        [HttpDelete("DeleteCase")]
        public async Task<IActionResult> DeleteCase([FromQuery] int id)
        {
            await _casesService.Delete(id);

            return Ok();
        }

    }
}
