using ProiectSoft.DAL.Models.UserModels;
using ProiectSoft.DAL.Wrappers;
using ProiectSoft.DAL.Wrappers.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.Services.UsersServices
{
    public interface IUserServices
    {
        Task<PagedResponse<List<UserGetModel>>> GetAll(UserFilter filter, string route);
        Task<Response<UserGetModel>> GetById(Guid id);
        Task Update(UserPutModel model, Guid id);
        Task Delete(Guid id);
    }
}
