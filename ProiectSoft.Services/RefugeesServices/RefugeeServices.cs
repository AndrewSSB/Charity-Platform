using Microsoft.EntityFrameworkCore;
using ProiectSoft.DAL;
using ProiectSoft.DAL.Entities;
using ProiectSoft.DAL.Models.RefugeeModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.Services.RefugeesServices
{
    public class RefugeeServices : IRefugeeServices
    {
        private readonly AppDbContext _context;

        public RefugeeServices(AppDbContext context)
        {
            _context = context;
        }

        public async Task Create(RefugeePostModel model)
        {
            if (model != null)
            {
                var refugee = new Refugee
                {
                    Name = model.Name,
                    lastName = model.Name,
                    Age = model.Age,
                    Details = model.Details,
                    ShelterId = model.ShelterId
                };

                await _context.AddAsync(refugee);
                await _context.SaveChangesAsync();
            }
        }

        public async Task Delete(int id)
        {
            var refugee = await _context.Refugees.FirstOrDefaultAsync(x => x.Id == id);

            if (refugee == null)
            {
                return;
            }

            _context.Refugees.Remove(refugee);
            await _context.SaveChangesAsync();
        }

        public async Task<List<RefugeeGetModel>> GetAll()
        {
            return await _context.Refugees.Select(x => new RefugeeGetModel
            {
                Id = x.Id,
                Name = x.Name,
                lastName = x.Name,
                Age = x.Age,
                Details = x.Details,
                ShelterId = x.ShelterId
            }).ToListAsync();
        }

        public async Task<RefugeeGetModel> GetById(int id)
        {
            var refugee = await _context.Refugees.FirstOrDefaultAsync(x => x.Id == id);

            if (refugee == null)
            {
                return new RefugeeGetModel();
            }
            return new RefugeeGetModel
            {
                Id = refugee.Id,
                Name = refugee.Name,
                lastName = refugee.Name,
                Age = refugee.Age,
                Details = refugee.Details,
                ShelterId = refugee.ShelterId
            };
        }

        public async Task Update(RefugeePutModel model, int id)
        {
            var refugee = await _context.Refugees.FirstOrDefaultAsync(x => x.Id == id);

            if (model == null || refugee == null) { return; }

            refugee.Name = model.Name;
            refugee.lastName = model.lastName;
            refugee.Age = model.Age;
            refugee.Details = model.Details;

            var refugeeShel = await _context.Refugees.FirstOrDefaultAsync(x => x.ShelterId == model.ShelterId);

            if (refugeeShel != null)
            {
                refugee.ShelterId = model.ShelterId;
            }

            await _context.SaveChangesAsync();
        }
    }
}
