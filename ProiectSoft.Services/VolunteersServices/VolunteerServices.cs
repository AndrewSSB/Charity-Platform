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

        public VolunteerServices(AppDbContext context, IUriServices uriServices)
        {
            _uriServices = uriServices;
            _context = context;
        }

        public async Task Create(VolunteerPostModel model)
        {
            if (model == null) { return; }

            var orgId = await _context.Organisations.FirstOrDefaultAsync(x => x.Id == model.OrganisationId);

            if (orgId == null) { return; }

            var volunteer = new Volunteer
            {
                Name = model.Name,
                lastName = model.lastName,
                Position = model.Position,
                contactDetails = model.contactDetails,
                OrganisationId = model.OrganisationId
            };

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
            var volunteers = await _context.Volunteers.Select(x => new VolunteerGetModel
            {
                Id = x.Id,
                Name = x.Name,
                lastName = x.lastName,
                Position = x.Position,
                contactDetails = x.contactDetails,
                OrganisationId = x.OrganisationId
            })
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var volunteerListCount = await _context.Cases.CountAsync();

            var pagedResponse = PaginationHelper.CreatePagedReponse<VolunteerGetModel>(volunteers, filter, volunteerListCount, _uriServices, route);

            return pagedResponse;
        }

        public async Task<VolunteerGetModel> GetById(int id)
        {
            var volunteer = await _context.Volunteers.FirstOrDefaultAsync(x => x.Id == id);

            if (volunteer == null)
            {
                return new VolunteerGetModel();
            }
            return new VolunteerGetModel
            {
                Id = volunteer.Id,
                Name = volunteer.Name,
                lastName = volunteer.lastName,
                Position = volunteer.Position,
                contactDetails = volunteer.contactDetails,
                OrganisationId = volunteer.OrganisationId
            };
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
