using ProiectSoft.DAL.Models.ShelterModels;
using ProiectSoft.DAL.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.Services.SheltersServices
{
    public interface IShelterServices
    {
        Task<PagedResponse<List<ShelterGetModel>>> GetAll(PaginationFilter filter, string route, string searchName, string orderBy, bool descending);
        Task<Response<ShelterGetModel>> GetById(int id);
        Task Create(ShelterPostModel model);
        Task Update(ShelterPutModel model, int id);
        Task Delete(int id);
    }
}
