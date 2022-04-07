using Microsoft.AspNetCore.Mvc;
using ProiectSoft.DAL.Models.OrganisationModels;
using ProiectSoft.DAL.Wrappers;
using ProiectSoft.Services.OrganizationsService;
using System.ComponentModel.DataAnnotations;

namespace ProiectSOFT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganisationController : Controller
    {
        private readonly IOrganisationService _organisationService;

        public OrganisationController(IOrganisationService organisationService)
        {
            _organisationService = organisationService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] PaginationFilter filter,
            [FromQuery] string? searchName,
            [FromQuery] string? orderBy,
            [FromQuery] bool descending)
        {
            var route = Request.Path.Value;

            var organisations = await _organisationService.GetAll(filter, route, searchName, orderBy, descending);

            if (organisations.Succeeded == false)
            {
                return NotFound("Something went wrong in GetAll organisations query");
            }

            return Ok(organisations);
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById([FromQuery] int id)
        {
            var organisation = await _organisationService.GetById(id);

            if (organisation.Succeeded)
            {
                return Ok(organisation);
            }

            return NotFound(organisation.Message);
        }

        [HttpPost("AddOrganisation")]
        public async Task<IActionResult> AddOrganisation([FromBody][Required] OrganisationPostModel model)
        {
            if (model == null)
                return BadRequest();

            await _organisationService.Create(model);

            return Ok();
        }

        [HttpPut("UpdateOrganisation")]
        public async Task<IActionResult> UpdateOrganisation([FromBody][Required] OrganisationPutModel model, [FromQuery] int id)
        {
            await _organisationService.Update(model, id);

            return Ok();
        }

        [HttpDelete("DeleteOrganisation")]
        public async Task<IActionResult> DeleteOrganisation([FromQuery] int id)
        {
            await _organisationService.Delete(id);

            return Ok();
        }
    }
}
