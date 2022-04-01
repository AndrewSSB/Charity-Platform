using Microsoft.EntityFrameworkCore;
using ProiectSoft.DAL;
using ProiectSoft.DAL.Entities;
using ProiectSoft.DAL.Models.CasesModels;
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

        public CasesService(AppDbContext context)
        {
            _context = context;
        }

        public async Task Create(CasesPostModel model)
        {
            if (model != null)
            {
                var _case = new Cases
                {
                    Id = model.Id == null ? Guid.NewGuid() : new Guid(model.Id), //merge sa si introduci tu un guid da e mai naspa
                    caseName = model.caseName,
                    caseDetails = model.caseDetails,
                    startDate = model.startDate,
                    endDate = model.endDate,
                    closed = model.closed,
                    DateCreated = model.created == null ? DateTime.Now : model.created
                };

                await _context.AddAsync(_case);
                await _context.SaveChangesAsync();
            }
        }
        public async Task Delete(Guid id)
        {
            var _case = await _context.Cases.FirstOrDefaultAsync(x => x.Id == id);
            
            if (_case == null)
            {
                return;
            }

            _context.Cases.Remove(_case);
            await _context.SaveChangesAsync();
        }

        public async Task<List<CasesGetModel>> GetAll()
        {
            return await _context.Cases.Select(x => new CasesGetModel
            {
                Id = x.Id.ToString(),
                caseName = x.caseName,
                caseDetails = x.caseDetails,
                startDate = x.startDate.ToString(),
                endDate = x.endDate.ToString(),
                closed = x.closed,
                Created = x.DateCreated.ToString(),
                Modified = x.DateModified.ToString(),
            }).ToListAsync();
        }

        public async Task<CasesGetModel> GetById(Guid id)
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

        public async Task Update(CasesPutModel model, Guid id)
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

            var x = _case;

            await _context.SaveChangesAsync();
        }
    }
}
