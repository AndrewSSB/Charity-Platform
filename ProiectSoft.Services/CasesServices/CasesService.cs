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
        public CasesService(AppDbContext context, IUriServices uriServices)
        {
            _context = context;
            _uriServices = uriServices;
        }

        public async Task Create(CasesPostModel model)
        {
            if (model == null) { return; }
           
            var _case = new Cases
            {   
                caseName = model.caseName,
                caseDetails = model.caseDetails,
                startDate = model.startDate,
                endDate = model.endDate,
                closed = model.closed,
                DateCreated = model.created,
                DateModified = model.modified
            };

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
            var casesList = await _context.Cases.Select(x => new CasesGetModel
            {
                Id = x.Id.ToString(),
                caseName = x.caseName,
                caseDetails = x.caseDetails,
                startDate = x.startDate.ToString(),
                endDate = x.endDate.ToString(),
                closed = x.closed,
                Created = x.DateCreated.ToString(),
                Modified = x.DateModified.ToString(),
            })
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var casesListCount = await _context.Cases.CountAsync();

            var pagedResponse = PaginationHelper.CreatePagedReponse<CasesGetModel>(casesList, filter, casesListCount, _uriServices, route);

            return pagedResponse;
        }

        public async Task<CasesGetModel> GetById(int id)
        {
            var _case = await _context.Cases.FirstOrDefaultAsync(x => x.Id == id);

            if (_case == null)
            {
                return new CasesGetModel();
            }

            var casesGetModel = new CasesGetModel
            {
                Id = _case.Id.ToString(),
                caseName = _case.caseName,
                caseDetails = _case.caseDetails,
                startDate = _case.startDate.ToString(),
                endDate = _case.endDate.ToString(),
                closed = _case.closed,
                Created = _case.DateCreated.ToString(),
                Modified = _case.DateModified.ToString()
            };

            return casesGetModel;
        }

        public async Task Update(CasesPutModel model, int id)
        {
            var _case = await _context.Cases.FirstOrDefaultAsync(x => x.Id == id);

            if (_case == null || model == null)
            {
                return;
            }

            _case.caseName = model.caseName;
            _case.caseDetails = model.caseDetails;
            _case.startDate = model.startDate;
            _case.endDate = model.endDate;
            _case.closed = model.closed;
            _case.DateModified = DateTime.Now; //se salveaza automat cand ai facut modificarea

            await _context.SaveChangesAsync();
        }
    }
}
