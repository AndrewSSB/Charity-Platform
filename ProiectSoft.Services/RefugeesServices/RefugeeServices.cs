using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ProiectSoft.BLL.Helpers;
using ProiectSoft.DAL;
using ProiectSoft.DAL.Entities;
using ProiectSoft.DAL.Models.RefugeeModels;
using ProiectSoft.DAL.Wrappers;
using ProiectSoft.Services.UriServicess;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.Services.RefugeesServices
{
    public class RefugeeServices : IRefugeeServices
    {
        private readonly AppDbContext _context;
        private readonly IUriServices _uriService;
        private readonly IMapper _mapper;
        private readonly ILogger<RefugeeServices> _logger;

        public RefugeeServices(AppDbContext context, IUriServices uriService, IMapper mapper, ILogger<RefugeeServices> logger)
        {
            _context = context;
            _uriService = uriService;
            _mapper = mapper;   
            _logger = logger;
        }

        public async Task Create(RefugeePostModel model)
        {
            LogContext.PushProperty("IdentificationMessage", $"Create refugee request for {model.Name}");

            var shelter = await _context.Shelters.FirstOrDefaultAsync(x => x.Id == model.ShelterId);

            if (shelter == null) {
                _logger.LogError($"OPS! {model.ShelterId} does not exist in our database");
                return; 
            }

            //de fiecare data cand adaug un refugiat, available space din shelter scade
            if (shelter.availableSpace == 0)
            {
                _logger.LogError($"There is no space left in {shelter.Name} for {model.Name}");
                return;
            }

            var refugee = _mapper.Map<Refugee>(model);

            shelter.availableSpace -= 1;

            await _context.AddAsync(refugee);
            await _context.SaveChangesAsync();
            
        }

        public async Task Delete(int id)
        {
            LogContext.PushProperty("IdentificationMessage", $"Delete refugee request for {id}");

            var refugee = await _context.Refugees.FirstOrDefaultAsync(x => x.Id == id);

            if (refugee == null)
            {
                _logger.LogError($"OPS! Refugee with id:{id} does not exist in our database");
                return;
            }

            var space = await _context.Shelters.Where(x => x.Id == refugee.ShelterId).FirstOrDefaultAsync(); //cred ca asa vine ? //asa o las momentan

            if (space != null)
            {
                space.availableSpace += 1;
            }

            _context.Refugees.Remove(refugee);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<List<RefugeeGetModel>>> GetAll(PaginationFilter filter, string route)
        {
            LogContext.PushProperty("IdentificationMessage", $"GetAll refugee request for Page:{filter.PageNumber}, with number of objects: {filter.PageSize}");

            var refugees = await _context.Refugees
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var refugeeModels = new List<RefugeeGetModel>();

            foreach (var refugee in refugees)
            {
                var refugeeModel = _mapper.Map<RefugeeGetModel>(refugee);
                refugeeModels.Add(refugeeModel);
            }

            var refugeesListCount = await _context.Refugees.CountAsync();

            var pagedResponse = PaginationHelper.CreatePagedReponse<RefugeeGetModel>(refugeeModels, filter, refugeesListCount, _uriService, route);

            return pagedResponse;
        }

        public async Task<Response<RefugeeGetModel>> GetById(int id)
        {
            LogContext.PushProperty("IdentificationMessage", $"GetById refugee request for {id}");

            var refugee = await _context.Refugees.FirstOrDefaultAsync(x => x.Id == id);

            if (refugee == null)
            {
                _logger.LogError($"OPS! Refugee with id:{id} does not exist in our database");
                return new Response<RefugeeGetModel>(false, $"Id {id} doesn't exist");
            }
            
            var refugeeGetModel = _mapper.Map<RefugeeGetModel>(refugee);

            return new Response<RefugeeGetModel>(refugeeGetModel);
        }

        public async Task Update(RefugeePutModel model, int id)
        {
            LogContext.PushProperty("IdentificationMessage", $"Update refugee request for {model.Name}");

            var refugee = await _context.Refugees.FirstOrDefaultAsync(x => x.Id == id);

            if (refugee == null) {
                _logger.LogError($"There is no refugee with ID:{id} in our database");
                return; 
            }

            var refugeeShel = await _context.Refugees.FirstOrDefaultAsync(x => x.ShelterId == model.ShelterId);

            if (refugeeShel == null)
            {
                _logger.LogError($"There is no shelter with ID:{model.ShelterId}. Update failed");
                return;
            }

            _mapper.Map<RefugeePutModel, Refugee>(model, refugee);

            await _context.SaveChangesAsync();
        }
    }
}
