using ProiectSoft.DAL.Models.ShelterModels;
using ProiectSoft.DAL.Wrappers;
using ProiectSoft.DAL.Wrappers.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.Services.SheltersServices
{
    public interface IShelterServices
    {
        Task<PagedResponse<List<ShelterGetModel>>> GetAll(ShelterFilter filter, string route);
        Task<Response<ShelterGetModel>> GetById(int id);
        Task Create(ShelterPostModel model);
        Task Update(ShelterPutModel model, int id);
        Task Delete(int id);
    }
}
