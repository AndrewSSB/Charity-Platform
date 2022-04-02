using Microsoft.EntityFrameworkCore;
using ProiectSoft.DAL;
using ProiectSoft.DAL.Entities;
using ProiectSoft.DAL.Models.DonationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.Services.DonationsServices
{
    public class DonationServices : IDonationServices
    {
        private readonly AppDbContext _context;

        public DonationServices(AppDbContext context)
        {
            _context = context;
        }

        public async Task Create(DonationPostModel model)
        {
            if (model == null) { return; }

            var user = _context.Users.FirstOrDefaultAsync(x => x.Id == model.UserId);
            var organisation = _context.Organisations.FirstOrDefaultAsync(x => x.Id == model.OrganisationId); //daca nu exista userID-ul, sau organisationID-ul renunt

            if (user == null) { return; }

            if (organisation == null) { return; }
            
            var location = new Donation
            {
                donation = model.donation,
                UserId = model.UserId, 
                OrganisationId = model.OrganisationId
            };

            await _context.AddAsync(location);
            await _context.SaveChangesAsync();
            
        }

        public async Task Delete(int id)
        {
            var donation = await _context.Donations.FirstOrDefaultAsync(x => x.Id == id);

            if (donation == null)
            {
                return;
            }

            _context.Donations.Remove(donation);
            await _context.SaveChangesAsync();
        }

        public async Task<List<DonationGetModel>> GetAll()
        {
            return await _context.Donations.Select(x => new DonationGetModel
            {
                Id = x.Id,
                donation = x.donation,
                UserId=x.UserId,
                OrganisationId=x.OrganisationId
            }).ToListAsync();
        }

        public async Task<DonationGetModel> GetById(int id)
        {
            var donation = await _context.Donations.FirstOrDefaultAsync(x => x.Id == id);

            if (donation == null)
            {
                return new DonationGetModel();
            }
            return new DonationGetModel
            {
                Id = donation.Id,
                donation = donation.donation,
                UserId = donation.UserId,
                OrganisationId = donation.OrganisationId
            };
        }

        public async Task Update(DonationPutModel model, int id)
        {
            var donation = await _context.Donations.FirstOrDefaultAsync(x => x.Id == id);

            if (model == null || donation == null) { return; }

            donation.donation = model.donation;

            var userId = await _context.Users.FirstOrDefaultAsync(x => x.Id == model.UserId);                   //verific daca cheile pe care vr sa le introduc exista in baza
            var orgId = await _context.Organisations.FirstOrDefaultAsync(x => x.Id == model.OrganisationId);    //daca nu le las pe cele vechi

            if (userId != null)
            {
                donation.UserId = model.UserId;
            }
            if (orgId != null)
            {
                donation.OrganisationId = model.OrganisationId;
            }

            await _context.SaveChangesAsync();
        }
    }
}
