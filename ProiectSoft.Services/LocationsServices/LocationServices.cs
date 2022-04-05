using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProiectSoft.BLL.Helpers;
using ProiectSoft.DAL;
using ProiectSoft.DAL.Entities;
using ProiectSoft.DAL.Models.LocationModels;
using ProiectSoft.DAL.Wrappers;
using ProiectSoft.Services.UriServicess;
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

        public LocationServices(AppDbContext context, IUriServices uriServices, IMapper mapper)
        {
            _context = context;
            _uriServices = uriServices;
            _mapper = mapper;   
        }

        public async Task Create(LocationPostModel model)
        {
            if (model == null) { return; }

            var location = _mapper.Map<Location>(model);

            await _context.AddAsync(location);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var location = await _context.Locations.FirstOrDefaultAsync(x => x.Id == id);

            if (location == null)
            {
                return;
            }

            _context.Locations.Remove(location);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<List<LocationGetModel>>> GetAll(PaginationFilter filter, string route)
        {
            var locations = await _context.Locations
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var locationModels = new List<LocationGetModel>();

            foreach (var location in locations)
            {
                var locationModel = _mapper.Map<LocationGetModel>(location);
                locationModels.Add(locationModel);
            }

            var locationsListCount = await _context.Cases.CountAsync();

            var pagedResponse = PaginationHelper.CreatePagedReponse<LocationGetModel>(locationModels, filter, locationsListCount, _uriServices, route);

            return pagedResponse;
        }

        public async Task<LocationGetModel> GetById(int id)
        {
            var location = await _context.Locations.FirstOrDefaultAsync(x => x.Id == id);

            if (location == null)
            {
                return new LocationGetModel();
            }

            var locationGetModel = _mapper.Map<LocationGetModel>(location);

            return locationGetModel;
        }

        public async Task Update(LocationPutModel model, int id)
        {
            var location = await _context.Locations.FirstOrDefaultAsync(x => x.Id == id);

            if (model == null || location == null) { return; }       

            _mapper.Map<LocationPutModel, Location>(model, location);

            await _context.SaveChangesAsync();
        }
    }
}
