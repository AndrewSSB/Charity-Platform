using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.DAL.Entities
{
    public class Shelter : BaseEntity.BaseEntity
    {
        public string Name { get; set; }
        public int? availableSpace { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int LocationId { get; set; }
        public int OrganisationId { get; set; }
        public virtual Location Location { get; set; } //prima metoda de 1:1
        public virtual ICollection<Refugee> Refugees { get; set; }
        public virtual Organisation Organisation { get; set; }
    }
}
