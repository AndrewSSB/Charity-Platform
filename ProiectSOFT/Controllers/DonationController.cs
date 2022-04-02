using Microsoft.AspNetCore.Mvc;
using ProiectSoft.DAL.Models.DonationModels;
using ProiectSoft.Services.DonationsServices;

namespace ProiectSOFT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonationController : Controller
    {
        private readonly IDonationServices _donationServices;

        public DonationController(IDonationServices locationServices)
        {
            _donationServices = locationServices;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var cases = await _donationServices.GetAll();

            if (cases == null)
                return BadRequest();

            return Ok(cases);
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById([FromQuery] int id)
        {
            var _case = await _donationServices.GetById(id);

            if (_case == null)
                return BadRequest();

            return Ok(_case);
        }

        [HttpPost("AddDonation")]
        public async Task<IActionResult> AddDonation([FromBody] DonationPostModel model)
        {
            if (model == null)
                return BadRequest();

            await _donationServices.Create(model);

            return Ok();
        }

        [HttpPut("UpdateDonation")]
        public async Task<IActionResult> UpdateDonation([FromBody] DonationPutModel model, [FromQuery] int id)
        {
            await _donationServices.Update(model, id);

            return Ok();
        }

        [HttpDelete("DeleteDonation")]
        public async Task<IActionResult> DeleteDonation([FromQuery] int id)
        {
            await _donationServices.Delete(id);

            return Ok();
        }
    }
}
