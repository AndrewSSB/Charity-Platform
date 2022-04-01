using ProiectSoft.DAL.Models.ShelterModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.Services.SheltersServices
{
    public interface IShelterServices
    {
        Task<List<ShelterGetModel>> GetAll();
        Task<ShelterGetModel> GetById(int id);
        Task Create(ShelterPostModel model);
        Task Update(ShelterPutModel model, int id);
        Task Delete(int id);
    }
}
