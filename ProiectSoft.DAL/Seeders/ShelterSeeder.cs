using ProiectSoft.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.DAL.Seeders
{
    public class ShelterSeeder : BaseSeeder
    {
        public ShelterSeeder(AppDbContext appDbContext) : base(appDbContext)
        {
        }

        public override void Seed()
        {

            var locationId = getContext().Locations.FirstOrDefault()?.Id;
            var organisationId = getContext().Organisations.FirstOrDefault()?.Id;

            if (locationId != null && organisationId != null)
            {
                var shelter = new Shelter
                {
                    Name = "Raza de speranta",
                    Email = "razadesperanta@info.ro",
                    Phone = "0741498829",
                    availableSpace = 25,
                    LocationId = locationId.Value,
                    OrganisationId = organisationId.Value
                };

                var _shelter = getContext().Shelters.FirstOrDefault(x => x.Name == shelter.Name);

                if (_shelter == null)
                {
                    getContext().Shelters.Add(shelter);
                    getContext().SaveChanges();
                }
            }
        }
    }
}
