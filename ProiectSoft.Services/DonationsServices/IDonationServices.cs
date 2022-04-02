using ProiectSoft.DAL.Models.DonationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.Services.DonationsServices
{
    public interface IDonationServices
    {
        Task<List<DonationGetModel>> GetAll();
        Task<DonationGetModel> GetById(int id);
        Task Create(DonationPostModel model);
        Task Update(DonationPutModel model, int id);
        Task Delete(int id);
    }
}
