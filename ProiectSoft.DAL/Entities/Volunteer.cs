using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.DAL.Entities
{
    public class Volunteer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string lastName { get; set; }
        public string Position { get; set; }
        public string contactDetails { get; set; }
        public int OrganisationId { get; set; }
        public virtual Organisation Organisation { get; set; }
    }
}
