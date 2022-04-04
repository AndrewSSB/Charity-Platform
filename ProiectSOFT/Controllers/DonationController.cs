using Microsoft.AspNetCore.Mvc;
using ProiectSoft.DAL.Models.DonationModels;
using ProiectSoft.DAL.Wrappers;
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
        public async Task<IActionResult> GetAll([FromQuery] PaginationFilter filter)
        {
            var route = Request.Path.Value;

            var donations = await _donationServices.GetAll(filter, route);

            return Ok(donations);
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById([FromQuery] int id)
        {
            var donation = await _donationServices.GetById(id);

            return Ok(new Response<DonationGetModel>(donation));
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
