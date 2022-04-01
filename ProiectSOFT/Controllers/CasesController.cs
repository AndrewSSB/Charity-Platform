using Microsoft.AspNetCore.Mvc;
using ProiectSoft.DAL.Models.CasesModels;
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

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var cases = await _casesService.GetAll();

            if (cases == null)
                return BadRequest();

            return Ok(cases);
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById([FromQuery] Guid id)
        {
            var _case = await _casesService.GetById(id);

            if (_case == null)
                return BadRequest();

            return Ok(_case);
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
        public async Task<IActionResult> UpdateCase([FromBody] CasesPutModel model, [FromQuery] Guid id)
        {
            await _casesService.Update(model, id);

            return Ok();
        }

        [HttpDelete("DeleteCase")]
        public async Task<ActionResult> DeleteCase([FromQuery] Guid id)
        {
            await _casesService.Delete(id);

            return Ok();
        }

    }
}
