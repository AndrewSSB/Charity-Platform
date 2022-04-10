using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProiectSoft.DAL.Models.UserModels;
using ProiectSoft.DAL.Wrappers;
using ProiectSoft.DAL.Wrappers.Filters;
using ProiectSoft.Services.UsersServices;
using System.ComponentModel.DataAnnotations;

namespace ProiectSOFT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly IUserServices _userServices;

        public UserController(IUserServices userServices)
        {
            _userServices = userServices;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] UserFilter filter)
        {
            var route = Request.Path.Value;

            var users = await _userServices.GetAll(filter, route);

            if (users.Succeeded == false)
            {
                return NotFound("Something went wrong in GetAll users query");
            }

            return Ok(users);
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById([FromQuery] Guid id)
        {
            var user = await _userServices.GetById(id);

            return Ok(user);
        }

        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody][Required] UserPutModel model, [FromQuery] Guid id)
        {
            await _userServices.Update(model, id);

            return Ok("Updated succesfully");
        }

        [HttpDelete("DeleteUser")]
        public async Task<IActionResult> DeleteUser([FromQuery] Guid id)
        {
            await _userServices.Delete(id);

            return Ok("Deleted succesfully");
        }
    }
}
