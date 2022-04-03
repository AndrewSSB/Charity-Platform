using Microsoft.EntityFrameworkCore;
using ProiectSoft.DAL;
using ProiectSoft.DAL.Entities;
using ProiectSoft.DAL.Models.UserModels;
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

        public UserServices(AppDbContext context)
        {
            _context = context;
        }

        public async Task Create(UserPostModel model)
        {
            if (model == null) { return; }
            
            var user = new User
            {
                Id = model.Id == null ? Guid.NewGuid() : new Guid(model.Id), //merge sa si introduci tu un guid da e mai naspa
                UserName = model.UserName,
                Email = model.email,
                Type = model.Type,
                DateCreated = model.DateCreated == null ? DateTime.Now : model.DateCreated,
                DateModified = model.DateModified == null ? DateTime.Now : model.DateModified
            };

            await _context.AddAsync(user);
            await _context.SaveChangesAsync();
            
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

        public async Task<List<UserGetModel>> GetAll()
        {
            return await _context.Users.Select(x => new UserGetModel
            {
                Id = x.Id.ToString(),
                UserName = x.UserName,
                email = x.Email,
                Type = x.Type,
                DateCreated = x.DateCreated.ToString(),
                DateModified = x.DateModified.ToString(),
            }).ToListAsync();
        }

        public async Task<UserGetModel> GetById(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                return new UserGetModel();
            }

            var userGetModel = new UserGetModel
            {
                Id = user.Id.ToString(),
                UserName = user.UserName,
                email = user.Email,
                Type = user.Type,
                DateCreated = user.DateCreated.ToString(),
                DateModified = user.DateModified.ToString(),
            };

            return userGetModel;
        }

        public async Task Update(UserPutModel model, Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (user == null || model == null)
            {
                return;
            }

            user.UserName = model.UserName;
            user.Email = model.email;
            user.Type = model.Type;
            user.DateCreated = model.DateCreated == null ? DateTime.Now : model.DateCreated;
            user.DateModified = DateTime.Now;

            await _context.SaveChangesAsync();
        }
    }
}
