using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProiectSoft.BLL.Helpers;
using ProiectSoft.DAL;
using ProiectSoft.DAL.Entities;
using ProiectSoft.DAL.Models.VolunteerModels;
using ProiectSoft.DAL.Wrappers;
using ProiectSoft.DAL.Wrappers.Filters;
using ProiectSoft.Services.UriServicess;
using Serilog.Context;
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
        private readonly ILogger<VolunteerServices> _logger;

        public VolunteerServices(AppDbContext context, IUriServices uriServices, IMapper mapper, ILogger<VolunteerServices> logger)
        {
            _uriServices = uriServices;
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task Create(VolunteerPostModel model)
        {
            LogContext.PushProperty("IdentificationMessage", $"Create volunteer request for {model.Name}");

            var orgId = await _context.Organisations.FirstOrDefaultAsync(x => x.Id == model.OrganisationId);

            if (orgId == null) {
                _logger.LogError($"OPS! There is no organisation with id:{model.OrganisationId}");
                return; 
            }

            var volunteer = _mapper.Map<Volunteer>(model);

            await _context.AddAsync(volunteer);
            await _context.SaveChangesAsync();
            
        }

        public async Task Delete(int id)
        {
            LogContext.PushProperty("IdentificationMessage", $"Delete volunteer request for {id}");

            var volunteer = await _context.Volunteers.FirstOrDefaultAsync(x => x.Id == id);

            if (volunteer == null)
            {
                _logger.LogError($"OPS! Volunteer with id:{id} does not exist in our database");
                throw new KeyNotFoundException($"There is no volunteer with id: {id}");
            }

            _context.Volunteers.Remove(volunteer);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<List<VolunteerGetModel>>> GetAll(VolunteerFilter filter, string route)
        {
            LogContext.PushProperty("IdentificationMessage", $"GetAll volunteers request for Page:{filter.PageNumber}, with number of objects: {filter.PageSize}");

            IQueryable<Volunteer> volunteers = _context.Volunteers;

            if (!string.IsNullOrEmpty(filter.searchName))
            {
                volunteers = volunteers.Where(x => x.Name!.Contains(filter.searchName));
            }

            if (!string.IsNullOrEmpty(filter.name))
            {
                volunteers = volunteers.Where(x => x.Name!.Contains(filter.name));
            }

            if (!string.IsNullOrEmpty(filter.lastName))
            {
                volunteers = volunteers.Where(x => x.lastName!.Contains(filter.lastName));
            }

            if (!string.IsNullOrEmpty(filter.position))
            {
                volunteers = volunteers.Where(x => x.Position!.Contains(filter.position));
            }

            if (filter.dateCreated != null)
            {
                volunteers = volunteers.Where(x => x.DateCreated.Value.ToString("mm/dd/yyyy").Contains(filter.dateCreated.Value.ToString("mm/dd/yyyy")));
            }

            switch (filter.orderBy)
            {
                case "Name":
                    volunteers = !filter.descending ? volunteers.OrderBy(x => x.Name) : volunteers.OrderByDescending(x => x.Name);
                    break;
                case "Date":
                    volunteers = !filter.descending ? volunteers.OrderBy(s => s.DateCreated) : volunteers.OrderByDescending(x => x.DateCreated);
                    break;
                case "Position":
                    volunteers = !filter.descending ? volunteers.OrderBy(s => s.Position): volunteers.OrderByDescending(x => x.Position);
                    break;
                default:
                    volunteers = !filter.descending ? volunteers.OrderBy(s => s.lastName) : volunteers.OrderByDescending(x => x.lastName);
                    break;
            }

            var volunteerModels = volunteers
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(_mapper.Map<VolunteerGetModel>)
                .ToList();

            var volunteerListCount = await _context.Volunteers.CountAsync();

            var pagedResponse = PaginationHelper.CreatePagedReponse<VolunteerGetModel>(volunteerModels, filter, volunteerListCount, _uriServices, route);

            return pagedResponse;
        }

        public async Task<Response<VolunteerGetModel>> GetById(int id)
        {
            LogContext.PushProperty("IdentificationMessage", $"GetById volunteer request for {id}");

            var volunteer = await _context.Volunteers.FirstOrDefaultAsync(x => x.Id == id);

            if (volunteer == null)
            {
                _logger.LogError($"OPS! Volunteer with id:{id} does not exist in our database");
                throw new KeyNotFoundException($"There is no volunteer with id: {id}");
            }

            var volunteerGetModel = _mapper.Map<VolunteerGetModel>(volunteer);

            return new Response<VolunteerGetModel>(volunteerGetModel);
        }

        public async Task Update(VolunteerPutModel model, int id)
        {
            var volunteer = await _context.Volunteers.FirstOrDefaultAsync(x => x.Id == id);

            if (volunteer == null) {
                _logger.LogError($"There is no volunteer with ID:{id} in our database");
                throw new KeyNotFoundException($"There is no volunteer with id: {id}");
            }

            var volunteerOrg = await _context.Organisations.FirstOrDefaultAsync(x => x.Id == model.OrganisationId);

            if (volunteerOrg == null)
            {
                _logger.LogError($"There is no organisation with ID:{model.OrganisationId} in our database");
                throw new KeyNotFoundException($"There is no organisation with id: {model.OrganisationId}"); ;
            }

            _mapper.Map<VolunteerPutModel, Volunteer>(model, volunteer);

            await _context.SaveChangesAsync();
        }
    }
}
