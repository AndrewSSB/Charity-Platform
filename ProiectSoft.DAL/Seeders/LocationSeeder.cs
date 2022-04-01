using ProiectSoft.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.DAL.Seeders
{
    public class LocationSeeder : BaseSeeder
    {
        public LocationSeeder(AppDbContext appDbContext) : base(appDbContext)
        {
        }

        public override void Seed()
        {
            var location = new Location
            {
                County = "Olt",
                City = "Slatina",
                Street = "Crisan",
                Number = 107
            };

            var _location = getContext().Locations.Where(x => x.City == location.City && x.Street == location.Street && x.Number == location.Number).FirstOrDefault();

            if (_location == null)
            {
                getContext().Locations.Add(location);
                getContext().SaveChanges();
            }
        }
    }
}
