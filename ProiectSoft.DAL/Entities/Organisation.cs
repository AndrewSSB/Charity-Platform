using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.DAL.Entities
{
    public class Organisation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Details { get; set; }
        public Guid CasesId { get; set; }
        public virtual ICollection<Shelter> Shelters { get; set; }
        public virtual ICollection<Volunteer> Volunteers { get; set; }
        public virtual Cases Case { get; set; }
        public virtual ICollection<Donation> Donations { get; set; }
    }
}
