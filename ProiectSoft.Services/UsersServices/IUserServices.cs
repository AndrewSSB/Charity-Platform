using ProiectSoft.DAL.Models.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.Services.UsersServices
{
    public interface IUserServices
    {
        Task<List<UserGetModel>> GetAll();
        Task<UserGetModel> GetById(Guid id);
        Task Update(UserPutModel model, Guid id);
        Task Delete(Guid id);
    }
}
