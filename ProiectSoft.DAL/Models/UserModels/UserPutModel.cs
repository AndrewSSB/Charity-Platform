using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.DAL.Models.UserModels
{
    public class UserPutModel
    {
        public string FirstName { get; set; } //momentan doar atat poate sa modifice
        public string LastName { get; set; }
        public string Type { get; set; } //it can be a person or an organisation
        public DateTime? DateModified { get; set; }
    }
}
