using ProiectSoft.DAL.Models.RefugeeModels;
using ProiectSoft.DAL.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.Services.RefugeesServices
{
    public interface IRefugeeServices
    {
        Task<PagedResponse<List<RefugeeGetModel>>> GetAll(PaginationFilter filter, string route);
        Task<RefugeeGetModel> GetById(int id);
        Task Create(RefugeePostModel model);
        Task Update(RefugeePutModel model, int id);
        Task Delete(int id);
        Task<int> CountAsync();
    }
}
