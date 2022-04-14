using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProiectSoft.BLL.Helpers;
using ProiectSoft.DAL;
using ProiectSoft.DAL.Entities;
using ProiectSoft.DAL.Models.UserModels;
using ProiectSoft.DAL.Wrappers;
using ProiectSoft.DAL.Wrappers.Filters;
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

        public async Task<PagedResponse<List<UserGetModel>>> GetAll(UserFilter filter, string route)
        {
            LogContext.PushProperty("IdentificationMessage", $"GetAll users request for Page:{filter.PageNumber}, with number of objects: {filter.PageSize}");

            IQueryable<User> users = _context.Users;

            if (!String.IsNullOrEmpty(filter.searchName))
            {
                users = users.Where(x => x.UserName!.Contains(filter.searchName));
            }

            if (!string.IsNullOrEmpty(filter.userName))
            {
                users = users.Where(x => x.UserName!.Contains(filter.userName));
            }

            if (!string.IsNullOrEmpty(filter.firstName))
            {
                users = users.Where(x => x.FirstName!.Contains(filter.firstName));
            }

            if (filter.dateCreated != null)
            {
                users = users.Where(x => x.DateCreated.Value.ToString("mm/dd/yyyy").Contains(filter.dateCreated.Value.ToString("mm/dd/yyyy")));
            }

            switch (filter.orderBy)
            {
                case "Name":
                    users = !filter.descending == false ? users.OrderBy(x => x.UserName) : users.OrderByDescending(x => x.UserName);
                    break;
                case "Email":
                    users = !filter.descending == false ? users.OrderBy(s => s.Email) : users.OrderByDescending(x => x.Email);
                    break;
                case "FirstName":
                    users = !filter.descending == false ? users.OrderBy(s => s.FirstName) : users.OrderByDescending(x => x.FirstName);
                    break;
                default:
                    users = !filter.descending == false ? users.OrderBy(s => s.LastName) : users.OrderByDescending(x => x.LastName);
                    break;
            }

            var userModels = users
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(_mapper.Map<UserGetModel>)
                .ToList();

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
    }
}
