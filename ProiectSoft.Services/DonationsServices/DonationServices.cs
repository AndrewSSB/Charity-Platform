using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProiectSoft.BLL.Helpers;
using ProiectSoft.DAL;
using ProiectSoft.DAL.Entities;
using ProiectSoft.DAL.Models.DonationModels;
using ProiectSoft.DAL.Wrappers;
using ProiectSoft.Services.UriServicess;
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
        private readonly IUriServices _uriServices;
        private readonly IMapper _mapper;
        public DonationServices(AppDbContext context, IUriServices uriServices, IMapper mapper)
        {
            _uriServices = uriServices;
            _context = context;
            _mapper = mapper;
        }

        public async Task Create(DonationPostModel model)
        {
            if (model == null) { return; }

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == model.UserId);
            var organisation = await _context.Organisations.FirstOrDefaultAsync(x => x.Id == model.OrganisationId); //daca nu exista userID-ul, sau organisationID-ul renunt

            if (user == null) { return; }

            if (organisation == null) { return; }
            
            var donation = _mapper.Map<Donation>(model);

            await _context.AddAsync(donation);
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

        public async Task<PagedResponse<List<DonationGetModel>>> GetAll(PaginationFilter filter, string route)
        {
            /*var donations = await _context.Donations.Select(x => new DonationGetModel
            {
                Id = x.Id,
                donation = x.donation,
                UserId = x.UserId,
                OrganisationId = x.OrganisationId
            })
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync(); */

            var donations = await _context.Donations
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var donationsModels = new List<DonationGetModel>();

            foreach (var donation in donations)
            {
                var donationsModel = _mapper.Map<DonationGetModel>(donation);
                donationsModels.Add(donationsModel);
            }

            var donationsListCount = await _context.Donations.CountAsync();

            var pagedResponse = PaginationHelper.CreatePagedReponse<DonationGetModel>(donationsModels, filter, donationsListCount, _uriServices, route);

            return pagedResponse;
        }

        public async Task<DonationGetModel> GetById(int id)
        {
            var donation = await _context.Donations.FirstOrDefaultAsync(x => x.Id == id);

            if (donation == null)
            {
                return new DonationGetModel();
            }
            
            var donationGetModel = _mapper.Map<DonationGetModel>(donation);

            return donationGetModel;
        }

        public async Task Update(DonationPutModel model, int id)
        {
            var donation = await _context.Donations.FirstOrDefaultAsync(x => x.Id == id);

            if (model == null || donation == null) { return; }

            donation.donation = model.donation;

            var userId = await _context.Users.FirstOrDefaultAsync(x => x.Id == model.UserId);                   //verific daca cheile pe care vr sa le introduc exista in baza
            var orgId = await _context.Organisations.FirstOrDefaultAsync(x => x.Id == model.OrganisationId);    //daca nu le las pe cele vechi
            //aici nu fac mapper momentan (pana nu fac middlewear asa o las)
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
