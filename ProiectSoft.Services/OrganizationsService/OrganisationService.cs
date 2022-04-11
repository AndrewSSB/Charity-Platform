using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProiectSoft.BLL.Helpers;
using ProiectSoft.DAL;
using ProiectSoft.DAL.Entities;
using ProiectSoft.DAL.Models.LocationModels;
using ProiectSoft.DAL.Models.OrganisationModels;
using ProiectSoft.DAL.Models.RefugeeModels;
using ProiectSoft.DAL.Models.ShelterModels;
using ProiectSoft.DAL.Wrappers;
using ProiectSoft.DAL.Wrappers.Filters;
using ProiectSoft.Services.OrganizationsService;
using ProiectSoft.Services.UriServicess;
using Serilog.Context;
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
        private readonly ILogger<OrganisationService> _logger;

        public OrganisationService(AppDbContext context, IUriServices uriServices, IMapper mapper, ILogger<OrganisationService> logger)
        {
            _context = context;
            _uriServices = uriServices;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task Create(OrganisationPostModel model)
        {
            LogContext.PushProperty("IdentificationMessage", $"Update organisation request for {model.Name}");

            var caseId = await _context.Cases.FirstOrDefaultAsync(x => x.Id == model.CasesId);

            if (caseId == null) 
            {
                _logger.LogError($"OPS! {model.Name} does not exist in our database");
                throw new KeyNotFoundException($"There is no case with id: {model.CasesId}"); 
            }

            var organisation = _mapper.Map<Organisation>(model);

            await _context.AddAsync(organisation);
            await _context.SaveChangesAsync();
            
        }

        public async Task Delete(int id)
        {
            LogContext.PushProperty("IdentificationMessage", $"Delete organisation request for {id}");

            var organisation = await _context.Organisations.FirstOrDefaultAsync(x => x.Id == id);

            if (organisation == null)
            {
                _logger.LogError($"There is no organisation with ID:{id} in our database");
                throw new KeyNotFoundException($"There is no organisation with id: {id}");
            }

            _context.Organisations.Remove(organisation);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<List<OrganisationGetModel>>> GetAll(OrganisationFilter filter, string route)
        {
            LogContext.PushProperty("IdentificationMessage", $"GetAll organisation request for Page:{filter.PageNumber}, with number of objects: {filter.PageSize}");

            IQueryable<Organisation> organisations = _context.Organisations;

            if (!string.IsNullOrEmpty(filter.searchName))
            {
                organisations = organisations.Where(x => x.Name!.Contains(filter.searchName));
            }

            if (!string.IsNullOrEmpty(filter.Name))
            {
                organisations = organisations.Where(x => x.Name!.Contains(filter.Name));
            }

            if (!string.IsNullOrEmpty(filter.email))
            {
                organisations = organisations.Where(x => x.Email!.Contains(filter.email));
            }

            if (filter.DateCreated != null)
            {
                organisations = organisations.Where(x => x.DateCreated.Value.ToString("mm/dd/yyyy").Contains(filter.DateCreated.Value.ToString("mm/dd/yyyy")));
            }

            switch (filter.orderBy)
            {
                case "Name":
                    organisations = !filter.descending ? organisations.OrderBy(x => x.Name) : organisations.OrderByDescending(x => x.Name);
                    break;
                case "Date":
                    organisations = !filter.descending ? organisations.OrderBy(s => s.CasesId) : organisations.OrderByDescending(x => x.CasesId);
                    break;
                case "Age":
                    organisations = !filter.descending ? organisations.OrderBy(s => s.DateCreated) : organisations.OrderByDescending(x => x.DateCreated);
                    break;
                default:
                    organisations = !filter.descending ? organisations.OrderBy(s => s.Id) : organisations.OrderByDescending(x => x.Id);
                    break;
            }


            var organisationModels = organisations
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(_mapper.Map<OrganisationGetModel>)
                .ToList();

            var orgListCount = await _context.Organisations.CountAsync();

            var pagedResponse = PaginationHelper.CreatePagedReponse<OrganisationGetModel>(organisationModels, filter, orgListCount, _uriServices, route);

            return pagedResponse;
        }

        public async Task<Response<OrganisationGetModel>> GetById(int id)
        {
            LogContext.PushProperty("IdentificationMessage", $"GetById organisation request for {id}");

            var organisation = await _context.Organisations.FirstOrDefaultAsync(x => x.Id == id);

            if (organisation == null)
            {
                _logger.LogError($"OPS! Organisation with id:{id} does not exist in our database");
                throw new KeyNotFoundException($"There is no organisation with id: {id}");
            }

            var organisationGetModel = _mapper.Map<OrganisationGetModel>(organisation);

            return new Response<OrganisationGetModel>(organisationGetModel);
        }

        public async Task Update(OrganisationPutModel model, int id)
        {
            LogContext.PushProperty("IdentificationMessage", $"Update organisation request for {model.Name}");

            var organisation = await _context.Organisations.FirstOrDefaultAsync(x => x.Id == id);

            if (organisation == null) {
                _logger.LogError($"There is no organisation with ID:{id} in our database");
                throw new KeyNotFoundException($"There is no organisation with id: {id}");
            }
            
            var caseId = await _context.Cases.FirstOrDefaultAsync(x => x.Id == model.CasesId); 

            if (caseId == null) 
            {
                _logger.LogError($"There is no case with ID:{model.CasesId}. Update failed");
                throw new KeyNotFoundException($"There is no case with id: {id}");
            }

            _mapper.Map<OrganisationPutModel, Organisation>(model, organisation);

            await _context.SaveChangesAsync();
        }

        public async Task<List<OrgAvailableSpace>> GetSheltersWithAvailableSpace()
        {
            var org = _context.Organisations
                .Include(s => s.Shelters.Where(x => x.availableSpace > 0))
                .Select(_mapper.Map<OrgAvailableSpace>)
                .ToList();

            return org;
        }
    }
}
