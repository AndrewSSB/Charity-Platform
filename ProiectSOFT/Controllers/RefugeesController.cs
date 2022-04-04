﻿using Microsoft.AspNetCore.Mvc;
using ProiectSoft.BLL.Helpers;
using ProiectSoft.DAL.Models.RefugeeModels;
using ProiectSoft.DAL.Wrappers;
using ProiectSoft.Services.RefugeesServices;
using ProiectSoft.Services.UriServicess;

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
        public async Task<IActionResult> GetAll([FromQuery] PaginationFilter filter)
        {
            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            
            var cases = await _refugeeServices.GetAll(validFilter.PageNumber, validFilter.PageSize);

            if (cases == null)
                return BadRequest();

            var totalRecord = await _refugeeServices.CountAsync();
            var pagedResponse = PaginationHelper.CreatePagedReponse<RefugeeGetModel>(cases, validFilter, totalRecord, _uriService, route);
            return Ok(pagedResponse);
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById([FromQuery] int id)
        {
            var _case = await _refugeeServices.GetById(id);

            if (_case == null)
                return BadRequest();

            return Ok(new ResponseW<RefugeeGetModel>(_case));
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
