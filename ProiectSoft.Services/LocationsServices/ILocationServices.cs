using ProiectSoft.DAL.Models.LocationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.Services.LocationsServices
{
    public interface ILocationServices
    {
        Task<List<LocationGetModel>> GetAll();
        Task<LocationGetModel> GetById(int id);
        Task Create(LocationPostModel model);
        Task Update(LocationPutModel model, int id);
        Task Delete(int id);
    }
}
