using Microsoft.EntityFrameworkCore;
using ProiectSoft.BLL.Helpers;
using ProiectSoft.DAL;
using ProiectSoft.DAL.Entities;
using ProiectSoft.DAL.Models.OrganisationModels;
using ProiectSoft.DAL.Wrappers;
using ProiectSoft.Services.OrganizationsService;
using ProiectSoft.Services.UriServicess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.Services.OrganizationService
{
    public class OrganisationService : IOrganisationService
    {
        private readonly AppDbContext _context;
        private readonly IUriServices _uriServices;

        public OrganisationService(AppDbContext context, IUriServices uriServices)
        {
            _context = context;
            _uriServices = uriServices;
        }

        public async Task Create(OrganisationPostModel model)
        {
            if (model == null) { return; }
            
            var caseId = await _context.Cases.FirstOrDefaultAsync(x => x.Id == model.CasesId);

            if (caseId == null) { return; }

            var organisation = new Organisation
            {
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                Details = model.Details,
                CasesId = model.CasesId
            };

            await _context.AddAsync(organisation);
            await _context.SaveChangesAsync();
            
        }

        public async Task Delete(int id)
        {
            var organisation = await _context.Organisations.FirstOrDefaultAsync(x => x.Id == id);

            if (organisation == null)
            {
                return;
            }

            _context.Organisations.Remove(organisation);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<List<OrganisationGetModel>>> GetAll(PaginationFilter filter, string route)
        {
            var organisations = await _context.Organisations.Select(x => new OrganisationGetModel
            {
                Id = x.Id,
                Name = x.Name,
                Email = x.Email,
                Phone = x.Phone,
                Details = x.Details,
                CasesId = x.CasesId
            })
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var orgListCount = await _context.Cases.CountAsync();

            var pagedResponse = PaginationHelper.CreatePagedReponse<OrganisationGetModel>(organisations, filter, orgListCount, _uriServices, route);

            return pagedResponse;
        }

        public async Task<OrganisationGetModel> GetById(int id)
        {
            var organisation = await _context.Organisations.FirstOrDefaultAsync(x => x.Id == id);

            if (organisation == null)
            {
                return new OrganisationGetModel();
            }
            return new OrganisationGetModel
            {
                Id = organisation.Id,
                Name = organisation.Name,
                Email = organisation.Email,
                Phone = organisation.Phone,
                Details = organisation.Details,
                CasesId = organisation.CasesId
            };
        }

        public async Task Update(OrganisationPutModel model, int id)
        {
            var organisation = await _context.Organisations.FirstOrDefaultAsync(x => x.Id == id);

            if (model == null || organisation == null) { return; }
            
            organisation.Name = model.Name;
            organisation.Email = model.Email;
            organisation.Phone = model.Phone;
            organisation.Details = model.Details;

            var caseId = await _context.Cases.FirstOrDefaultAsync(x => x.Id == model.CasesId); //ar trebui sa verific daca exista cheia pe care o modific, daca exista o schimb, daca nu ramane aceeasi
            //sau cu select _context.Cases.Select(x => x.Id).FirstOrDefaultAsync(x => x.Id == model.CaseId);
            
            //si acum daca dau un guid invalid ar trebui sa ramana acelasi care era inainte
            if (caseId != null) { organisation.CasesId = model.CasesId; }

            await _context.SaveChangesAsync();

        }
    }
}
