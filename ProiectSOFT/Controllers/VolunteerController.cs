using Microsoft.AspNetCore.Mvc;
using ProiectSoft.DAL.Models.VolunteerModels;
using ProiectSoft.DAL.Wrappers;
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
        public async Task<IActionResult> GetAll([FromQuery] PaginationFilter filter)
        {
            var route = Request.Path.Value;

            var volunteers = await _volunteerServices.GetAll(filter, route);

            return Ok(volunteers);
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById([FromQuery] int id)
        {
            var volunteer = await _volunteerServices.GetById(id);

            return Ok(new Response<VolunteerGetModel>(volunteer));
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
        public async Task<IActionResult> DeleteVolunteer([FromQuery] int id)
        {
            await _volunteerServices.Delete(id);

            return Ok();
        }
    }
}
