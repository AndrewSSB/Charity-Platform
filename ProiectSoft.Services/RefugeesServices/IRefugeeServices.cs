using ProiectSoft.DAL.Models.RefugeeModels;
using ProiectSoft.DAL.Wrappers;
using ProiectSoft.DAL.Wrappers.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.Services.RefugeesServices
{
    public interface IRefugeeServices
    {
        Task<PagedResponse<List<RefugeeGetModel>>> GetAll(RefugeesFilter filter, string route);
        Task<Response<RefugeeGetModel>> GetById(int id);
        Task Create(RefugeePostModel model);
        Task Update(RefugeePutModel model, int id);
        Task Delete(int id);
        //Task<List<RefugeeGetModel>> OrderBy(List<RefugeeGetModel> refugees, string orderBy, bool descending);
    }
}
