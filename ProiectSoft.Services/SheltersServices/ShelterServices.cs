using AutoMapper;
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
        private readonly IMapper _mapper;
        public ShelterServices(AppDbContext context, IUriServices uriServices, IMapper mapper)
        {
            _uriServices = uriServices;
            _context = context;
            _mapper = mapper;
        }

        public async Task Create(ShelterPostModel model)
        {
            if (model == null) { return; }

            var location = await _context.Locations.FirstOrDefaultAsync(x => x.Id == model.LocationId);
            var organisation = await _context.Organisations.FirstOrDefaultAsync(x => x.Id == model.OrganisationId);

            if (location == null) { return; }

            if (organisation == null) { return; }
        
            var shelter = _mapper.Map<Shelter>(model);

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
            var shelters = await _context.Shelters
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var shelterModels = new List<ShelterGetModel>();

            foreach (var shetler in shelters)
            {
                var shelterModel = _mapper.Map<ShelterGetModel>(shetler);
                shelterModels.Add(shelterModel);
            }

            var shelterListCount = await _context.Cases.CountAsync();

            var pagedResponse = PaginationHelper.CreatePagedReponse<ShelterGetModel>(shelterModels, filter, shelterListCount, _uriServices, route);

            return pagedResponse;
        }

        public async Task<ShelterGetModel> GetById(int id)
        {
            var shelter = await _context.Shelters.FirstOrDefaultAsync(x => x.Id == id);

            if (shelter == null)
            {
                return new ShelterGetModel();
            }

            var shelterGetModel = _mapper.Map<ShelterGetModel>(shelter);

            return shelterGetModel;
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
