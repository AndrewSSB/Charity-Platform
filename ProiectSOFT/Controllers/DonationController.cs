using Microsoft.AspNetCore.Mvc;
using ProiectSoft.DAL.Models.DonationModels;
using ProiectSoft.DAL.Wrappers;
using ProiectSoft.Services.DonationsServices;
using System.ComponentModel.DataAnnotations;

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
        public async Task<IActionResult> GetAll([FromQuery] PaginationFilter filter,
            [FromQuery] string? orderBy,
            [FromQuery] bool descending)
        {
            var route = Request.Path.Value;

            var donations = await _donationServices.GetAll(filter, route, orderBy, descending);

            if (donations.Succeeded == false)
            {
                return NotFound("Something went wrong in GetAll donations query");
            }

            return Ok(donations);
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById([FromQuery] int id)
        {
            var donation = await _donationServices.GetById(id);

            return Ok(donation);
        }

        [HttpPost("AddDonation")]
        public async Task<IActionResult> AddDonation([FromBody][Required] DonationPostModel model)
        {
            await _donationServices.Create(model);

            return Ok("Created succesfully");
        }

        [HttpPut("UpdateDonation")]
        public async Task<IActionResult> UpdateDonation([FromBody][Required] DonationPutModel model, [FromQuery] int id)
        {
            await _donationServices.Update(model, id);

            return Ok("Updated succesfully");
        }

        [HttpDelete("DeleteDonation")]
        public async Task<IActionResult> DeleteDonation([FromQuery] int id)
        {
            await _donationServices.Delete(id);

            return Ok("Deleted succesfully");
        }
    }
}
