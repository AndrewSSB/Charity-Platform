using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProiectSoft.BLL.Helpers;
using ProiectSoft.DAL;
using ProiectSoft.DAL.Entities;
using ProiectSoft.DAL.Models.CasesModels;
using ProiectSoft.DAL.Wrappers;
using ProiectSoft.DAL.Wrappers.Filters;
using ProiectSoft.Services.UriServicess;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.MiddlewareManager;

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
                throw new KeyNotFoundException($"There is no case with id: {id}, try with another one");
            }

            _context.Cases.Remove(_case);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<List<CasesGetModel>>> GetAll(CasesFilter filter, string route)
        {
            LogContext.PushProperty("IdentificationMessage", $"GetAll case request for Page:{filter.PageNumber}, with number of objects: {filter.PageSize}");

            IQueryable<Cases> cases = _context.Cases;

            if (!string.IsNullOrEmpty(filter.searchName))
            {
                cases = cases.Where(x => x.caseName!.Contains(filter.searchName));
            }

            if (!string.IsNullOrEmpty(filter.filterName))
            {
                cases = cases.Where(x => x.caseName.Contains(filter.filterName));
            }

            if (filter.filterDate != null)
            {
                cases = cases.Where(x => x.startDate.Value.ToString("mm/dd/yyyy").Contains(filter.filterDate.Value.ToString("mm/dd/yyyy")));
            }

            switch (filter.orderBy) 
            {
                case "Name":
                    cases = !filter.descending ? cases.OrderBy(x => x.caseName) : cases.OrderByDescending(x => x.caseName);
                    break;
                case "Date":
                    cases = !filter.descending ? cases.OrderBy(s => s.DateCreated) : cases.OrderByDescending(x => x.DateCreated);
                    break;
                case "Age":
                    cases = !filter.descending ? cases.OrderBy(s => s.endDate) : cases.OrderByDescending(x => x.endDate);
                    break;
                default:
                    cases = !filter.descending ? cases.OrderBy(s => s.Id) : cases.OrderByDescending(x => x.Id);
                    break;
            }

            var casesModels = cases
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(_mapper.Map<CasesGetModel>)
                .ToList();

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
                throw new KeyNotFoundException($"There is no case with id: {id}");
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
                throw new AppException($"There is no case with id: {id}, try with another one");
            }

            _mapper.Map<CasesPutModel, Cases>(model, _case);

            await _context.SaveChangesAsync();
        }
    }
}
