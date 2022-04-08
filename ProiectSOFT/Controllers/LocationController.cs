using Microsoft.AspNetCore.Mvc;
using ProiectSoft.DAL.Models.LocationModels;
using ProiectSoft.DAL.Wrappers;
using ProiectSoft.Services.LocationsServices;
using System.ComponentModel.DataAnnotations;

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
        public async Task<IActionResult> GetAll([FromQuery] PaginationFilter filter,
            [FromQuery] string? searchCity,
            [FromQuery] string? orderBy,
            [FromQuery] bool descending,
            [FromQuery] string[] filters)
        {
            var route = Request.Path.Value;

            var locations = await _locationServices.GetAll(filter, route, searchCity, orderBy, descending, filters);

            if (locations.Succeeded == false)
            {
                return NotFound("Something went wrong in GetAll locations query");
            }

            return Ok(locations);
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById([FromQuery] int id)
        {
            var location = await _locationServices.GetById(id);
            
            return Ok(location);
        }

        [HttpPost("AddLocation")]
        public async Task<IActionResult> AddLocation([FromBody][Required] LocationPostModel model)
        {
            await _locationServices.Create(model);

            return Ok("Created succesfully");
        }

        [HttpPut("UpdateLocation")]
        public async Task<IActionResult> UpdateLocation([FromBody][Required] LocationPutModel model, [FromQuery] int id)
        {
            await _locationServices.Update(model, id);

            return Ok("Updated succesfully");
        }

        [HttpDelete("DeleteLocation")]
        public async Task<IActionResult> DeleteLocation([FromQuery] int id)
        {
            await _locationServices.Delete(id);

            return Ok("Deleted succesfully");
        }
    }
}
