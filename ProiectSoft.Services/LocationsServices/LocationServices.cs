using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ProiectSoft.BLL.Helpers;
using ProiectSoft.DAL;
using ProiectSoft.DAL.Entities;
using ProiectSoft.DAL.Models.LocationModels;
using ProiectSoft.DAL.Wrappers;
using ProiectSoft.Services.UriServicess;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.Services.LocationsServices
{
    public class LocationServices : ILocationServices
    {
        private readonly AppDbContext _context;
        private readonly IUriServices _uriServices;
        private readonly IMapper _mapper;
        private readonly ILogger<LocationServices> _logger;

        public LocationServices(AppDbContext context, IUriServices uriServices, IMapper mapper, ILogger<LocationServices> logger)
        {
            _context = context;
            _uriServices = uriServices;
            _mapper = mapper;   
            _logger = logger;   
        }

        public async Task Create(LocationPostModel model)
        {
            LogContext.PushProperty("IdentificationMessage", $"Create location request for {model.City}");

            var location = _mapper.Map<Location>(model);

            await _context.AddAsync(location);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            LogContext.PushProperty("IdentificationMessage", $"Delete location request for {id}");

            var location = await _context.Locations.FirstOrDefaultAsync(x => x.Id == id);

            if (location == null)
            {
                _logger.LogError($"OPS! Location with id:{id} does not exist in our database");
                return;
            }

            _context.Locations.Remove(location);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<List<LocationGetModel>>> GetAll(PaginationFilter filter, string route, string searchCity, string orderBy, bool descending)
        {
            LogContext.PushProperty("IdentificationMessage", $"GetAll locations request for Page:{filter.PageNumber}, with number of objects: {filter.PageSize}");

            var locations = await _context.Locations
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            if (!string.IsNullOrEmpty(searchCity))
            {
                locations = locations.Where(x => x.City!.Contains(searchCity)).ToList();
            }

            locations = await OrderBy(locations, orderBy, descending);

            var locationModels = new List<LocationGetModel>();

            foreach (var location in locations)
            {
                var locationModel = _mapper.Map<LocationGetModel>(location);
                locationModels.Add(locationModel);
            }

            var locationsListCount = await _context.Locations.CountAsync();

            var pagedResponse = PaginationHelper.CreatePagedReponse<LocationGetModel>(locationModels, filter, locationsListCount, _uriServices, route);

            return pagedResponse;
        }

        public async Task<Response<LocationGetModel>> GetById(int id)
        {
            LogContext.PushProperty("IdentificationMessage", $"GetById location request for {id}");

            var location = await _context.Locations.FirstOrDefaultAsync(x => x.Id == id);

            if (location == null)
            {
                _logger.LogError($"OPS! location with id:{id} does not exist in our database");
                return new Response<LocationGetModel>(false, $"Id {id} doesn't exist");
            }

            var locationGetModel = _mapper.Map<LocationGetModel>(location);

            return new Response<LocationGetModel>(locationGetModel);
        }

        public async Task Update(LocationPutModel model, int id)
        {
            LogContext.PushProperty("IdentificationMessage", $"Update location request for {JsonConvert.SerializeObject(model)}");

            var location = await _context.Locations.FirstOrDefaultAsync(x => x.Id == id);

            if (location == null) {
                _logger.LogError($"There is no location with ID:{id} in our database");
                return; 
            }       

            _mapper.Map<LocationPutModel, Location>(model, location);

            await _context.SaveChangesAsync();
        }

        private async Task<List<Location>> OrderBy(List<Location> locations, string orderBy, bool descending)
        {
            switch (orderBy)
            {
                case "County":
                    locations = descending == false ? locations.OrderBy(x => x.County).ToList() : locations.OrderByDescending(x => x.County).ToList();
                    break;
                case "City":
                    locations = descending == false ? locations.OrderBy(s => s.City).ToList() : locations.OrderByDescending(x => x.City).ToList();
                    break;
                case "Street":
                    locations = descending == false ? locations.OrderBy(s => s.Street).ToList() : locations.OrderByDescending(x => x.Street).ToList();
                    break;
                default:
                    locations = descending == false ? locations.OrderBy(s => s.Id).ToList() : locations.OrderByDescending(x => x.Id).ToList();
                    break;
            }

            return locations;
        }
    }
}
