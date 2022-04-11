using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProiectSoft.DAL.Models.VolunteerModels;
using ProiectSoft.DAL.Wrappers;
using ProiectSoft.DAL.Wrappers.Filters;
using ProiectSoft.Services.VolunteersServices;
using System.ComponentModel.DataAnnotations;

namespace ProiectSOFT.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class VolunteerController : Controller
    {
        private readonly IVolunteerServices _volunteerServices;

        public VolunteerController(IVolunteerServices volunteerServices)
        {
            _volunteerServices = volunteerServices;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] VolunteerFilter filter)
        {
            var route = Request.Path.Value;

            var volunteers = await _volunteerServices.GetAll(filter, route);

            if (volunteers.Succeeded == false)
            {
                return NotFound("Something went wrong in GetAll volunteers query");
            }

            return Ok(volunteers);
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById([FromQuery] int id)
        {
            var volunteer = await _volunteerServices.GetById(id);

            return Ok(volunteer);
        }

        [HttpPost("AddVolunteer")]
        public async Task<IActionResult> AddVolunteer([FromBody][Required] VolunteerPostModel model)
        {
            await _volunteerServices.Create(model);

            return Ok("Created succesfully");
        }

        [HttpPut("UpdateVolunteer")]
        public async Task<IActionResult> UpdateVolunteer([FromBody][Required] VolunteerPutModel model, [FromQuery] int id)
        {
            await _volunteerServices.Update(model, id);

            return Ok("Updated succesfully");
        }

        [HttpDelete("DeleteVolunteer")]
        public async Task<IActionResult> DeleteVolunteer([FromQuery] int id)
        {
            await _volunteerServices.Delete(id);

            return Ok("Deleted succesfully");
        }
    }
}
