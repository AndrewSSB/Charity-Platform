using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.DAL.Entities
{
    public class Donation
    {
        public int Id { get; set; }
        public string donation { get; set; }
        public Guid UserId { get; set; }
        public int OrganisationId { get; set; }
        public virtual User User { get; set; }
        public virtual Organisation Organisation { get; set; }
    }
}
