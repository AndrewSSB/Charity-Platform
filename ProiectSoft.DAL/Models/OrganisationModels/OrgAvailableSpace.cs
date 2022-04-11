using ProiectSoft.DAL.Models.ShelterModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.DAL.Models.OrganisationModels
{
    public class OrgAvailableSpace : OrganisationGetModel
    {
        public virtual ICollection<ShelterGetModel> Shelters { get; set; }
    }
}
