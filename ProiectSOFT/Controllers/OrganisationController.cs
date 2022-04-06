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
        public async Task<IActionResult> GetAll([FromQuery] PaginationFilter filter)
        {
            var route = Request.Path.Value;

            var organisations = await _organisationService.GetAll(filter, route);

            return Ok(organisations);
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById([FromQuery] int id)
        {
            var organisation = await _organisationService.GetById(id);

            return Ok(new Response<OrganisationGetModel>(organisation));
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
