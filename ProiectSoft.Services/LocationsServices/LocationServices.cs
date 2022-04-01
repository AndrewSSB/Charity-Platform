using Microsoft.EntityFrameworkCore;
using ProiectSoft.DAL;
using ProiectSoft.DAL.Entities;
using ProiectSoft.DAL.Models.LocationModels;
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

        public LocationServices(AppDbContext context)
        {
            _context = context;
        }

        public async Task Create(LocationPostModel model)
        {
            if (model != null)
            {
                var location = new Location
                {
                    County = model.County,
                    City = model.City,
                    Street = model.Street,
                    Number = model.Number,
                };

                await _context.AddAsync(location);
                await _context.SaveChangesAsync();
            }
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

        public async Task<List<LocationGetModel>> GetAll()
        {
            return await _context.Locations.Select(x => new LocationGetModel
            {
                Id = x.Id,
                County = x.County,
                City = x.City,
                Street = x.Street,
                Number= x.Number
            }).ToListAsync();
        }

        public async Task<LocationGetModel> GetById(int id)
        {
            var location = await _context.Locations.FirstOrDefaultAsync(x => x.Id == id);

            if (location == null)
            {
                return new LocationGetModel();
            }
            return new LocationGetModel
            {
                Id = location.Id,
                County = location.County,
                City=location.City,
                Street=location.Street,
                Number = location.Number
            };
        }

        public async Task Update(LocationPutModel model, int id)
        {
            var location = await _context.Locations.FirstOrDefaultAsync(x => x.Id == id);

            if (model == null || location == null) { return; }
            
            location.County = model.County;
            location.City = model.City;
            location.Street = model.Street;
            location.Number = model.Number;

            await _context.SaveChangesAsync();
        }
    }
}
