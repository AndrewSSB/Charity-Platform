using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ProiectSoft.BLL.Helpers;
using ProiectSoft.DAL.Models.RefugeeModels;
using ProiectSoft.DAL.Wrappers;
using ProiectSoft.DAL.Wrappers.Filters;
using ProiectSoft.Services.RefugeesServices;
using ProiectSoft.Services.UriServicess;
using System.ComponentModel.DataAnnotations;

namespace ProiectSOFT.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class RefugeesController : Controller
    {
        private readonly IRefugeeServices _refugeeServices;
        private readonly IUriServices _uriService;

        public RefugeesController(IRefugeeServices refugeeServices, IUriServices uriServices)
        {
            _refugeeServices = refugeeServices;
            _uriService = uriServices;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] RefugeesFilter filter)
        {
            var route = Request.Path.Value;

            var refugees = await _refugeeServices.GetAll(filter, route);

            if (refugees.Succeeded == false)
                return NotFound("Something went wrong in GetAll refugees query");

            return Ok(refugees);
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById([FromQuery] int id)
        {
            var refugee = await _refugeeServices.GetById(id);

            return Ok(refugee);
        }

        [HttpPost("AddRefugee")]
        public async Task<IActionResult> AddRefugee([FromBody][Required] RefugeePostModel model)
        {
            await _refugeeServices.Create(model);

            return Ok("Created succesfully");
        }

        [HttpPut("UpdateRefugee")]
        public async Task<IActionResult> UpdateRefugee([FromBody][Required] RefugeePutModel model, [FromQuery] int id)
        {
            await _refugeeServices.Update(model, id);

            return Ok("Updated succesfully");
        }

        [HttpDelete("DeleteRefugee")]
        public async Task<IActionResult> DeleteRefugee([FromQuery] int id)
        {
            await _refugeeServices.Delete(id);

            return Ok("Deleted succesfully");
        }
    }
}
