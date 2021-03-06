using ProiectSoft.DAL.Models.VolunteerModels;
using ProiectSoft.DAL.Wrappers;
using ProiectSoft.DAL.Wrappers.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.Services.VolunteersServices
{
    public interface IVolunteerServices
    {
        Task<PagedResponse<List<VolunteerGetModel>>> GetAll(VolunteerFilter filter, string route);
        Task<Response<VolunteerGetModel>> GetById(int id);
        Task Create(VolunteerPostModel model);
        Task Update(VolunteerPutModel model, int id);
        Task Delete(int id);
    }
}
