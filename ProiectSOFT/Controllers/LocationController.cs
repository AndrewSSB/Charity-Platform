using Microsoft.AspNetCore.Mvc;
using ProiectSoft.DAL.Models.LocationModels;
using ProiectSoft.Services.LocationsServices;

namespace ProiectSOFT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : Controller
    {
        private readonly ILocationServices _locationServices;

        public LocationController(ILocationServices locationServices)
        {
            _locationServices = locationServices;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var cases = await _locationServices.GetAll();

            if (cases == null)
                return BadRequest();

            return Ok(cases);
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById([FromQuery] int id)
        {
            var _case = await _locationServices.GetById(id);

            if (_case == null)
                return BadRequest();

            return Ok(_case);
        }

        [HttpPost("AddLocation")]
        public async Task<IActionResult> AddLocation([FromBody] LocationPostModel model)
        {
            if (model == null)
                return BadRequest();

            await _locationServices.Create(model);

            return Ok();
        }

        [HttpPut("UpdateLocation")]
        public async Task<IActionResult> UpdateLocation([FromBody] LocationPutModel model, [FromQuery] int id)
        {
            await _locationServices.Update(model, id);

            return Ok();
        }

        [HttpDelete("DeleteLocation")]
        public async Task<ActionResult> DeleteLocation([FromQuery] int id)
        {
            await _locationServices.Delete(id);

            return Ok();
        }
    }
}
