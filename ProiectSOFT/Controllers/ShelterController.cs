using Microsoft.AspNetCore.Mvc;
using ProiectSoft.DAL.Models.ShelterModels;
using ProiectSoft.DAL.Wrappers;
using ProiectSoft.Services.SheltersServices;
using System.ComponentModel.DataAnnotations;

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
        public async Task<IActionResult> GetAll([FromQuery] PaginationFilter filter,
            [FromQuery] string? searchName,
            [FromQuery] string? orderBy,
            [FromQuery] bool descending,
            [FromQuery] string[] filters)
        {
            var route = Request.Path.Value;

            var shelters = await _shelterServices.GetAll(filter, route, searchName, orderBy, descending, filters);

            if (shelters.Succeeded == false)
            {
                return NotFound("Something went wrong in GetAll shelters query");
            }

            return Ok(shelters);
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById([FromQuery] int id)
        {
            var shelter = await _shelterServices.GetById(id);

            return Ok(shelter);
        }

        [HttpPost("AddShelter")]
        public async Task<IActionResult> AddShelter([FromBody][Required] ShelterPostModel model)
        {
            await _shelterServices.Create(model);

            return Ok("Created succesfully");
        }

        [HttpPut("UpdateShelter")]
        public async Task<IActionResult> UpdateShelter([FromBody][Required] ShelterPutModel model, [FromQuery] int id)
        {
            await _shelterServices.Update(model, id);

            return Ok("Updated succesfully");
        }

        [HttpDelete("DeleteShelter")]
        public async Task<IActionResult> DeleteShelter([FromQuery] int id)
        {
            await _shelterServices.Delete(id);

            return Ok("Deleted succesfully");
        }
    }
}
