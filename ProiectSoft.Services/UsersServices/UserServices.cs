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
                throw new KeyNotFoundException($"Thre is no user with id:{id}");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<List<UserGetModel>>> GetAll(PaginationFilter filter, string route, 
            string searchUserName, string orderBy, bool descending, string[] filters)
        {
            LogContext.PushProperty("IdentificationMessage", $"GetAll users request for Page:{filter.PageNumber}, with number of objects: {filter.PageSize}");

            var users = await _context.Users
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            if (!String.IsNullOrEmpty(searchUserName))
            {
                users = users.Where(x => x.UserName!.Contains(searchUserName)).ToList();
            }

            users = await OrderBy(users, orderBy, descending);

            if (filters.Count() > 0)
            {
                users = await FilterUser(users, filters);
            }

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
                throw new KeyNotFoundException($"Thre is no user with id: {id}");
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
                throw new KeyNotFoundException($"There is no user with id:{id}");
            }

            _mapper.Map<UserPutModel, User>(model, user);

            await _context.SaveChangesAsync();
        }

        private async Task<List<User>> OrderBy(List<User> users, string orderBy, bool descending)
        {
            switch (orderBy)
            {
                case "Name":
                    users = descending == false ? users.OrderBy(x => x.UserName).ToList() : users.OrderByDescending(x => x.UserName).ToList();
                    break;
                case "Email":
                    users = descending == false ? users.OrderBy(s => s.Email).ToList() : users.OrderByDescending(x => x.Email).ToList();
                    break;
                case "Age":
                    users = descending == false ? users.OrderBy(s => s.FirstName).ToList() : users.OrderByDescending(x => x.FirstName).ToList();
                    break;
                default:
                    users = descending == false ? users.OrderBy(s => s.LastName).ToList() : users.OrderByDescending(x => x.LastName).ToList();
                    break;
            }

            return users;
        }

        private async Task<List<User>> FilterUser(List<User> users, string[] filters)
        {
            if (filters.Length == 1)
            {
                users = users.Where(x => x.UserName.Contains(filters[0])).ToList();
            }
            else if (filters.Length == 2)
            {
                users = users.Where(x => x.UserName.Contains(filters[0]) || 
                x.FirstName.Contains(filters[1])).ToList();
            }else if (filters.Length == 3)
            {
                users = users.Where(x => x.UserName.Contains(filters[0]) ||
                x.FirstName.Contains(filters[1]) ||
                x.DateCreated.Value.ToString("mm/dd/yyyy").Contains(filters[2]) ).ToList();
            }

            return users.ToList();
        }
    }
}
