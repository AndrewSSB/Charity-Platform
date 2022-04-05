using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProiectSoft.BLL.Helpers;
using ProiectSoft.DAL;
using ProiectSoft.DAL.Entities;
using ProiectSoft.DAL.Models.VolunteerModels;
using ProiectSoft.DAL.Wrappers;
using ProiectSoft.Services.UriServicess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.Services.VolunteersServices
{
    public class VolunteerServices : IVolunteerServices
    {
        private readonly AppDbContext _context;
        private readonly IUriServices _uriServices;
        private readonly IMapper _mapper;

        public VolunteerServices(AppDbContext context, IUriServices uriServices, IMapper mapper)
        {
            _uriServices = uriServices;
            _context = context;
            _mapper = mapper;
        }

        public async Task Create(VolunteerPostModel model)
        {
            if (model == null) { return; }

            var orgId = await _context.Organisations.FirstOrDefaultAsync(x => x.Id == model.OrganisationId);

            if (orgId == null) { return; }

            var volunteer = _mapper.Map<Volunteer>(model);

            await _context.AddAsync(volunteer);
            await _context.SaveChangesAsync();
            
        }

        public async Task Delete(int id)
        {
            var volunteer = await _context.Volunteers.FirstOrDefaultAsync(x => x.Id == id);

            if (volunteer == null)
            {
                return;
            }

            _context.Volunteers.Remove(volunteer);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<List<VolunteerGetModel>>> GetAll(PaginationFilter filter, string route)
        {
            var volunteers = await _context.Volunteers
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var volunteerModels = new List<VolunteerGetModel>();

            foreach (var volunteer in volunteers)
            {
                var volunteerModel = _mapper.Map<VolunteerGetModel>(volunteer);
                volunteerModels.Add(volunteerModel);
            }

            var volunteerListCount = await _context.Cases.CountAsync();

            var pagedResponse = PaginationHelper.CreatePagedReponse<VolunteerGetModel>(volunteerModels, filter, volunteerListCount, _uriServices, route);

            return pagedResponse;
        }

        public async Task<VolunteerGetModel> GetById(int id)
        {
            var volunteer = await _context.Volunteers.FirstOrDefaultAsync(x => x.Id == id);

            if (volunteer == null)
            {
                return new VolunteerGetModel();
            }

            var volunteerGetModel = _mapper.Map<VolunteerGetModel>(volunteer);

            return volunteerGetModel;
        }

        public async Task Update(VolunteerPutModel model, int id)
        {
            var volunteer = await _context.Volunteers.FirstOrDefaultAsync(x => x.Id == id);

            if (model == null || volunteer == null) { return; }

            volunteer.Name = model.Name;
            volunteer.lastName = model.lastName;
            volunteer.Position = model.Position;
            volunteer.contactDetails = model.contactDetails;

            var volunteerOrg = await _context.Organisations.FirstOrDefaultAsync(x => x.Id == model.OrganisationId);

            if (volunteerOrg != null)
            {
                volunteer.OrganisationId = model.OrganisationId;
            }

            await _context.SaveChangesAsync();
        }
    }
}
