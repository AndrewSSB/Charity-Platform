using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.DAL.Models.VolunteerModels
{
    public class VolunteerPostModel
    {
        public string Name { get; set; }
        public string lastName { get; set; }
        public string Position { get; set; }
        public string contactDetails { get; set; }
        public int OrganisationId { get; set; }
        public DateTime? DateModified { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}
