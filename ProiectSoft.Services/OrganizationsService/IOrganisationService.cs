using ProiectSoft.DAL.Models.OrganisationModels;
using ProiectSoft.DAL.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.Services.OrganizationsService
{
    public interface IOrganisationService
    {
        Task<PagedResponse<List<OrganisationGetModel>>> GetAll(PaginationFilter filter, string route, 
            string searchName, string orderBy, bool descending, string[] filters);
        Task<Response<OrganisationGetModel>> GetById(int id);
        Task Create(OrganisationPostModel model);
        Task Update(OrganisationPutModel model, int id);
        Task Delete(int id);
    }
}
