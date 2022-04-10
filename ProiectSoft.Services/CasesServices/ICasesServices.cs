using ProiectSoft.DAL.Models.CasesModels;
using ProiectSoft.DAL.Wrappers;
using ProiectSoft.DAL.Wrappers.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.Services.CasesServices
{
    public interface ICasesServices
    {
        Task<PagedResponse<List<CasesGetModel>>> GetAll(CasesFilter filter, string route);
        Task<Response<CasesGetModel>> GetById(int id);
        Task Create(CasesPostModel model);
        Task Update(CasesPutModel model, int id);
        Task Delete(int id);
    }
}
