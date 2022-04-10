using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ProiectSoft.BLL.Helpers;
using ProiectSoft.DAL;
using ProiectSoft.DAL.Entities;
using ProiectSoft.DAL.Models.DonationModels;
using ProiectSoft.DAL.Wrappers;
using ProiectSoft.DAL.Wrappers.Filters;
using ProiectSoft.Services.UriServicess;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Utils.MiddlewareManager;

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
                throw new KeyNotFoundException($"{model.UserId} does not exist");
            }

            if (organisation == null) {
                _logger.LogError($"OPS! {model.OrganisationId} does not exist in our database");
                throw new KeyNotFoundException($"{model.OrganisationId} does not exist");
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
                throw new KeyNotFoundException($"There is no donation with id {id}");
            }

            _context.Donations.Remove(donation);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<List<DonationGetModel>>> GetAll(DonationFilter filter, string route)
        {
            LogContext.PushProperty("IdentificationMessage", $"GetAll donation request for Page:{filter.PageNumber}, with number of objects: {filter.PageSize}");

            IQueryable<Donation> donations = _context.Donations; //good work with filters

            /*if (filter.prop != null) //filtrare mai buna
            {
                donations = donations.Where(age > prop);
            }*/

            switch (filter.orderBy)
            {
                case "Date":
                    donations = !filter.descending ? donations.OrderBy(x => x.DateCreated) : donations.OrderByDescending(x => x.DateCreated);
                    break;
                case "User":
                    donations = !filter.descending ? donations.OrderBy(s => s.UserId) : donations.OrderByDescending(x => x.UserId);
                    break;
                case "Organisation":
                    donations = !filter.descending ? donations.OrderBy(s => s.OrganisationId) : donations.OrderByDescending(x => x.OrganisationId);
                    break;
                default:
                    donations = !filter.descending ? donations.OrderBy(s => s.Id) : donations.OrderByDescending(x => x.Id);
                    break;
            }

            var result = donations
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(_mapper.Map<DonationGetModel>)
                .ToList();

            var donationsListCount = await _context.Donations.CountAsync();

            var pagedResponse = PaginationHelper.CreatePagedReponse<DonationGetModel>(result, filter, donationsListCount, _uriServices, route);

            return pagedResponse;
        }

        public async Task<Response<DonationGetModel>> GetById(int id)
        {
            LogContext.PushProperty("IdentificationMessage", $"GetById donation request for {id}");

            var donation = await _context.Donations.FirstOrDefaultAsync(x => x.Id == id);

            if (donation == null)
            {
                _logger.LogError($"OPS! Donation with id:{id} does not exist in our database");
                throw new KeyNotFoundException($"There is no donation with id {id}");
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
                throw new KeyNotFoundException($"There is no donation with id: {id}"); 
            }

            var userId = await _context.Users.FirstOrDefaultAsync(x => x.Id == model.UserId);                   //verific daca cheile pe care vr sa le introduc exista in baza
            var orgId = await _context.Organisations.FirstOrDefaultAsync(x => x.Id == model.OrganisationId);    //daca nu le las pe cele vechi

            if (userId == null)
            {
                _logger.LogError($"There is no user with ID:{userId}. Update failed");
                throw new AppException($"There is no user with id: {model.UserId}");
            }
            if (orgId == null)
            {
                _logger.LogError($"There is no organisation with ID:{orgId}. Update failed");
                throw new AppException($"There is no organisation with id {model.OrganisationId}");
            }

            _mapper.Map<DonationPutModel, Donation>(model, donation);

            await _context.SaveChangesAsync();
        }
    }
}
