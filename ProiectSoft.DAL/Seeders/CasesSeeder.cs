using ProiectSoft.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.DAL.Seeders
{
    public class CasesSeeder : BaseSeeder
    {
        public CasesSeeder(AppDbContext appDbContext) : base(appDbContext)
        {
        }
        public override void Seed()
        {
            var _case = new Cases
            {
                caseName = "Ukranian Refugees",
                caseDetails = "Helping refugees with shelters and food",
                startDate = DateTime.Now,
                closed = false,
                DateCreated = DateTime.Now
            };

            if (!getContext().Cases.Any(x => x.caseName == _case.caseName))
            {
                getContext().Cases.Add(_case);
                getContext().SaveChanges();
            }
        }
    }
}
