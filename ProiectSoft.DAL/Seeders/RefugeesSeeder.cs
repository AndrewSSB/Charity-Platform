using Microsoft.EntityFrameworkCore;
using ProiectSoft.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.DAL.Seeders
{
    public class RefugeesSeeder : BaseSeeder
    {
        public RefugeesSeeder(AppDbContext appDbContext) : base(appDbContext)
        {
        }

        public override void Seed()
        {
            var shelter = getContext().Shelters.FirstOrDefault()?.Id;

            if (shelter != null)
            {
                var refugees = new List<Refugee>();

                for (int i = 0; i < 5; i++)
                {
                    refugees.Add(new Refugee
                    {
                        Name = $"Test{i}",
                        lastName = $"LastTest{i}",
                        Age = i * 10,
                        Details = "-",
                        ShelterId = shelter.Value
                    });
                }

                var _refugees = getContext().Refugees.Count(); //momentan o las asa

                if (_refugees == 0)
                {
                    foreach (var refugee in refugees)
                    {
                        getContext().Refugees.Add(refugee);
                        getContext().SaveChanges();
                    }
                }
            }
        }
    }
}
