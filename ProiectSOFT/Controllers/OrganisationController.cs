using Microsoft.AspNetCore.Mvc;
using ProiectSoft.DAL.Models.OrganisationModels;
using ProiectSoft.Services.OrganizationsService;

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
        public async Task<IActionResult> GetAll()
        {
            var cases = await _organisationService.GetAll();

            if (cases == null)
                return BadRequest();

            return Ok(cases);
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById([FromQuery] int id)
        {
            var _case = await _organisationService.GetById(id);

            if (_case == null)
                return BadRequest();

            return Ok(_case);
        }

        [HttpPost("AddOrganisation")]
        public async Task<IActionResult> AddOrganisation([FromBody] OrganisationPostModel model)
        {
            if (model == null)
                return BadRequest();

            await _organisationService.Create(model);

            return Ok();
        }

        [HttpPut("UpdateOrganisation")]
        public async Task<IActionResult> UpdateOrganisation([FromBody] OrganisationPutModel model, [FromQuery] int id)
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
