using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.DAL.Models.VolunteerModels
{
    public class VolunteerGetModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string lastName { get; set; }
        public string Position { get; set; }
        public string contactDetails { get; set; }
        public int OrganisationId { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
    }
}
