using Microsoft.EntityFrameworkCore;
using ProiectSoft.DAL;
using ProiectSoft.DAL.Entities;
using ProiectSoft.DAL.Models.VolunteerModels;
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

        public VolunteerServices(AppDbContext context)
        {
            _context = context;
        }

        public async Task Create(VolunteerPostModel model)
        {
            if (model == null) { return; }

            var orgId = _context.Organisations.FirstOrDefaultAsync(x => x.Id == model.OrganisationId);

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

        public async Task<List<VolunteerGetModel>> GetAll()
        {
            return await _context.Volunteers.Select(x => new VolunteerGetModel
            {
                Id = x.Id,
                Name = x.Name,
                lastName = x.lastName,
                Position = x.Position,
                contactDetails = x.contactDetails,
                OrganisationId = x.OrganisationId
            }).ToListAsync();
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
