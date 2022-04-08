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
using Utils.MiddlewareManager;

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
                throw new AppException("You have entered a wrong id for shelter");
            }

            //de fiecare data cand adaug un refugiat, available space din shelter scade
            if (shelter.availableSpace == 0)
            {
                _logger.LogError($"There is no space left in {shelter.Name} for {model.Name}");
                throw new AppException("There is no space left for refugees, we're sorry");
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
                throw new KeyNotFoundException($"There is no refugee with id: {id}, try with another one");
            }

            var space = await _context.Shelters.Where(x => x.Id == refugee.ShelterId).FirstOrDefaultAsync(); //cred ca asa vine ? //asa o las momentan

            if (space != null)
            {
                space.availableSpace += 1;
            }

            _context.Refugees.Remove(refugee);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<List<RefugeeGetModel>>> GetAll(PaginationFilter filter, string route, string searchName, string orderBy, bool descending, int? age, string flag)
        {
            LogContext.PushProperty("IdentificationMessage", $"GetAll refugee request for Page:{filter.PageNumber}, with number of objects: {filter.PageSize}");

            var refugees = await _context.Refugees
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            //search by name
            if (!String.IsNullOrEmpty(searchName))
            {
                refugees = refugees.Where(x => x.Name!.Contains(searchName)).ToList(); //nu merge ToListAsync();
            }

            //filter option (am facut dupa age, nu aveam dupa ce altceva) //se putea si mai bine. probabil modific pe viitor
            if (age != null)
            {
                refugees = await Filter(refugees, age, flag);
            }
            // daca pune ceva dupa care nu se poate ordona -> intra pe default si in plus daca descending e true atunci
            //atunci trebuie sa pot intra in functie si sa ordonez default descrescator e gandita cam prost stiu, poate o modfic
            refugees = await OrderBy(refugees, orderBy, descending);

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
                throw new KeyNotFoundException("Refugee not found");
                //return new Response<RefugeeGetModel>(false, $"Id {id} doesn't exist");
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
                throw new KeyNotFoundException($"Refugee not found for id{id}");
            }

            var refugeeShel = await _context.Refugees.FirstOrDefaultAsync(x => x.ShelterId == model.ShelterId);

            if (refugeeShel == null)
            {
                _logger.LogError($"There is no shelter with ID:{model.ShelterId}. Update failed");
                throw new KeyNotFoundException($"U have entered an invalid if for shelter. id:{id}");
            }   

            _mapper.Map<RefugeePutModel, Refugee>(model, refugee);

            await _context.SaveChangesAsync();
        }

        private async Task<List<Refugee>> OrderBy(List<Refugee> refugees, string orderBy, bool descending)
        {
            switch (orderBy)
            {
                case "Name":
                    refugees = descending == false ? refugees.OrderBy(x => x.Name).ToList() : refugees.OrderByDescending(x => x.Name).ToList();
                    break;
                case "Date":
                    refugees = descending == false ? refugees.OrderBy(s => s.DateCreated).ToList() : refugees.OrderByDescending(x => x.DateCreated).ToList();
                    break;
                case "Age":
                    refugees = descending == false ? refugees.OrderBy(s => s.Age).ToList() : refugees.OrderByDescending(x => x.Age).ToList();
                    break;
                default:
                    refugees = descending == false ? refugees.OrderBy(s => s.lastName).ToList() : refugees.OrderByDescending(x=>x.lastName).ToList();
                    break;  
            }

            return refugees;
        }

        private async Task<List<Refugee>> Filter(List<Refugee> refugees, int? age, string flag)
        {
            if (0 < age && age < 120) //probabil ar trebui sa dau throw la o exceptie (dupa ce fac middleware o sa revin aici)
            {
                if (flag == "<")
                {
                    return refugees.Where(x => x.Age < age).ToList();
                }
                else if (flag == ">")
                {
                    return refugees.Where(x => x.Age > age).ToList();
                }
                else
                {
                    throw new AppException("You entered a wrong flag");
                }
            }
            else
            {
                throw new AppException("Age is not valid");
            }
        }
    }
}
