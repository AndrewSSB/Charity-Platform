using ProiectSoft.DAL.Models.VolunteerModels;
using ProiectSoft.DAL.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.Services.VolunteersServices
{
    public interface IVolunteerServices
    {
        Task<PagedResponse<List<VolunteerGetModel>>> GetAll(PaginationFilter filter, string route, string searchName, string orderBy, bool descending);
        Task<Response<VolunteerGetModel>> GetById(int id);
        Task Create(VolunteerPostModel model);
        Task Update(VolunteerPutModel model, int id);
        Task Delete(int id);
    }
}
