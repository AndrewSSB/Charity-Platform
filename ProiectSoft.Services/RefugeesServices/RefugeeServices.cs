﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProiectSoft.BLL.Helpers;
using ProiectSoft.DAL;
using ProiectSoft.DAL.Entities;
using ProiectSoft.DAL.Models.RefugeeModels;
using ProiectSoft.DAL.Wrappers;
using ProiectSoft.Services.UriServicess;
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
        private readonly IUriServices _uriService;
        private readonly IMapper _mapper;

        public RefugeeServices(AppDbContext context, IUriServices uriService, IMapper mapper)
        {
            _context = context;
            _uriService = uriService;
            _mapper = mapper;   
        }

        public async Task Create(RefugeePostModel model)
        {
            if (model == null) { return; }

            var shelter = await _context.Shelters.FirstOrDefaultAsync(x => x.Id == model.ShelterId);

            if (shelter == null) { return; }

            var refugee = _mapper.Map<Refugee>(model);

            //de fiecare data cand adaug un refugiat, available space din shelter scade

            //var space = await _context.Shelters.FirstOrDefaultAsync(x => x.Id == model.ShelterId); //momentan o las asa, ceri ajutor mai tarziu

            shelter.availableSpace -= 1;

            await _context.AddAsync(refugee);
            await _context.SaveChangesAsync();
            
        }

        public async Task Delete(int id)
        {
            var refugee = await _context.Refugees.FirstOrDefaultAsync(x => x.Id == id);

            if (refugee == null)
            {
                return;
            }

            var space = await _context.Shelters.Where(x => x.Id == refugee.ShelterId).FirstOrDefaultAsync(); //cred ca asa vine ? //asa o las momentan

            if (space != null)
            {
                space.availableSpace += 1;
            }

            _context.Refugees.Remove(refugee);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<List<RefugeeGetModel>>> GetAll(PaginationFilter filter, string route)
        {
            var refugees = await _context.Refugees
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var refugeeModels = new List<RefugeeGetModel>();

            foreach (var refugee in refugees)
            {
                var refugeeModel = _mapper.Map<RefugeeGetModel>(refugee);
                refugeeModels.Add(refugeeModel);
            }

            var refugeesListCount = await _context.Refugees.CountAsync();

            var pagedResponse = PaginationHelper.CreatePagedReponse<RefugeeGetModel>(refugeeModels, filter, refugeesListCount, _uriService, route);

            return pagedResponse;
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
            //aici la fel
            var refugeeShel = await _context.Refugees.FirstOrDefaultAsync(x => x.ShelterId == model.ShelterId);

            if (refugeeShel != null)
            {
                refugee.ShelterId = model.ShelterId;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _context.Refugees.CountAsync();
        }
    }
}
