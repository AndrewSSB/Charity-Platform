using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProiectSoft.BLL.Helpers;
using ProiectSoft.DAL;
using ProiectSoft.DAL.Entities;
using ProiectSoft.DAL.Models.UserModels;
using ProiectSoft.DAL.Wrappers;
using ProiectSoft.Services.UriServicess;
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

        public UserServices(AppDbContext context, IUriServices uriServices, IMapper mapper)
        {
            _uriServices = uriServices;
            _context = context;
            _mapper = mapper;
        }

        public async Task Delete(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                return;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<List<UserGetModel>>> GetAll(PaginationFilter filter, string route)
        {
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

            var usersListCount = await _context.Cases.CountAsync();

            var pagedResponse = PaginationHelper.CreatePagedReponse<UserGetModel>(userModels, filter, usersListCount, _uriServices, route);

            return pagedResponse;
        }

        public async Task<UserGetModel> GetById(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                return new UserGetModel();
            }

            var userGetModel = _mapper.Map<UserGetModel>(user);

            return userGetModel;
        }

        public async Task Update(UserPutModel model, Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (user == null || model == null)
            {
                return;
            }

            _mapper.Map<UserPutModel, User>(model, user);

            await _context.SaveChangesAsync();
        }
    }
}
