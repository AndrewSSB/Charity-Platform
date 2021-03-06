using ProiectSoft.DAL.Models.LocationModels;
using ProiectSoft.DAL.Wrappers;
using ProiectSoft.DAL.Wrappers.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.Services.LocationsServices
{
    public interface ILocationServices
    {
        Task<PagedResponse<List<LocationGetModel>>> GetAll(LocationFilter filter, string route);
        Task<Response<LocationGetModel>> GetById(int id);
        Task Create(LocationPostModel model);
        Task Update(LocationPutModel model, int id);
        Task Delete(int id);
    }
}
