using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ProiectSoft.BLL.Helpers;
using ProiectSoft.DAL.Models.RefugeeModels;
using ProiectSoft.DAL.Wrappers;
using ProiectSoft.Services.RefugeesServices;
using ProiectSoft.Services.UriServicess;
using System.ComponentModel.DataAnnotations;

namespace ProiectSOFT.Controllers
{
    [Route("api/[controller]")]
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
        public async Task<IActionResult> GetAll([FromQuery] PaginationFilter filter,
            [FromQuery] string? searchName,
            [FromQuery] string? orderBy,
            [FromQuery] bool descending
        )
        {
            var route = Request.Path.Value;

            var refugees = await _refugeeServices.GetAll(filter, route, searchName, orderBy, descending);

            if (refugees.Succeeded == false)
                return NotFound("Something went wrong in GetAll refugees query");

            return Ok(refugees);
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById([FromQuery] int id)
        {
            var refugee = await _refugeeServices.GetById(id);

            if (refugee.Succeeded)
            {
                return Ok(refugee);      
            }

            return NotFound(refugee.Message);
        }

        [HttpPost("AddRefugee")]
        public async Task<IActionResult> AddRefugee([FromBody][Required] RefugeePostModel model)
        {
            if (model == null)
                return BadRequest();

            await _refugeeServices.Create(model);

            return Ok();
        }

        [HttpPut("UpdateRefugee")]
        public async Task<IActionResult> UpdateRefugee([FromBody][Required] RefugeePutModel model, [FromQuery] int id)
        {
            await _refugeeServices.Update(model, id);

            return Ok();
        }

        [HttpDelete("DeleteRefugee")]
        public async Task<IActionResult> DeleteRefugee([FromQuery] int id)
        {
            await _refugeeServices.Delete(id);

            return Ok();
        }
    }
}
