using Microsoft.AspNetCore.Mvc;
using ProiectSoft.DAL.Models.ShelterModels;
using ProiectSoft.Services.SheltersServices;

namespace ProiectSOFT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShelterController : Controller
    {
        private readonly IShelterServices _shelterServices;

        public ShelterController(IShelterServices locationServices)
        {
            _shelterServices = locationServices;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var cases = await _shelterServices.GetAll();

            if (cases == null)
                return BadRequest();

            return Ok(cases);
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById([FromQuery] int id)
        {
            var _case = await _shelterServices.GetById(id);

            if (_case == null)
                return BadRequest();

            return Ok(_case);
        }

        [HttpPost("AddShelter")]
        public async Task<IActionResult> AddShelter([FromBody] ShelterPostModel model)
        {
            if (model == null)
                return BadRequest();

            await _shelterServices.Create(model);

            return Ok();
        }

        [HttpPut("UpdateShelter")]
        public async Task<IActionResult> UpdateLocation([FromBody] ShelterPutModel model, [FromQuery] int id)
        {
            await _shelterServices.Update(model, id);

            return Ok();
        }

        [HttpDelete("DeleteShelter")]
        public async Task<ActionResult> DeleteLocation([FromQuery] int id)
        {
            await _shelterServices.Delete(id);

            return Ok();
        }
    }
}
