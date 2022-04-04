using Microsoft.EntityFrameworkCore;
using ProiectSoft.BLL.Helpers;
using ProiectSoft.DAL;
using ProiectSoft.DAL.Entities;
using ProiectSoft.DAL.Models.ShelterModels;
using ProiectSoft.DAL.Wrappers;
using ProiectSoft.Services.UriServicess;
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

        public ShelterServices(AppDbContext context, IUriServices uriServices)
        {
            _uriServices = uriServices;
            _context = context;
        }

        public async Task Create(ShelterPostModel model)
        {
            if (model == null) { return; }

            var location = await _context.Locations.FirstOrDefaultAsync(x => x.Id == model.LocationId);
            var organisation = await _context.Organisations.FirstOrDefaultAsync(x => x.Id == model.OrganisationId);

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

        public async Task<PagedResponse<List<ShelterGetModel>>> GetAll(PaginationFilter filter, string route)
        {
            var shelters = await _context.Shelters.Select(x => new ShelterGetModel
            {
                Id = x.Id,
                Name = x.Name,
                availableSpace = x.availableSpace,
                Email = x.Email,
                Phone = x.Phone,
                LocationId = x.LocationId,
                OrganisationId = x.OrganisationId
            })
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var shelterListCount = await _context.Cases.CountAsync();

            var pagedResponse = PaginationHelper.CreatePagedReponse<ShelterGetModel>(shelters, filter, shelterListCount, _uriServices, route);

            return pagedResponse;
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
