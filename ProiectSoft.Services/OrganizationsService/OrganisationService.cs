using Microsoft.EntityFrameworkCore;
using ProiectSoft.DAL;
using ProiectSoft.DAL.Entities;
using ProiectSoft.DAL.Models.OrganisationModels;
using ProiectSoft.Services.OrganizationsService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.Services.OrganizationService
{
    public class OrganisationService : IOrganisationService
    {
        private readonly AppDbContext _context;

        public OrganisationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task Create(OrganisationPostModel model)
        {
            if (model != null)
            {
                var organisation = new Organisation { 
                    Name = model.Name,
                    Email = model.Email,
                    Phone = model.Phone,
                    Details = model.Details,
                    CasesId = model.CasesId
                };

                await _context.AddAsync(organisation);
                await _context.SaveChangesAsync();
            }
        }

        public async Task Delete(int id)
        {
            var organisation = await _context.Organisations.FirstOrDefaultAsync(x => x.Id == id);

            if (organisation == null)
            {
                return;
            }

            _context.Organisations.Remove(organisation);
            await _context.SaveChangesAsync();
        }

        public async Task<List<OrganisationGetModel>> GetAll()
        {
            return await _context.Organisations.Select(x => new OrganisationGetModel
            {
                Id = x.Id,
                Name = x.Name,
                Email = x.Email,
                Phone = x.Phone,
                Details = x.Details,
                CasesId = x.CasesId
            }).ToListAsync();
        }

        public async Task<OrganisationGetModel> GetById(int id)
        {
            var organisation = await _context.Organisations.FirstOrDefaultAsync(x => x.Id == id);

            if (organisation == null)
            {
                return new OrganisationGetModel();
            }
            return new OrganisationGetModel
            {
                Id = organisation.Id,
                Name = organisation.Name,
                Email = organisation.Email,
                Phone = organisation.Phone,
                Details = organisation.Details,
                CasesId = organisation.CasesId
            };
        }

        public async Task Update(OrganisationPutModel model, int id)
        {
            var organisation = _context.Organisations.FirstOrDefault(x => x.Id == id);

            if (model == null || organisation == null) { return; }
            
            organisation.Name = model.Name;
            organisation.Email = model.Email;
            organisation.Phone = model.Phone;
            organisation.Details = model.Details;
            organisation.CasesId = model.CasesId;

            await _context.SaveChangesAsync();

        }
    }
}
