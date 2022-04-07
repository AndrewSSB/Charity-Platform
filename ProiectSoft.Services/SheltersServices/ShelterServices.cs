using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProiectSoft.BLL.Helpers;
using ProiectSoft.DAL;
using ProiectSoft.DAL.Entities;
using ProiectSoft.DAL.Models.ShelterModels;
using ProiectSoft.DAL.Wrappers;
using ProiectSoft.Services.UriServicess;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.Services.SheltersServices
{
    public class ShelterServices : IShelterServices
    {
        private readonly AppDbContext _context;
        private readonly IUriServices _uriServices;
        private readonly IMapper _mapper;
        private readonly ILogger<ShelterServices> _logger;
        public ShelterServices(AppDbContext context, IUriServices uriServices, IMapper mapper, ILogger<ShelterServices> logger)
        {
            _uriServices = uriServices;
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task Create(ShelterPostModel model)
        {
            LogContext.PushProperty("IdentificationMessage", $"Create shelter request for {model.Name}");

            var location = await _context.Locations.FirstOrDefaultAsync(x => x.Id == model.LocationId);
            var organisation = await _context.Organisations.FirstOrDefaultAsync(x => x.Id == model.OrganisationId);

            if (location == null) {
                _logger.LogError($"OPS! {model.LocationId} does not exist in our database");
                return; 
            }

            if (organisation == null) {
                _logger.LogError($"OPS! {model.OrganisationId} does not exist in our database");
                return; 
            }
        
            var shelter = _mapper.Map<Shelter>(model);

            await _context.AddAsync(shelter);
            await _context.SaveChangesAsync();
            
        }

        public async Task Delete(int id)
        {
            LogContext.PushProperty("IdentificationMessage", $"Delete shelter request for {id}");

            var shelter = await _context.Shelters.FirstOrDefaultAsync(x => x.Id == id);

            if (shelter == null)
            {
                _logger.LogError($"OPS! Shelter with id:{id} does not exist in our database");
                return;
            }

            _context.Shelters.Remove(shelter);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<List<ShelterGetModel>>> GetAll(PaginationFilter filter, string route, string searchName, string orderBy, bool descending)
        {
            LogContext.PushProperty("IdentificationMessage", $"GetAll shelter request for Page:{filter.PageNumber}, with number of objects: {filter.PageSize}");

            var shelters = await _context.Shelters
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            if (string.IsNullOrEmpty(searchName))
            {
                shelters = shelters.Where(x => x.Name!.Contains(searchName)).ToList();
            }

            shelters = await OrderBy(shelters, orderBy, descending);

            var shelterModels = new List<ShelterGetModel>();

            foreach (var shetler in shelters)
            {
                var shelterModel = _mapper.Map<ShelterGetModel>(shetler);
                shelterModels.Add(shelterModel);
            }

            var shelterListCount = await _context.Shelters.CountAsync();

            var pagedResponse = PaginationHelper.CreatePagedReponse<ShelterGetModel>(shelterModels, filter, shelterListCount, _uriServices, route);

            return pagedResponse;
        }

        public async Task<Response<ShelterGetModel>> GetById(int id)
        {
            LogContext.PushProperty("IdentificationMessage", $"GetById shelter request for {id}");

            var shelter = await _context.Shelters.FirstOrDefaultAsync(x => x.Id == id);

            if (shelter == null)
            {
                _logger.LogError($"OPS! Shelter with id:{id} does not exist in our database");
                return new Response<ShelterGetModel>(false, $"Id {id} doesn't exist");
            }

            var shelterGetModel = _mapper.Map<ShelterGetModel>(shelter);

            return new Response<ShelterGetModel>(shelterGetModel);
        }

        public async Task Update(ShelterPutModel model, int id)
        {
            LogContext.PushProperty("IdentificationMessage", $"Update shelter request for {model.Name}");

            var shelter = await _context.Shelters.FirstOrDefaultAsync(x => x.Id == id);

            if (shelter == null) {
                _logger.LogError($"There is no shelter with ID:{id} in our database");
                return; 
            }

            var shelterLoc = await _context.Locations.FirstOrDefaultAsync(x => x.Id == model.LocationId);
            var shelterOrg = await _context.Organisations.FirstOrDefaultAsync(x => x.Id == model.OrganisationId);

            if (shelterLoc == null)
            {
                _logger.LogError($"There is no location with ID:{model.LocationId}. Update failed");
                return;
            }
            if (shelterOrg == null)
            {
                _logger.LogError($"There is no organisation with ID:{model.OrganisationId}. Update failed");
                return;
            }

            _mapper.Map<ShelterPutModel, Shelter>(model, shelter);

            await _context.SaveChangesAsync();
        }

        private async Task<List<Shelter>> OrderBy(List<Shelter> shelters, string orderBy, bool descending)
        {
            switch (orderBy)
            {
                case "Name":
                    shelters = descending == false ? shelters.OrderBy(x => x.Name).ToList() : shelters.OrderByDescending(x => x.Name).ToList();
                    break;
                case "Date":
                    shelters = descending == false ? shelters.OrderBy(s => s.DateCreated).ToList() : shelters.OrderByDescending(x => x.DateCreated).ToList();
                    break;
                case "Space":
                    shelters = descending == false ? shelters.OrderBy(s => s.availableSpace).ToList() : shelters.OrderByDescending(x => x.availableSpace).ToList();
                    break;
                case "Email":
                    shelters = descending == false ? shelters.OrderBy(s => s.Email).ToList() : shelters.OrderByDescending(x => x.Email).ToList();
                    break;
                default:
                    shelters = descending == false ? shelters.OrderBy(s => s.Id).ToList() : shelters.OrderByDescending(x => x.Id).ToList();
                    break;
            }

            return shelters;
        }
    }
}
