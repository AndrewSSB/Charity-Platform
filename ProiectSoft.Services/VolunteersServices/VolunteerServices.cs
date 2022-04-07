using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProiectSoft.BLL.Helpers;
using ProiectSoft.DAL;
using ProiectSoft.DAL.Entities;
using ProiectSoft.DAL.Models.VolunteerModels;
using ProiectSoft.DAL.Wrappers;
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
                return;
            }

            _context.Volunteers.Remove(volunteer);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<List<VolunteerGetModel>>> GetAll(PaginationFilter filter, string route, string searchName, string orderBy, bool descending)
        {
            LogContext.PushProperty("IdentificationMessage", $"GetAll volunteers request for Page:{filter.PageNumber}, with number of objects: {filter.PageSize}");

            var volunteers = await _context.Volunteers
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            if (string.IsNullOrEmpty(searchName))
            {
                volunteers = volunteers.Where(x => x.Name!.Contains(searchName)).ToList();
            }

            volunteers = await OrderBy(volunteers, orderBy, descending);

            var volunteerModels = new List<VolunteerGetModel>();

            foreach (var volunteer in volunteers)
            {
                var volunteerModel = _mapper.Map<VolunteerGetModel>(volunteer);
                volunteerModels.Add(volunteerModel);
            }

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
                return new Response<VolunteerGetModel>(false, $"Id {id} doesn't exist");
            }

            var volunteerGetModel = _mapper.Map<VolunteerGetModel>(volunteer);

            return new Response<VolunteerGetModel>(volunteerGetModel);
        }

        public async Task Update(VolunteerPutModel model, int id)
        {
            var volunteer = await _context.Volunteers.FirstOrDefaultAsync(x => x.Id == id);

            if (volunteer == null) {
                _logger.LogError($"There is no volunteer with ID:{id} in our database");
                return; 
            }

            var volunteerOrg = await _context.Organisations.FirstOrDefaultAsync(x => x.Id == model.OrganisationId);

            if (volunteerOrg == null)
            {
                _logger.LogError($"There is no organisation with ID:{model.OrganisationId} in our database");
                return;
            }

            _mapper.Map<VolunteerPutModel, Volunteer>(model, volunteer);

            await _context.SaveChangesAsync();
        }

        private async Task<List<Volunteer>> OrderBy(List<Volunteer> refugees, string orderBy, bool descending)
        {
            switch (orderBy)
            {
                case "Name":
                    refugees = descending == false ? refugees.OrderBy(x => x.Name).ToList() : refugees.OrderByDescending(x => x.Name).ToList();
                    break;
                case "Date":
                    refugees = descending == false ? refugees.OrderBy(s => s.DateCreated).ToList() : refugees.OrderByDescending(x => x.DateCreated).ToList();
                    break;
                case "Position":
                    refugees = descending == false ? refugees.OrderBy(s => s.Position).ToList() : refugees.OrderByDescending(x => x.Position).ToList();
                    break;
                default:
                    refugees = descending == false ? refugees.OrderBy(s => s.lastName).ToList() : refugees.OrderByDescending(x => x.lastName).ToList();
                    break;
            }

            return refugees;
        }
    }
}
