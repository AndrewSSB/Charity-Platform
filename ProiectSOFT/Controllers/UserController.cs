using Microsoft.AspNetCore.Mvc;
using ProiectSoft.DAL.Models.UserModels;
using ProiectSoft.Services.UsersServices;

namespace ProiectSOFT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserServices _userServices;

        public UserController(IUserServices userServices)
        {
            _userServices = userServices;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var cases = await _userServices.GetAll();

            if (cases == null)
                return BadRequest();

            return Ok(cases);
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById([FromQuery] Guid id)
        {
            var _case = await _userServices.GetById(id);

            if (_case == null)
                return BadRequest();

            return Ok(_case);
        }

        [HttpPost("AddUser")]
        public async Task<IActionResult> AddUser([FromBody] UserPostModel model)
        {
            if (model == null)
                return BadRequest();

            await _userServices.Create(model);

            return Ok();
        }

        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] UserPutModel model, [FromQuery] Guid id)
        {
            await _userServices.Update(model, id);

            return Ok();
        }

        [HttpDelete("DeleteUser")]
        public async Task<ActionResult> DeleteUser([FromQuery] Guid id)
        {
            await _userServices.Delete(id);

            return Ok();
        }
    }
}
