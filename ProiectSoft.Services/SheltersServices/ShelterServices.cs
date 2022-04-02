using Microsoft.EntityFrameworkCore;
using ProiectSoft.DAL;
using ProiectSoft.DAL.Entities;
using ProiectSoft.DAL.Models.ShelterModels;
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

        public ShelterServices(AppDbContext context)
        {
            _context = context;
        }

        public async Task Create(ShelterPostModel model)
        {
            if (model == null) { return; }

            var location = _context.Locations.FirstOrDefaultAsync(x => x.Id == model.LocationId);
            var organisation = _context.Organisations.FirstOrDefaultAsync(x => x.Id == model.OrganisationId);

            if (location == null) { return; }

            if (organisation == null) { return; }
            
            var shelter = new Shelter
            {
                Name = model.Name,
                availableSpace = model.availableSpace,
                Email = model.Email,
                Phone = model.Phone,
                LocationId = model.LocationId,
                OrganisationId = model.OrganisationId
            };

            await _context.AddAsync(shelter);
            await _context.SaveChangesAsync();
            
        }

        public async Task Delete(int id)
        {
            var shelter = await _context.Shelters.FirstOrDefaultAsync(x => x.Id == id);

            if (shelter == null)
            {
                return;
            }

            _context.Shelters.Remove(shelter);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ShelterGetModel>> GetAll()
        {
            return await _context.Shelters.Select(x => new ShelterGetModel
            {
                Id = x.Id,
                Name = x.Name,
                availableSpace = x.availableSpace,
                Email = x.Email,
                Phone = x.Phone,
                LocationId = x.LocationId,
                OrganisationId = x.OrganisationId
            }).ToListAsync();
        }

        public async Task<ShelterGetModel> GetById(int id)
        {
            var shelter = await _context.Shelters.FirstOrDefaultAsync(x => x.Id == id);

            if (shelter == null)
            {
                return new ShelterGetModel();
            }
            return new ShelterGetModel
            {
                Id = shelter.Id,
                Name = shelter.Name,
                availableSpace = shelter.availableSpace,
                Email = shelter.Email,
                Phone = shelter.Phone,
                LocationId = shelter.LocationId,
                OrganisationId = shelter.OrganisationId
            };
        }

        public async Task Update(ShelterPutModel model, int id)
        {
            var shelter = await _context.Shelters.FirstOrDefaultAsync(x => x.Id == id);

            if (model == null || shelter == null) { return; }

            shelter.Name = model.Name;
            shelter.availableSpace = model.availableSpace;
            shelter.Email = model.Email;
            shelter.Phone = model.Phone;

            var shelterLoc = await _context.Locations.FirstOrDefaultAsync(x => x.Id == model.LocationId);
            var shelterOrg = await _context.Organisations.FirstOrDefaultAsync(x => x.Id == model.OrganisationId);

            if (shelterLoc != null)
            {
                shelter.LocationId = model.LocationId;
            }
            if (shelterOrg != null)
            {
                shelter.OrganisationId = model.OrganisationId;
            }

            await _context.SaveChangesAsync();
        }
    }
}
