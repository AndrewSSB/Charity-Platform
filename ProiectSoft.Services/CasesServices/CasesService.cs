using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProiectSoft.BLL.Helpers;
using ProiectSoft.DAL;
using ProiectSoft.DAL.Entities;
using ProiectSoft.DAL.Models.CasesModels;
using ProiectSoft.DAL.Wrappers;
using ProiectSoft.Services.UriServicess;
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
        public CasesService(AppDbContext context, IUriServices uriServices, IMapper mapper)
        {
            _context = context;
            _uriServices = uriServices;
            _mapper = mapper;   
        }

        public async Task Create(CasesPostModel model)
        {
            if (model == null) { return; }       

            var _case = _mapper.Map<Cases>(model);

            await _context.AddAsync(_case);
            await _context.SaveChangesAsync();
            
        }
        public async Task Delete(int id)
        {
            var _case = await _context.Cases.FirstOrDefaultAsync(x => x.Id == id);
            
            if (_case == null)
            {
                return;
            }

            _context.Cases.Remove(_case);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<List<CasesGetModel>>> GetAll(PaginationFilter filter, string route)
        {
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

        public async Task<CasesGetModel> GetById(int id)
        {
            var _case = await _context.Cases.FirstOrDefaultAsync(x => x.Id == id);

            if (_case == null)
            {
                return new CasesGetModel();
            }

            var caseGetModel = _mapper.Map<CasesGetModel>(_case);

            return caseGetModel;
        }

        public async Task Update(CasesPutModel model, int id)
        {
            var _case = await _context.Cases.FirstOrDefaultAsync(x => x.Id == id);

            if (_case == null || model == null)
            {
                return;
            }

            _mapper.Map<CasesPutModel, Cases>(model, _case);

            await _context.SaveChangesAsync();
        }
    }
}
