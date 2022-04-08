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
            [FromQuery] bool descending,
            [FromQuery] string[] filters)
        {
            var route = Request.Path.Value;

            var organisations = await _organisationService.GetAll(filter, route, searchName, orderBy, descending, filters);

            if (organisations.Succeeded == false)
            {
                return NotFound("Something went wrong in GetAll organisations query"); // pentru get all ar trebui sa primesc foarte greu fail pe request
                                                                                       // momentan nu vad un caz in care se poate intampla, dar am zis sa fie specificata eroarea
            }                                                                          // pot primi maxim o lista goala 

            return Ok(organisations);
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById([FromQuery] int id)
        {
            var organisation = await _organisationService.GetById(id);

            return Ok(organisation);
        }

        [HttpPost("AddOrganisation")]
        public async Task<IActionResult> AddOrganisation([FromBody][Required] OrganisationPostModel model)
        {
            await _organisationService.Create(model);

            return Ok("Created succesfully");
        }

        [HttpPut("UpdateOrganisation")]
        public async Task<IActionResult> UpdateOrganisation([FromBody][Required] OrganisationPutModel model, [FromQuery] int id)
        {
            await _organisationService.Update(model, id);

            return Ok("Updated succesfully");
        }

        [HttpDelete("DeleteOrganisation")]
        public async Task<IActionResult> DeleteOrganisation([FromQuery] int id)
        {
            await _organisationService.Delete(id);

            return Ok("Deleted succesfully");
        }
    }
}
