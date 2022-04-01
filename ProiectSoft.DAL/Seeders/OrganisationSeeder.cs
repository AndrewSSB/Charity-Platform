using ProiectSoft.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.DAL.Seeders
{
    public class OrganisationSeeder : BaseSeeder
    {
        public OrganisationSeeder(AppDbContext appDbContext) : base(appDbContext)
        {
        }

        public override void Seed()
        {
            var id = getContext().Cases.FirstOrDefault()?.Id;

            if (id != null)
            {
                var organisation = new Organisation
                {
                    Name = "Habar N-am",
                    Email = "test@softbinator.com",
                    Phone = "0741498829",
                    Details = "-",
                    CasesId = new Guid(id.Value.ToString())
                };

                if (!getContext().Organisations.Any(x => x.Name == organisation.Name))
                {
                    getContext().Organisations.Add(organisation);
                    getContext().SaveChanges();
                }
            }
        }
    }
}
