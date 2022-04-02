using Microsoft.AspNetCore.Mvc;
using ProiectSoft.DAL.Models.VolunteerModels;
using ProiectSoft.Services.VolunteersServices;

namespace ProiectSOFT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VolunteerController : Controller
    {
        private readonly IVolunteerServices _volunteerServices;

        public VolunteerController(IVolunteerServices volunteerServices)
        {
            _volunteerServices = volunteerServices;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var cases = await _volunteerServices.GetAll();

            if (cases == null)
                return BadRequest();

            return Ok(cases);
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById([FromQuery] int id)
        {
            var _case = await _volunteerServices.GetById(id);

            if (_case == null)
                return BadRequest();

            return Ok(_case);
        }

        [HttpPost("AddVolunteer")]
        public async Task<IActionResult> AddVolunteer([FromBody] VolunteerPostModel model)
        {
            if (model == null)
                return BadRequest();

            await _volunteerServices.Create(model);

            return Ok();
        }

        [HttpPut("UpdateVolunteer")]
        public async Task<IActionResult> UpdateVolunteer([FromBody] VolunteerPutModel model, [FromQuery] int id)
        {
            await _volunteerServices.Update(model, id);

            return Ok();
        }

        [HttpDelete("DeleteVolunteer")]
        public async Task<ActionResult> DeleteVolunteer([FromQuery] int id)
        {
            await _volunteerServices.Delete(id);

            return Ok();
        }
    }
}
