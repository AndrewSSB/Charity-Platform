using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.DAL.Seeders
{
    public abstract class BaseSeeder
    {
        private readonly AppDbContext _appDbContext;

        public BaseSeeder(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public AppDbContext getContext()
        {
            return _appDbContext;
        } 

        public abstract void Seed();
    }
}
