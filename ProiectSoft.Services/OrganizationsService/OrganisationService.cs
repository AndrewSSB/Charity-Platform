using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProiectSoft.BLL.Helpers;
using ProiectSoft.DAL;
using ProiectSoft.DAL.Entities;
using ProiectSoft.DAL.Models.OrganisationModels;
using ProiectSoft.DAL.Wrappers;
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
                return; 
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
                return;
            }

            _context.Organisations.Remove(organisation);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<List<OrganisationGetModel>>> GetAll(PaginationFilter filter, string route, string searchName, string orderBy, bool descending)
        {
            LogContext.PushProperty("IdentificationMessage", $"GetAll organisation request for Page:{filter.PageNumber}, with number of objects: {filter.PageSize}");

            var organisations = await _context.Organisations
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            if (!string.IsNullOrEmpty(searchName))
            {
                organisations = organisations.Where(x => x.Name!.Contains(searchName)).ToList();
            }

            organisations = await OrderBy(organisations, orderBy, descending);

            var organisationModels = new List<OrganisationGetModel>();

            foreach (var organisation in organisations)
            {
                var _caseModel = _mapper.Map<OrganisationGetModel>(organisation);
                organisationModels.Add(_caseModel);
            }

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
                return new Response<OrganisationGetModel>(false, $"Id {id} doesn't exist");
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
                return; 
            }
            
            var caseId = await _context.Cases.FirstOrDefaultAsync(x => x.Id == model.CasesId); 
            //ar trebui sa verific daca exista cheia pe care o modific, daca exista o schimb, daca nu ramane aceeasi

            if (caseId == null) 
            {
                _logger.LogError($"There is no case with ID:{model.CasesId}. Update failed");
                return;
            }

            _mapper.Map<OrganisationPutModel, Organisation>(model, organisation);

            await _context.SaveChangesAsync();
        }

        private async Task<List<Organisation>> OrderBy(List<Organisation> organisations, string orderBy, bool descending)
        {
            switch (orderBy)
            {
                case "Name":
                    organisations = descending == false ? organisations.OrderBy(x => x.Name).ToList() : organisations.OrderByDescending(x => x.Name).ToList();
                    break;
                case "Date":
                    organisations = descending == false ? organisations.OrderBy(s => s.CasesId).ToList() : organisations.OrderByDescending(x => x.CasesId).ToList();
                    break;
                case "Age":
                    organisations = descending == false ? organisations.OrderBy(s => s.DateCreated).ToList() : organisations.OrderByDescending(x => x.DateCreated).ToList();
                    break;
                default:
                    organisations = descending == false ? organisations.OrderBy(s => s.Id).ToList() : organisations.OrderByDescending(x => x.Id).ToList();
                    break;
            }

            return organisations;
        }
    }
}
