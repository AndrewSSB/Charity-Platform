using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ProiectSoft.BLL.Helpers;
using ProiectSoft.DAL;
using ProiectSoft.DAL.Entities;
using ProiectSoft.DAL.Models.DonationModels;
using ProiectSoft.DAL.Wrappers;
using ProiectSoft.Services.UriServicess;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProiectSoft.Services.DonationsServices
{
    public class DonationServices : IDonationServices
    {
        private readonly AppDbContext _context;
        private readonly IUriServices _uriServices;
        private readonly IMapper _mapper;
        private readonly ILogger<DonationServices> _logger;
        public DonationServices(AppDbContext context, IUriServices uriServices, IMapper mapper, ILogger<DonationServices> logger)
        {
            _uriServices = uriServices;
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task Create(DonationPostModel model)
        {
            LogContext.PushProperty("IdentificationMessage", $"Create donation request for {JsonConvert.SerializeObject(model)}");

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == model.UserId);
            var organisation = await _context.Organisations.FirstOrDefaultAsync(x => x.Id == model.OrganisationId); //daca nu exista userID-ul, sau organisationID-ul renunt

            if (user == null) {
                _logger.LogError($"OPS! {model.UserId} does not exist in our database");
                return; 
            }

            if (organisation == null) {
                _logger.LogError($"OPS! {model.OrganisationId} does not exist in our database");
                return; 
            }
            
            var donation = _mapper.Map<Donation>(model);

            await _context.AddAsync(donation);
            await _context.SaveChangesAsync();
            
        }

        public async Task Delete(int id)
        {
            LogContext.PushProperty("IdentificationMessage", $"Delete donation request for {id}");

            var donation = await _context.Donations.FirstOrDefaultAsync(x => x.Id == id);

            if (donation == null)
            {
                _logger.LogError($"OPS! Donation with id:{id} does not exist in our database");
                return;
            }

            _context.Donations.Remove(donation);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<List<DonationGetModel>>> GetAll(PaginationFilter filter, string route)
        {
            LogContext.PushProperty("IdentificationMessage", $"GetAll donation request for Page:{filter.PageNumber}, with number of objects: {filter.PageSize}");

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

        public async Task<Response<DonationGetModel>> GetById(int id)
        {
            LogContext.PushProperty("IdentificationMessage", $"GetById donation request for {id}");

            var donation = await _context.Donations.FirstOrDefaultAsync(x => x.Id == id);

            if (donation == null)
            {
                _logger.LogError($"OPS! Donation with id:{id} does not exist in our database");
                return new Response<DonationGetModel>(false, $"Id {id} doesn't exist");
            }
            
            var donationGetModel = _mapper.Map<DonationGetModel>(donation);

            return new Response<DonationGetModel>(donationGetModel);
        }

        public async Task Update(DonationPutModel model, int id)
        {
            LogContext.PushProperty("IdentificationMessage", $"Update donation request for {JsonConvert.SerializeObject(model)}");

            var donation = await _context.Donations.FirstOrDefaultAsync(x => x.Id == id);

            if (donation == null) {
                _logger.LogError($"There is no donation with ID:{id} in our database");
                return; 
            }

            var userId = await _context.Users.FirstOrDefaultAsync(x => x.Id == model.UserId);                   //verific daca cheile pe care vr sa le introduc exista in baza
            var orgId = await _context.Organisations.FirstOrDefaultAsync(x => x.Id == model.OrganisationId);    //daca nu le las pe cele vechi

            if (userId == null)
            {
                _logger.LogError($"There is no user with ID:{userId}. Update failed");
                return;
            }
            if (orgId != null)
            {
                _logger.LogError($"There is no organisation with ID:{orgId}. Update failed");
                return;
            }

            _mapper.Map<DonationPutModel, Donation>(model, donation);

            await _context.SaveChangesAsync();
        }
    }
}
