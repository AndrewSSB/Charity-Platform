using ProiectSoft.DAL.Models.OrganisationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.Services.OrganizationsService
{
    public interface IOrganisationService
    {
        Task<List<OrganisationGetModel>> GetAll();
        Task<OrganisationGetModel> GetById(int id);
        Task Create(OrganisationPostModel model);
        Task Update(OrganisationPutModel model, int id);
        Task Delete(int id);
    }
}
