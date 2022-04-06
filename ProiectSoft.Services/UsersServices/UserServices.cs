using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProiectSoft.BLL.Helpers;
using ProiectSoft.DAL;
using ProiectSoft.DAL.Entities;
using ProiectSoft.DAL.Models.UserModels;
using ProiectSoft.DAL.Wrappers;
using ProiectSoft.Services.UriServicess;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.Services.UsersServices
{
    public class UserServices : IUserServices
    {
        private readonly AppDbContext _context;
        private readonly IUriServices _uriServices;
        private readonly IMapper _mapper;
        private readonly ILogger<UserServices> _logger;
        public UserServices(AppDbContext context, IUriServices uriServices, IMapper mapper, ILogger<UserServices> logger)
        {
            _uriServices = uriServices;
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task Delete(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                _logger.LogError($"OPS! User with id:{id} does not exist in our database");
                return;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<List<UserGetModel>>> GetAll(PaginationFilter filter, string route)
        {
            LogContext.PushProperty("IdentificationMessage", $"GetAll users request for Page:{filter.PageNumber}, with number of objects: {filter.PageSize}");

            var users = await _context.Users
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var userModels = new List<UserGetModel>();

            foreach (var user in users)
            {
                var userModel = _mapper.Map<UserGetModel>(user);
                userModels.Add(userModel);
            }

            var usersListCount = await _context.Users.CountAsync();

            var pagedResponse = PaginationHelper.CreatePagedReponse<UserGetModel>(userModels, filter, usersListCount, _uriServices, route);

            return pagedResponse;
        }

        public async Task<Response<UserGetModel>> GetById(Guid id)
        {
            LogContext.PushProperty("IdentificationMessage", $"GetById user request for {id}");

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                _logger.LogError($"OPS! User with id:{id} does not exist in our database");
                return new Response<UserGetModel>(false, $"Id {id} doesn't exist");
            }

            var userGetModel = _mapper.Map<UserGetModel>(user);

            return new Response<UserGetModel>(userGetModel);
        }

        public async Task Update(UserPutModel model, Guid id)
        {
            LogContext.PushProperty("IdentificationMessage", $"Update user request for {model.LastName}");

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                _logger.LogError($"OPS! User with id:{id} does not exist in our database");
                return;
            }

            _mapper.Map<UserPutModel, User>(model, user);

            await _context.SaveChangesAsync();
        }
    }
}
