using AutoMapper;
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
        private readonly IMapper _mapper;

        public OrganisationService(AppDbContext context, IUriServices uriServices, IMapper mapper)
        {
            _context = context;
            _uriServices = uriServices;
            _mapper = mapper;
        }

        public async Task Create(OrganisationPostModel model)
        {
            if (model == null) { return; }
            
            var caseId = await _context.Cases.FirstOrDefaultAsync(x => x.Id == model.CasesId);

            if (caseId == null) { return; }

            var organisation = _mapper.Map<Organisation>(model);

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
            var organisations = await _context.Organisations
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var organisationModels = new List<OrganisationGetModel>();

            foreach (var organisation in organisations)
            {
                var _caseModel = _mapper.Map<OrganisationGetModel>(organisation);
                organisationModels.Add(_caseModel);
            }

            var orgListCount = await _context.Cases.CountAsync();

            var pagedResponse = PaginationHelper.CreatePagedReponse<OrganisationGetModel>(organisationModels, filter, orgListCount, _uriServices, route);

            return pagedResponse;
        }

        public async Task<OrganisationGetModel> GetById(int id)
        {
            var organisation = await _context.Organisations.FirstOrDefaultAsync(x => x.Id == id);

            if (organisation == null)
            {
                return new OrganisationGetModel();
            }

            var organisationGetModel = _mapper.Map<OrganisationGetModel>(organisation);

            return organisationGetModel;
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
