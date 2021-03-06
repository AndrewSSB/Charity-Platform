using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProiectSoft.BLL.Helpers;
using ProiectSoft.DAL;
using ProiectSoft.DAL.Entities;
using ProiectSoft.DAL.Models.ShelterModels;
using ProiectSoft.DAL.Wrappers;
using ProiectSoft.DAL.Wrappers.Filters;
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
                throw new KeyNotFoundException($"There is no location with id: {model.LocationId}");
            }

            if (organisation == null) {
                _logger.LogError($"OPS! {model.OrganisationId} does not exist in our database");
                throw new KeyNotFoundException($"There is no organisation with id: {model.OrganisationId}");
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
                throw new KeyNotFoundException($"There is no shelter with id: {id}");
            }

            _context.Shelters.Remove(shelter);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<List<ShelterGetModel>>> GetAll(ShelterFilter filter, string route)
        {
            LogContext.PushProperty("IdentificationMessage", $"GetAll shelter request for Page:{filter.PageNumber}, with number of objects: {filter.PageSize}");

            IQueryable<Shelter> shelters = _context.Shelters;

            if (!string.IsNullOrEmpty(filter.searchName))
            {
                shelters = shelters.Where(x => x.Name!.Contains(filter.searchName));
            }

            if (!string.IsNullOrEmpty(filter.name))
            {
                shelters = shelters.Where(x => x.Name!.Contains(filter.name));
            }

            if (filter.availableSpace != null)
            {
                shelters = shelters.Where(x => x.availableSpace >= filter.availableSpace);
            }

            if (filter.dateCreated != null)
            {
                shelters = shelters.Where(x => x.DateCreated.Value.ToString("mm/dd/yyyy").Contains(filter.dateCreated.Value.ToString("mm/dd/yyyy")));
            }

            switch (filter.orderBy)
            {
                case "Name":
                    shelters = !filter.descending == false ? shelters.OrderBy(x => x.Name) : shelters.OrderByDescending(x => x.Name);
                    break;
                case "Date":
                    shelters = !filter.descending == false ? shelters.OrderBy(s => s.DateCreated) : shelters.OrderByDescending(x => x.DateCreated);
                    break;
                case "Space":
                    shelters = !filter.descending == false ? shelters.OrderBy(s => s.availableSpace) : shelters.OrderByDescending(x => x.availableSpace);
                    break;
                case "Email":
                    shelters = !filter.descending == false ? shelters.OrderBy(s => s.Email) : shelters.OrderByDescending(x => x.Email);
                    break;
                default:
                    shelters = !filter.descending == false ? shelters.OrderBy(s => s.Id) : shelters.OrderByDescending(x => x.Id);
                    break;
            }

            var shelterModels = shelters
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(_mapper.Map<ShelterGetModel>)
                .ToList();

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
                throw new KeyNotFoundException($"There is no shelter with id: {id}");
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
                throw new KeyNotFoundException($"There is no shelter with id: {id}");
            }

            var shelterLoc = await _context.Locations.FirstOrDefaultAsync(x => x.Id == model.LocationId);
            var shelterOrg = await _context.Organisations.FirstOrDefaultAsync(x => x.Id == model.OrganisationId);

            if (shelterLoc == null)
            {
                _logger.LogError($"There is no location with ID:{model.LocationId}. Update failed");
                throw new KeyNotFoundException($"There is no location with id: {model.LocationId}");
            }
            if (shelterOrg == null)
            {
                _logger.LogError($"There is no organisation with ID:{model.OrganisationId}. Update failed");
                throw new KeyNotFoundException($"There is no organisation with id: {model.OrganisationId}");
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

        private async Task<List<Shelter>> FilterLoc(List<Shelter> shelters, string[] filters)
        {
            if (filters.Length == 1)
            {
                shelters = shelters.Where(x => x.Name.Contains(filters[0])).ToList();
            }
            else if (filters.Length == 2)
            {
                shelters = shelters.Where(x => x.Name.Contains(filters[0]) || 
                x.availableSpace > Int32.Parse(filters[1])).ToList();
            }
            else if (filters.Length == 3) //aici pe front ar putea sa faca dupa data mm/dd/yyyy sau dupa mm sau dd sau yyyy
            {
                shelters = shelters.Where(x => x.Name.Contains(filters[0]) ||
                x.availableSpace > Int32.Parse(filters[1]) ||
                x.DateCreated.Value.ToString("mm/dd/yyyy").Contains(filters[3])).ToList();
            }

            return shelters.ToList();
        }
    }
}
