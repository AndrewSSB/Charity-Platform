using ProiectSoft.DAL.Entities;
using ProiectSoft.DAL.Models.OrganisationModels;
using ProiectSoft.DAL.Wrappers;
using ProiectSoft.DAL.Wrappers.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.Services.OrganizationsService
{
    public interface IOrganisationService
    {
        Task<PagedResponse<List<OrganisationGetModel>>> GetAll(OrganisationFilter filter, string route);
        Task<Response<OrganisationGetModel>> GetById(int id);
        Task Create(OrganisationPostModel model);
        Task Update(OrganisationPutModel model, int id);
        Task Delete(int id);
        //Task<List<Organisation>> GetAllLocations();
    }
}
