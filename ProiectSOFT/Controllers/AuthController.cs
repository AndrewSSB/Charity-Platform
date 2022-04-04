using Microsoft.AspNetCore.Mvc;
using ProiectSoft.BLL.Interfaces;
using ProiectSoft.BLL.Models;
using ProiectSoft.BLL.Models.Login_Model;
using ProiectSoft.BLL.Models.RegisterModel;

namespace ProiectSOFT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IAuthManager _authManager;

        public AuthController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            var result = await _authManager.Register(model);

            if (result)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var result = await _authManager.Login(model);
            if (result != null)
            {
                return Ok(result);
            }

            return BadRequest();
        }
    }
}
