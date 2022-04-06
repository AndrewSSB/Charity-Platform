using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProiectSoft.BLL.Helpers;
using ProiectSoft.DAL;
using ProiectSoft.DAL.Entities;
using ProiectSoft.DAL.Models.CasesModels;
using ProiectSoft.DAL.Wrappers;
using ProiectSoft.Services.UriServicess;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.Services.CasesServices
{
    public class CasesService : ICasesServices
    {
        private readonly AppDbContext _context;
        private readonly IUriServices _uriServices;
        private readonly IMapper _mapper;
        private readonly ILogger<CasesService> _logger;
        public CasesService(AppDbContext context, IUriServices uriServices, IMapper mapper, ILogger<CasesService> logger)
        {
            _context = context;
            _uriServices = uriServices;
            _mapper = mapper;   
            _logger = logger;   
        }

        public async Task Create(CasesPostModel model)
        {
            LogContext.PushProperty("IdentificationMessage", $"Create case request for {model.caseName}");

            var _case = _mapper.Map<Cases>(model);

            await _context.AddAsync(_case);
            await _context.SaveChangesAsync();
            
        }
        public async Task Delete(int id)
        {
            LogContext.PushProperty("IdentificationMessage", $"Delete case request for {id}");

            var _case = await _context.Cases.FirstOrDefaultAsync(x => x.Id == id);
            
            if (_case == null)
            {
                _logger.LogError($"OPS! Case with id:{id} does not exist in our database");
                return;
            }

            _context.Cases.Remove(_case);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<List<CasesGetModel>>> GetAll(PaginationFilter filter, string route)
        {
            LogContext.PushProperty("IdentificationMessage", $"GetAll case request for Page:{filter.PageNumber}, with number of objects: {filter.PageSize}");

            var cases = await _context.Cases
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var casesModels = new List<CasesGetModel>();

            foreach (var _case in cases)
            {
                var _caseModel = _mapper.Map<CasesGetModel>(_case);
                casesModels.Add(_caseModel);
            }

            var casesListCount = await _context.Cases.CountAsync();

            var pagedResponse = PaginationHelper.CreatePagedReponse<CasesGetModel>(casesModels, filter, casesListCount, _uriServices, route);

            return pagedResponse;
        }

        public async Task<Response<CasesGetModel>> GetById(int id)
        {
            LogContext.PushProperty("IdentificationMessage", $"GetById case request for {id}");

            var _case = await _context.Cases.FirstOrDefaultAsync(x => x.Id == id);

            if (_case == null)
            {
                _logger.LogError($"OPS! Case with id:{id} does not exist in our database");
                return new Response<CasesGetModel>(false, $"Id {id} doesn't exist");
            }

            var caseGetModel = _mapper.Map<CasesGetModel>(_case);

            return new Response<CasesGetModel>(caseGetModel);
        }

        public async Task Update(CasesPutModel model, int id)
        {
            LogContext.PushProperty("IdentificationMessage", $"Update case request for {model.caseName}");

            var _case = await _context.Cases.FirstOrDefaultAsync(x => x.Id == id);

            if (_case == null)
            {
                _logger.LogError($"There is no case with ID:{id} in our database");
                return;
            }

            _mapper.Map<CasesPutModel, Cases>(model, _case);

            await _context.SaveChangesAsync();
        }
    }
}
