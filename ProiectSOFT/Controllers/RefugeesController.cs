using Microsoft.AspNetCore.Mvc;
using ProiectSoft.DAL.Models.RefugeeModels;
using ProiectSoft.Services.RefugeesServices;

namespace ProiectSOFT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RefugeesController : Controller
    {
        private readonly IRefugeeServices _refugeeServices;

        public RefugeesController(IRefugeeServices refugeeServices)
        {
            _refugeeServices = refugeeServices;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var cases = await _refugeeServices.GetAll();

            if (cases == null)
                return BadRequest();

            return Ok(cases);
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById([FromQuery] int id)
        {
            var _case = await _refugeeServices.GetById(id);

            if (_case == null)
                return BadRequest();

            return Ok(_case);
        }

        [HttpPost("AddRefugee")]
        public async Task<IActionResult> AddRefugee([FromBody] RefugeePostModel model)
        {
            if (model == null)
                return BadRequest();

            await _refugeeServices.Create(model);

            return Ok();
        }

        [HttpPut("UpdateRefugee")]
        public async Task<IActionResult> UpdateRefugee([FromBody] RefugeePutModel model, [FromQuery] int id)
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
