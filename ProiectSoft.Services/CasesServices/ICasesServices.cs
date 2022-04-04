using ProiectSoft.DAL.Models.CasesModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.Services.CasesServices
{
    public interface ICasesServices
    {
        Task<List<CasesGetModel>> GetAll();
        Task<CasesGetModel> GetById(int id);
        Task Create(CasesPostModel model);
        Task Update(CasesPutModel model, int id);
        Task Delete(int id);
    }
}
