using Microsoft.AspNetCore.Mvc;
using ProiectSoft.DAL.Models.ShelterModels;
using ProiectSoft.DAL.Wrappers;
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
        public async Task<IActionResult> GetAll([FromQuery] PaginationFilter filter)
        {
            var route = Request.Path.Value;

            var shelters = await _shelterServices.GetAll(filter, route);

            return Ok(shelters);
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById([FromQuery] int id)
        {
            var shelter = await _shelterServices.GetById(id);

            return Ok(new Response<ShelterGetModel>(shelter));
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
        public async Task<IActionResult> UpdateShelter([FromBody] ShelterPutModel model, [FromQuery] int id)
        {
            await _shelterServices.Update(model, id);

            return Ok();
        }

        [HttpDelete("DeleteShelter")]
        public async Task<IActionResult> DeleteShelter([FromQuery] int id)
        {
            await _shelterServices.Delete(id);

            return Ok();
        }
    }
}
