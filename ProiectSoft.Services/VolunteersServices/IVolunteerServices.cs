using ProiectSoft.DAL.Models.VolunteerModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.Services.VolunteersServices
{
    public interface IVolunteerServices
    {
        Task<List<VolunteerGetModel>> GetAll();
        Task<VolunteerGetModel> GetById(int id);
        Task Create(VolunteerPostModel model);
        Task Update(VolunteerPutModel model, int id);
        Task Delete(int id);
    }
}
